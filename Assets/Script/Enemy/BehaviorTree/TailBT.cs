using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailBT : BossBT {

    [SerializeField]
    TailPattern VellumTail;

    new void Awake() => base.Awake();
    new void Update() => base.Update();

    protected override INode SetBT() {
        return new SelectNode(
            new List<INode> {
                new ActionNode(IsAttacking),
                new ActionNode(Pattern1)
            }
        );
    }

    protected override INode.ENodeState IsAttacking() {
        if (VellumTail.isAttacking)
            return INode.ENodeState.Running;
        else
            return INode.ENodeState.Failure;
    }
    protected override INode.ENodeState Pattern1() {
        VellumTail.StartCoroutine(VellumTail.Pattern1());
        return INode.ENodeState.Success;
    }
}
