using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Singleton<Player> {
    Animator animator;
    Camera cam;
    public CharacterController characterController;
    public Renderer RD;
    [SerializeField]
    Image HpBar;
    public float HP = 200;
    public float MaxHp = 200;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float AttackDelay = 2.2f;
    public float RollDelay = 0.7f;
    public float RollDistance = 5.0f;
    private bool isRoll = false;
    private bool isAttack = false;
    Vector3 moveDirection;
    Vector3 rollDirection;
    Vector3 knockbackDir;
    public Weapon weapon;
    public float smoothness = 10f;
    float hitDelay = 0;
    private bool IsHit;

    void Start() {
        animator = GetComponent<Animator>();
        cam = Camera.main;
        characterController = GetComponent<CharacterController>();
    }

    void Update() {
        if (SystemMng.ins.state == SystemMng.STATE.PAUSE)
            return;
        hitDelay += Time.deltaTime;
        moveDirection.y += gravity * 0.1f;  //characterController.isGround() 오작동 방지
        if (isAttack)
            return;
        Roll();
        if (isRoll || isAttack)
            return;
        Move();
        Attack1();
    }


    private void LateUpdate() {
    }

    //구르기
    private void Roll() {
        if (Input.GetKeyDown(KeyCode.Space) && !isRoll) {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            rollDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
            if (rollDirection == Vector3.zero)
                return;
            animator.SetTrigger("Roll");
            isRoll = true;
            StartCoroutine(Rolling());
        } else if (isRoll) {
            transform.rotation = Quaternion.LookRotation(rollDirection);
            characterController.Move(rollDirection.normalized * RollDistance * Time.deltaTime);
        }
    }
    private void Move() {

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);



        if (Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f) { // 입력 있을때 카메라가 보는 방향기준으로 회전
            Vector3 playerRotate = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }

        if (!characterController.isGrounded) {
            print("중력작용");
            moveDirection.y += gravity;
        }

        moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal") + new Vector3(0, moveDirection.y, 0);
        characterController.Move(moveDirection.normalized * speed * Time.deltaTime);

        moveDirection = Vector3.Scale(moveDirection, new Vector3(1, 0, 1));
        animator.SetFloat("Speed", moveDirection.magnitude, 0.1f, Time.deltaTime);
    }

    private void Attack1() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Vector3 playerRotate = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.LookRotation(playerRotate);
            isAttack = true;
            animator.SetTrigger("Attack1");
            weapon.Attack();
            StartCoroutine(Attack());
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy") {
            if (hitDelay > 1.0f) {// 맞고 1초간 다시 안맞게
                //IsHit = true;
                hitDelay = 0f;
                HP -= 20;
                HpBar.fillAmount = HP / MaxHp;
                if (HP <= 0) {
                    SystemMng.ins.StartCoroutine(SystemMng.ins.DieCoroutine());
                }
                knockbackDir = transform.position - other.transform.position;
                StartCoroutine(HitCoroutine());
            }
        }
    }

    IEnumerator Rolling() {
        yield return new WaitForSeconds(RollDelay);//1.5초동안 다른 입력x;
        isRoll = false;
    }
    IEnumerator Attack() {
        yield return new WaitForSeconds(AttackDelay);//AttackDelay동안 다른 입력x;
        isAttack = false;
    }
    IEnumerator HitCoroutine() {

        knockbackDir = knockbackDir + new Vector3(0, 1, 0);

        characterController.Move(knockbackDir.normalized * 50 * Time.deltaTime);

        for (int i = 0; i < 5; i++) {
            RD.material.SetFloat("_Float1", 0.5f);
            yield return new WaitForSeconds(0.1f);
            RD.material.SetFloat("_Float1", 0f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
