using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNode : INode {
    List<INode> childrens;

    public SelectNode(List<INode> childrens) {
        this.childrens = childrens;
    }

    public virtual INode.ENodeState Evaluate() {
        if (childrens == null)
            return INode.ENodeState.Failure;

        foreach (var child in childrens) {
            switch(child.Evaluate()){
                case INode.ENodeState.Running:
                    return INode.ENodeState.Running;
                case INode.ENodeState.Success:
                    return INode.ENodeState.Success;
                case INode.ENodeState.Failure:
                    continue;
            }
        }
        return INode.ENodeState.Failure;
    }
}
