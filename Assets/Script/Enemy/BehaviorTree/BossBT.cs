using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossBT : MonoBehaviour {
    BehaviorTreeRunner BTRunner;

    protected void Awake() {
        BTRunner = new BehaviorTreeRunner(SetBT());
    }

    protected void Update() {
        BTRunner.Run();
    }

    protected abstract INode SetBT();
    protected abstract INode.ENodeState IsAttacking();
    protected abstract INode.ENodeState Pattern1();
}
