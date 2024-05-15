using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : StateMachineBehaviour {
    float CoolTime = 0f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("АјАн");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CoolTime += Time.deltaTime;
        if (CoolTime > 1.0f) {
            CoolTime = 0.0f;
            animator.SetTrigger("Idle");
        }
    }
}
