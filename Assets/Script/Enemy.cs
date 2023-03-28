using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Enemy : MonoBehaviour {
    public Renderer RD;
    Rigidbody rb;
    NavMeshAgent agent;

    [SerializeField] ParticleSystem ps;
    [SerializeField] Image HpBar;
    float hitDelay = 0;
    public float knockbackPower = 5.0f;
    public float HP = 200;
    public float MaxHp = 200;
    bool isDead = false;
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        if (SystemMng.ins.state == SystemMng.STATE.PAUSE) {
            Stop();
            return;
        }

        if (!isDead) {
            Detect();
        }
        hitDelay += Time.deltaTime;

    }
    public void SetDestination() {
        agent.SetDestination(Player.ins.transform.position);
    }
    public void Stop() {
        agent.ResetPath();
    }
    public void Detect() {
        Vector3 d = Player.ins.transform.position;
        Vector3 s = transform.position;
        if (Vector3.Distance(s, d) < 8) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(d - s), Time.deltaTime * 10);

        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Weapon") {//무기와 닿았을 때
            if (hitDelay > 1.0f) {// 맞고 1초간 다시 안맞게
                hitDelay = 0f;
                Vector3 knockbackDir = transform.position - other.transform.position;
                ps.transform.rotation = Quaternion.LookRotation(other.transform.position);
                ps.Play();
                rb.AddForce(knockbackDir.normalized * knockbackPower + new Vector3(0, 1, 0) * knockbackPower, ForceMode.Impulse);
                HP -= 20;
                HpBar.fillAmount = HP / MaxHp;
                if (HP <= 0) {
                    isDead = true;
                    StartCoroutine(die());
                    SystemMng.ins.mobCnt--;
                } else {
                    StartCoroutine(hit());
                }
                print("Hit! HP : " + HP);
            }
        }
    }

    IEnumerator hit() {
        for(int i = 0; i < 5; i++) {
            RD.material.SetFloat("_Float1", 0.5f);
            yield return new WaitForSeconds(0.1f);
            RD.material.SetFloat("_Float1", 0f);
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator die() {
        agent.enabled = false;
        float alpha = 2.0f;
        while (alpha >= 0) {
            RD.material.SetFloat("_Alpha",alpha);
            yield return new WaitForSeconds(Time.deltaTime);
            alpha -= Time.deltaTime;
        }
        gameObject.SetActive(false);
        
    }
}
