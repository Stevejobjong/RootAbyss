using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//자식 노드가 Success or Running 발생시 return
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
            }
        }
        return INode.ENodeState.Failure;
    }
}
