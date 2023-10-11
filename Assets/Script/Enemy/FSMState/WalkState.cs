using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : StateMachineBehaviour {
    Enemy enemy;
    float CoolTime = 0f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        enemy = animator.GetComponent<Enemy>();
        enemy.SetDestination();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy.isDead)
            return;
        CoolTime += Time.deltaTime;
        if (Vector3.Distance(animator.transform.position, Player.ins.transform.position) < 1) {
            enemy.Stop();
            animator.SetTrigger("Attack");
        }

        if (CoolTime > 3.0f) {
            CoolTime = 0.0f;
            animator.SetTrigger("Idle");
        }
    }

}
