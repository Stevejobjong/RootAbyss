using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vellum : Boss {
    public Renderer[] RD;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem hitPs;
    [SerializeField] ParticleSystem ps;
    [SerializeField] Image HpBar;
    public Transform muzzle;
    public GameObject FireBall;
    public GameObject Camera;
    public float HP = 600;
    public float MaxHp = 600;
    public float shakePower = 0.3f;
    float hitDelay = 0;
    protected override void Update() {
        base.Update();
        hitDelay += Time.deltaTime;

        if (patternSecond) {
            StartCoroutine(pattern2());
            patternSecond = false;
        }
    }

    protected override IEnumerator pattern1() {
        StartCoroutine(Burrow());
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Move());
        yield return new WaitForSeconds(3.0f);
        warning.CreateWarning();
        yield return new WaitForSeconds(1.5f);
        warning.DestoryWarning();
        StartCoroutine(Out());
    }
    IEnumerator pattern2() {
        ps.Play();
        yield return new WaitForSeconds(1.5f);
        FireBall.gameObject.SetActive(true);
        FireBall.transform.position = muzzle.position;
        Vector3 d = Player.ins.transform.position + new Vector3(0, 0.5f, 0);
        while (FireBall.transform.position.y > 1) {
            FireBall.transform.position = Vector3.Lerp(FireBall.transform.position, d, Time.deltaTime * 4);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        FireBall.gameObject.SetActive(false);
        yield return null;

    }

    public IEnumerator CameraShake(float time, float power) {
        float timer = 0;

        while (timer <= time) {
            Camera.transform.localPosition = Random.insideUnitSphere * power + Camera.transform.localPosition;

            timer += Time.deltaTime;
            yield return null;
        }

    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Weapon") {
            if (hitDelay > 1.0f) {// 맞고 1초간 다시 안맞게
                hitDelay = 0f;
                HP -= 20;
                HpBar.fillAmount = HP / MaxHp;
                hitPs.transform.rotation = Quaternion.LookRotation(Player.ins.transform.position);
                hitPs.Play();
                if (HP <= 0) {
                    //isDead = true;
                    StartCoroutine(die());
                } else {
                    StartCoroutine(hit());
                }
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
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(PlayerPos.x, transform.position.y, PlayerPos.z), 20 * Time.deltaTime);
            BossPos = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
            PlayerPos = Vector3.Scale(Player.ins.transform.position, new Vector3(1, 0, 1));
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
    IEnumerator die() {
        animator.SetBool("Die", true);
        yield return new WaitForSeconds(2f);
        float alpha = 2.0f;
        while (alpha >= 0) {
            for (int j = 0; j < RD.Length; j++) {
                RD[j].material.SetFloat("_Alpha", alpha);
            }
            yield return new WaitForSeconds(0.1f);
            alpha -= 0.1f;
        }
        gameObject.SetActive(false);
        SystemMng.ins.StartCoroutine(SystemMng.ins.ClearCoroutine());

    }
}
