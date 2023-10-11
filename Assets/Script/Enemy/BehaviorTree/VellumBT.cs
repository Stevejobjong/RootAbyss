using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VellumBT : BossBT {
    public Renderer[] RD;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem hitPs;
    [SerializeField] ParticleSystem ps;
    [SerializeField] Image HpBar;
    [SerializeField] CapsuleCollider cc;
    public Transform muzzle;
    public GameObject FireBall;
    public GameObject Camera;
    public float HP = 600;
    public float MaxHp = 600;
    public float shakePower = 0.1f;
    public bool isAttacking = false;
    float hitDelay = 0;
    protected override void Update() {
        base.Update();
        hitDelay += Time.deltaTime;
    }
    public override IEnumerator Pattern1() {
        isAttacking = true;
        StartCoroutine(Burrow());
        yield return new WaitForSeconds(1.0f);
        Player.ins.ResetTarget();
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(Move());
        warning.CreateWarning();
        yield return new WaitForSeconds(1.5f);
        warning.DestoryWarning();
        StartCoroutine(Out());
        yield return new WaitForSeconds(3.0f);
        isAttacking = false;
    }
    public IEnumerator Pattern2() {
        isAttacking = true;
        float t = 0f;
        animator.SetTrigger("FireBall");
        yield return new WaitForSeconds(0.5f);
        ps.Play();
        yield return new WaitForSeconds(1.3f);
        FireBall.transform.position = muzzle.position;
        FireBall.gameObject.SetActive(true);
        Vector3 d = Player.ins.transform.position + new Vector3(0, 0.5f, 0);
        while (FireBall.transform.position.y > 1 && t<2.5f) {
            FireBall.transform.position = Vector3.Lerp(FireBall.transform.position, d, Time.deltaTime * 4);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        FireBall.gameObject.SetActive(false);
        yield return new WaitForSeconds(3.0f);
        isAttacking = false;
    }

    public IEnumerator CameraShake(float time, float power) {
        float timer = 0;

        while (timer <= time) {
            if (SystemMng.ins.state == SystemMng.STATE.PLAY)
                Camera.transform.localPosition = Random.insideUnitSphere
                    * power + Camera.transform.localPosition;
            timer += Time.deltaTime;
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Weapon") {
            if (hitDelay > 1.0f) {
                hitDelay = 0f;
                int num = Random.Range(0, 3);
                float damage = Player.ins.power;
                switch (num) {
                    case 0:
                        damage -= 10;
                        break;
                    case 1:
                        damage *= 2;
                        break;
                    default:
                        break;
                }
                HP -= damage;
                HpBar.fillAmount = HP / MaxHp;
                hitPs.transform.rotation = Quaternion.LookRotation(Player.ins.transform.position);
                hitPs.Play();
                int r = Random.Range(0, 3);
                switch (r) {
                    case 0:
                        SoundMng.ins.PlayEffect("body_hit_large_32");
                        break;
                    case 1:
                        SoundMng.ins.PlayEffect("body_hit_large_44");
                        break;
                    case 2:
                        SoundMng.ins.PlayEffect("body_hit_large_76");
                        break;
                    default:
                        break;
                }
                StartCoroutine(hit());
                print("Hit! HP : " + HP);
            }
        }
    }

    public override IEnumerator Out() {
        StartCoroutine(CameraShake(1.0f, shakePower));
        while (gameObject.transform.position.y < max) {
            transform.Translate(0, Time.deltaTime * 20, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    protected override IEnumerator Move() {
        Vector3 BossPos = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
        Vector3 PlayerPos = Vector3.Scale(Player.ins.transform.position, new Vector3(1, 0, 1));

        StartCoroutine(CameraShake(2.0f, shakePower));
        while (Vector3.Distance(BossPos, PlayerPos) > 2f) {
            BossPos = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            PlayerPos = Vector3.Scale(Player.ins.transform.position, new Vector3(1, 0, 1));
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(PlayerPos.x,
                transform.position.y, PlayerPos.z), 30 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    IEnumerator hit() {
        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < RD.Length; j++) {
                RD[j].material.SetFloat("_Float1", 0.5f);
            }
            yield return new WaitForSeconds(0.1f);
            for (int j = 0; j < RD.Length; j++) {
                RD[j].material.SetFloat("_Float1", 0f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator die() {

        if (Player.ins.enemyLocked)
            Player.ins.ResetTarget();
        SystemMng.ins.vellumAi.gameObject.SetActive(false);
        SystemMng.ins.tailAi.gameObject.SetActive(false);
        animator.SetTrigger("Die");
        isDead = true;
        cc.enabled = false;
        yield return new WaitForSeconds(2f);
        float alpha = 1.0f;
        while (alpha >= 0) {
            for (int j = 0; j < RD.Length; j++) {
                RD[j].material.SetFloat("_Alpha", alpha);
            }
            yield return new WaitForSeconds(Time.deltaTime);
            alpha -= Time.deltaTime;
        }
        gameObject.SetActive(false);
        SystemMng.ins.StartCoroutine(SystemMng.ins.ClearCoroutine());

    }
}
