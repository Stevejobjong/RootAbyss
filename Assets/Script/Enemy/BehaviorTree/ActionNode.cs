using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionNode : INode {
    //Func 제네릭 대리자
    Func<INode.ENodeState> _Function = null;
    public ActionNode(Func<INode.ENodeState> Funtion) { 
        _Function = Funtion;
    }

    //식 본문 멤버, ?. ?? 연산자
    public INode.ENodeState Evaluate() => _Function?.Invoke() ?? INode.ENodeState.Failure;
}
