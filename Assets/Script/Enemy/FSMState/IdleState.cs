using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateMachineBehaviour {

    float CoolTime = 0f;
    Enemy enemy;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        enemy=animator.GetComponent<Enemy>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy.isDead)
            return;
        CoolTime += Time.deltaTime;
        if (Vector3.Distance(animator.transform.position, Player.ins.transform.position) < 1 && CoolTime > 1.0f && !enemy.isDead) {
            CoolTime = 0f;
            enemy.Stop();
            animator.SetTrigger("Attack");
        }
        if (Vector3.Distance(animator.transform.position, Player.ins.transform.position) < 8 && CoolTime > 1.0f) {
            CoolTime = 0f;
            animator.SetTrigger("Walk");
        }
    }
}
