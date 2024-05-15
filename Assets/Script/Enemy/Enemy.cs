using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Enemy : MonoBehaviour {
    public Renderer RD;
    NavMeshAgent agent;
    Rigidbody rb;
    Collider cd;
    [SerializeField] ParticleSystem ps;

    [SerializeField] Image HpBar;
    public float HP = 200;
    public float MaxHp = 200;

    public float knockbackPower = 5.0f;
    public bool isDead = false;
    float hitDelay = 0;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        cd= GetComponent<Collider>();
    }

    void Update() {
        if (SystemMng.ins.state == STATE.PAUSE && gameObject.activeSelf) {
            Stop();
            return;
        }

        if (!isDead) {
            Detect();
        }
        hitDelay += Time.deltaTime;

    }

    public void SetDestination()
    {
        if (!isDead)
            agent.SetDestination(Player.ins.transform.position);
    }
    public void Stop() {
        if(!isDead)
            agent.ResetPath();
    }
    public void Detect() {
        Vector3 d = Player.ins.transform.position;
        Vector3 s = transform.position;
        if (Vector3.Distance(s, d) < 8) {       //플레이어와의 거리가 8미만이면 바라보기
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
                int r = Random.Range(0, 4);
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
                    case 3:
                        SoundMng.ins.PlayEffect("body_hit_small_79");
                        break;
                    default:
                        break;
                }

                rb.AddForce(knockbackDir.normalized * knockbackPower + new Vector3(0, 1, 0) * knockbackPower, ForceMode.Impulse);
                int num = Random.Range(0, 3);
                float damage = Player.ins.power;
                switch(num){
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
        if(Player.ins.enemyLocked)
            Player.ins.ResetTarget();
        cd.isTrigger = false;
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
