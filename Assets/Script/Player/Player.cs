using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : Singleton<Player> {
    //public enum PLAYERSTATE { IDLE, ATTACK, ROLL }
    //public PLAYERSTATE state = PLAYERSTATE.IDLE;
    Animator animator;
    Camera cam;
    [SerializeField] CameraMove camMove;
    public Weapon weapon;
    public CharacterController characterController;
    public Renderer RD;

    [SerializeField] Image hpBar;
    public TMP_Text hpText;
    public float hp = 200;
    public float maxHp = 200;

    public float speed = 5f;
    public float runSpeed = 2f;
    public float gravity = -9.8f;
    public float attackDelay;
    public float rollDelay;
    public float rollDistance = 10.0f;
    public float smoothness = 10f;
    public float power = 20;
    float hitDelay = 0;
    private bool isRoll = false;
    private bool isRun = false;
    Vector3 moveDirection;
    Vector3 rollDirection;
    Vector3 knockbackDir;
    Vector3 moveInput;

    Transform currentTarget;
    [SerializeField] Transform lockOnCanvas;
    [SerializeField] bool zeroVert_Look;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] Transform enemyTarget_Locator;
    public bool enemyLocked;
    Vector3 pos;
    float currentYOffset;

    void Start() {
        animator = GetComponent<Animator>();
        cam = Camera.main;
        characterController = GetComponent<CharacterController>();
    }

    void Update() {

        hpText.text = hp + "/" + maxHp;

        if (SystemMng.ins.state == SystemMng.STATE.PAUSE)
            return;

        hitDelay += Time.deltaTime;
        moveDirection.y += gravity * 0.1f;      //characterController.isGround() 오작동 방지

        if (attackDelay > 0) {  //attackDelay동안 다른 입력x
            attackDelay -= Time.deltaTime;
            return;
        }
        if (isRoll) //구르는 동안 다른 입력x
            return;

        if (Input.GetKey(KeyCode.LeftShift)) {      //달리기
            isRun = true;
        } else {
            isRun = false;
        }
        Roll();
        Move();
        LockOn();
        Attack1();
    }



    private void Move() {
        Vector3 f;
        Vector3 r;
        if (enemyLocked) {  //락온중 사용
            f = transform.TransformDirection(Vector3.forward);
            r = transform.TransformDirection(Vector3.right);
        } else {            //평상시 사용
            f = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));     // cam의 forward에서 y성분 제거
            r = Vector3.Scale(cam.transform.right, new Vector3(1, 0, 1));       // cam의 right에서 y성분 제거
        }


        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (moveInput.magnitude > 0.5f && !enemyLocked) {       // 입력 있을때 카메라가 보는 방향을 기준으로 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(f * moveInput.y + r * moveInput.x), Time.deltaTime * smoothness);
        }

        if (!characterController.isGrounded) {      //중력
            moveDirection.y += gravity;
        }

        moveDirection = f * moveInput.y + r * moveInput.x + new Vector3(0, moveDirection.y, 0);

        if (isRun)
            runSpeed = 2f;
        else
            runSpeed = 1f;

        characterController.Move(moveDirection.normalized * speed * runSpeed * Time.deltaTime);

        moveDirection = Vector3.Scale(moveDirection, new Vector3(1, 0, 1));
        //평소 애니메이션
        animator.SetFloat("Speed", moveDirection.normalized.magnitude * runSpeed, 0.1f, Time.deltaTime);
        //락온 애니메이션
        animator.SetFloat("Horizontal", moveInput.x, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", moveInput.y, 0.1f, Time.deltaTime);
    }

    //구르기
    private void Roll() {
        if (enemyLocked)        //락온중 구르기 방지
            return;

        Vector3 forward = transform.TransformDirection(Vector3.forward);    //로컬 forward 를 월드 forward로
        Vector3 right = transform.TransformDirection(Vector3.right);    //로컬 right 를 월드 right로

        if (Input.GetKeyDown(KeyCode.Space) && !isRoll) {
            rollDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

            if (rollDirection == Vector3.zero)      //방향키 입력없이 Space만 누를 경우 구르지 않음
                return;
            rollDelay = 0.8f;
            animator.SetTrigger("Roll");
            StartCoroutine("Rolling",forward);

        }
    }

    private void Attack1() {
        if (Input.GetKey(KeyCode.Mouse0)) {

            Vector3 playerRotate = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.LookRotation(playerRotate);

            animator.SetTrigger("Attack1");
            weapon.Attack();
            attackDelay = 1.1f;
        }
    }
    private void LockOn() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            if (currentTarget) {
                ResetTarget();
                return;
            }

            if (currentTarget = ScanNearBy()) FoundTarget();
            else ResetTarget();
        }

        if (currentTarget != null)
            if (currentTarget.gameObject.activeSelf == false)       //타겟 비활성시(적 사망) ResetTarget
                ResetTarget();

        if (enemyLocked) {
            StartCoroutine(LookTarget());
        }
    }
    private Transform ScanNearBy() {
        //구 범위의 targetLayers인 Collider찾기
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, 15f, targetLayers);
        float closestAngle = 60f;
        Transform closestTarget = null;

        if (nearbyTargets.Length <= 0) return null;     //타겟없으면 return null;

        for (int i = 0; i < nearbyTargets.Length; i++) {
            Vector3 dir = nearbyTargets[i].transform.position - cam.transform.position;       //cam에서 타겟방향 벡터
            dir.y = 0;
            float _angle = Vector3.Angle(cam.transform.forward, dir);                         //cam의 forward와 타겟사이의 각도

            if (_angle < closestAngle)      //closestAngle보다 작은각도면 그 타겟 저장, closetAngle 갱신
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = _angle;
            }
        }

        if (!closestTarget) return null;        //60보다 작은각X return null;

        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if (Blocked(tarPos)) return null;       //타겟사이에 장애물 있으면 return null;
        return closestTarget;
    }
    void FoundTarget() {
        lockOnCanvas.gameObject.SetActive(true);
        animator.SetLayerWeight(1, 0);      //NormalMode 레이어 가중치0
        animator.SetLayerWeight(2, 1);      //StrafeMode 레이어 가중치1
        enemyLocked = true;
    }

    public void ResetTarget() {     //락온 취소
        lockOnCanvas.gameObject.SetActive(false);
        currentTarget = null;
        camMove.rotX = camMove.transform.localRotation.eulerAngles.x;
        camMove.rotY = camMove.transform.localRotation.eulerAngles.y;
        enemyLocked = false;
        animator.SetLayerWeight(1, 1);      //NormalMode 레이어 가중치1
        animator.SetLayerWeight(2, 0);      //StrafeMode 레이어 가중치0
    }

    bool Blocked(Vector3 t) {
        RaycastHit hit;
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit)) {
            if (!hit.transform.CompareTag("Enemy")) return true;
        }
        return false;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy" && !isRoll) {
            int damage = 5;
            print(other.name);
            if (hitDelay > 1.0f) {
                hitDelay = 0f;
                switch (other.name) {
                    case "Vellum BT":
                    case "Fire Ball":
                        damage = 20;
                        break;
                    case "Vellum_tail":
                        damage = 10;
                        break;
                    default:
                        damage = 5;
                        break;
                }
                hp -= damage;
                hpBar.fillAmount = hp / maxHp;
                if (hp <= 0) {
                    SystemMng.ins.StartCoroutine(SystemMng.ins.DieCoroutine());
                }
                knockbackDir = transform.position - other.transform.position;
                knockbackDir.y = 0;
                StartCoroutine(HitCoroutine());
            }
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.tag == "Enemy" && !isRoll) {
            if (hitDelay > 1.0f) {
                hitDelay = 0f;
                hp -= 20;
                hpBar.fillAmount = hp / maxHp;
                if (hp <= 0) {
                    SystemMng.ins.StartCoroutine(SystemMng.ins.DieCoroutine());
                }
                knockbackDir = transform.position - other.transform.position;
                knockbackDir.y = 0;
                StartCoroutine(HitCoroutine());
            }
        }
    }
    IEnumerator LookTarget() {      //락온중 타겟에 Locator를 붙이고 캐릭터의 방향을 타겟에 고정하는 함수
        if (currentTarget == null) {
            ResetTarget();
            yield return null;
        }
        while (enemyLocked) {
            pos = currentTarget.position + new Vector3(0, currentYOffset, 0);
            lockOnCanvas.position = pos;
            lockOnCanvas.localScale = Vector3.one * ((cam.transform.position - pos).magnitude * 0.1f);      //타겟과의 거리에 따라 크기 조절

            enemyTarget_Locator.position = pos;
            Vector3 dir = currentTarget.position - transform.position;
            Vector3 camDir = currentTarget.position - cam.transform.position;
            dir.y = 0;
            camDir.y = -3f;
            Quaternion rot = Quaternion.LookRotation(dir);
            Quaternion camRot = Quaternion.LookRotation(camDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 20);
            camMove.transform.rotation = Quaternion.Lerp(camMove.transform.rotation, camRot, Time.deltaTime * 20);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return null;
    }
    IEnumerator Rolling(Vector3 forward) {

        isRoll = true;
        while (rollDelay>0) {
            rollDelay -= Time.deltaTime;
            characterController.Move(forward * rollDistance * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isRoll = false;
    }
    IEnumerator HitCoroutine() {

        //knockbackDir = knockbackDir + new Vector3(0, 1, 0);

        characterController.Move(knockbackDir.normalized * 50 * Time.deltaTime);

        int r = UnityEngine.Random.Range(0, 3);

        switch (r) {
            case 0:
                SoundMng.ins.PlayEffect("body_hit_small_11");
                break;
            case 1:
                SoundMng.ins.PlayEffect("body_hit_small_20");
                break;
            case 2:
                SoundMng.ins.PlayEffect("body_hit_small_23");
                break;
            default:
                break;
        }
        for (int i = 0; i < 5; i++) {
            RD.material.SetFloat("_Float1", 0.5f);
            yield return new WaitForSeconds(0.1f);
            RD.material.SetFloat("_Float1", 0f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDrawGizmos() {       //락온 인식범위 확인용
        Gizmos.DrawWireSphere(transform.position, 15);
    }
}
