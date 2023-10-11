using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ڽ� ��尡 Failure �߻� �� return
//�� Running �߻� �� ���� �����ӿ��� �ٽ� �� �ڽ��� ���ϱ� ���� return
public class SequenceNode : INode {
    List<INode> childrens;
    public SequenceNode(List<INode> childrens) {
        this.childrens = childrens;
    }

    public INode.ENodeState Evaluate() {
        if (childrens == null)
            return INode.ENodeState.Failure;

        foreach(var child in childrens) {
            switch (child.Evaluate()) {
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;
                case INode.ENodeState.Failure:
                    return INode.ENodeState.Failure;
                case INode.ENodeState.Success:
                    continue;
            }
        }
        return INode.ENodeState.Success;
    }
}
