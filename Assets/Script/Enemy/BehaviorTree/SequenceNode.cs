using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//자식 노드가 Failure 발생 시 return
//단 Running 발생 시 다음 프레임에도 다시 그 자식을 평가하기 위해 return
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
