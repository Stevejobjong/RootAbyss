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
        moveDirection.y += gravity * 0.1f;      //characterController.isGround() ���۵� ����

        if (attackDelay > 0) {  //attackDelay���� �ٸ� �Է�x
            attackDelay -= Time.deltaTime;
            return;
        }
        if (isRoll) //������ ���� �ٸ� �Է�x
            return;

        if (Input.GetKey(KeyCode.LeftShift)) {      //�޸���
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
        if (enemyLocked) {  //������ ���
            f = transform.TransformDirection(Vector3.forward);
            r = transform.TransformDirection(Vector3.right);
        } else {            //���� ���
            f = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));     // cam�� forward���� y���� ����
            r = Vector3.Scale(cam.transform.right, new Vector3(1, 0, 1));       // cam�� right���� y���� ����
        }


        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (moveInput.magnitude > 0.5f && !enemyLocked) {       // �Է� ������ ī�޶� ���� ������ �������� ȸ��
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(f * moveInput.y + r * moveInput.x), Time.deltaTime * smoothness);
        }

        if (!characterController.isGrounded) {      //�߷�
            moveDirection.y += gravity;
        }

        moveDirection = f * moveInput.y + r * moveInput.x + new Vector3(0, moveDirection.y, 0);

        if (isRun)
            runSpeed = 2f;
        else
            runSpeed = 1f;

        characterController.Move(moveDirection.normalized * speed * runSpeed * Time.deltaTime);

        moveDirection = Vector3.Scale(moveDirection, new Vector3(1, 0, 1));
        //��� �ִϸ��̼�
        animator.SetFloat("Speed", moveDirection.normalized.magnitude * runSpeed, 0.1f, Time.deltaTime);
        //���� �ִϸ��̼�
        animator.SetFloat("Horizontal", moveInput.x, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", moveInput.y, 0.1f, Time.deltaTime);
    }

    //������
    private void Roll() {
        if (enemyLocked)        //������ ������ ����
            return;

        Vector3 forward = transform.TransformDirection(Vector3.forward);    //���� forward �� ���� forward��
        Vector3 right = transform.TransformDirection(Vector3.right);    //���� right �� ���� right��

        if (Input.GetKeyDown(KeyCode.Space) && !isRoll) {
            rollDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

            if (rollDirection == Vector3.zero)      //����Ű �Է¾��� Space�� ���� ��� ������ ����
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
            if (currentTarget.gameObject.activeSelf == false)       //Ÿ�� ��Ȱ����(�� ���) ResetTarget
                ResetTarget();

        if (enemyLocked) {
            StartCoroutine(LookTarget());
        }
    }
    private Transform ScanNearBy() {
        //�� ������ targetLayers�� Colliderã��
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, 15f, targetLayers);
        float closestAngle = 60f;
        Transform closestTarget = null;

        if (nearbyTargets.Length <= 0) return null;     //Ÿ�پ����� return null;

        for (int i = 0; i < nearbyTargets.Length; i++) {
            Vector3 dir = nearbyTargets[i].transform.position - cam.transform.position;       //cam���� Ÿ�ٹ��� ����
            dir.y = 0;
            float _angle = Vector3.Angle(cam.transform.forward, dir);                         //cam�� forward�� Ÿ�ٻ����� ����

            if (_angle < closestAngle)      //closestAngle���� ���������� �� Ÿ�� ����, closetAngle ����
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = _angle;
            }
        }

        if (!closestTarget) return null;        //60���� ������X return null;

        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if (Blocked(tarPos)) return null;       //Ÿ�ٻ��̿� ��ֹ� ������ return null;
        return closestTarget;
    }
    void FoundTarget() {
        lockOnCanvas.gameObject.SetActive(true);
        animator.SetLayerWeight(1, 0);      //NormalMode ���̾� ����ġ0
        animator.SetLayerWeight(2, 1);      //StrafeMode ���̾� ����ġ1
        enemyLocked = true;
    }

    public void ResetTarget() {     //���� ���
        lockOnCanvas.gameObject.SetActive(false);
        currentTarget = null;
        camMove.rotX = camMove.transform.localRotation.eulerAngles.x;
        camMove.rotY = camMove.transform.localRotation.eulerAngles.y;
        enemyLocked = false;
        animator.SetLayerWeight(1, 1);      //NormalMode ���̾� ����ġ1
        animator.SetLayerWeight(2, 0);      //StrafeMode ���̾� ����ġ0
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
    IEnumerator LookTarget() {      //������ Ÿ�ٿ� Locator�� ���̰� ĳ������ ������ Ÿ�ٿ� �����ϴ� �Լ�
        if (currentTarget == null) {
            ResetTarget();
            yield return null;
        }
        while (enemyLocked) {
            pos = currentTarget.position + new Vector3(0, currentYOffset, 0);
            lockOnCanvas.position = pos;
            lockOnCanvas.localScale = Vector3.one * ((cam.transform.position - pos).magnitude * 0.1f);      //Ÿ�ٰ��� �Ÿ��� ���� ũ�� ����

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

    private void OnDrawGizmos() {       //���� �νĹ��� Ȯ�ο�
        Gizmos.DrawWireSphere(transform.position, 15);
    }
}
