using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateMachineBehaviour {

    float CheckTime = 0f;
    Enemy enemy;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        enemy=animator.GetComponent<Enemy>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CheckTime += Time.deltaTime;
        if (Vector3.Distance(animator.transform.position, Player.ins.transform.position) < 8 && CheckTime > 1.0f) {
            CheckTime = 0f;
            enemy.SetDestination();
            animator.SetTrigger("Walk");
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }


}
