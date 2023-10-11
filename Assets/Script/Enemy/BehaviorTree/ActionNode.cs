using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionNode : INode {
    //Func ���׸� �븮��
    Func<INode.ENodeState> _Function = null;
    public ActionNode(Func<INode.ENodeState> Funtion) { 
        _Function = Funtion;
    }

    //�� ���� ���, ?. ?? ������
    public INode.ENodeState Evaluate() => _Function?.Invoke() ?? INode.ENodeState.Failure;
}
