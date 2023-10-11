using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VellumAI : BossAI {

    [SerializeField]
    VellumBT bossVellum;

    new void Awake() => base.Awake();
    new void Update() => base.Update();

    protected override INode SetBT() {
        return new SelectNode(
            new List<INode>() {
                new SequenceNode(
                    new List<INode> {
                        new ActionNode(IsDead),
                        new ActionNode(ActionDie)
                    }
                    ),
                new SelectNode(
                    new List<INode> {
                        new ActionNode(IsAttacking),
                        new RandomSelectNode(
                            new List<INode> {
                                new ActionNode(Pattern1),
                                new ActionNode(Pattern2)
                            }
                            )
                    }
                    )
            }
            );
    }

    INode.ENodeState IsDead() {
        if (bossVellum.HP <= 0)
            return INode.ENodeState.Success;
        else
            return INode.ENodeState.Failure;
    }

    INode.ENodeState ActionDie() {
        print("º§·ë Á×À½");
        SystemMng.ins.t.gameObject.SetActive(false);
        bossVellum.StopAllCoroutines();
        Vector3 VellumPos = bossVellum.transform.position;
        bossVellum.transform.position = new Vector3(VellumPos.x, 0, VellumPos.z);
        bossVellum.StartCoroutine(bossVellum.die());
        return INode.ENodeState.Success;
    }
    protected override INode.ENodeState IsAttacking() {
        if (bossVellum.isAttacking)
            return INode.ENodeState.Running;
        else
            return INode.ENodeState.Failure;
    }
    protected override INode.ENodeState Pattern1() {
        bossVellum.StartCoroutine(bossVellum.Pattern1());
        return INode.ENodeState.Success;
    }
    INode.ENodeState Pattern2() {
        bossVellum.StartCoroutine(bossVellum.Pattern2());
        return INode.ENodeState.Success;
    }
}
