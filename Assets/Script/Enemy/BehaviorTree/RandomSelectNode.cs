using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ڽ��� ���� �� SelectNode ����
public class RandomSelectNode : SelectNode {
    List<INode> childrens;

    public RandomSelectNode(List<INode> childrens) : base(childrens) {
        this.childrens = childrens;
    }

    public override INode.ENodeState Evaluate() {
        int k = Random.Range(0, 2);
        if (k==1) {
            INode node = childrens[0];
            childrens[0] = childrens[1];
            childrens[1] = node;
        }
        /*
        //childrens.Count 3�̻��� ���
        for (int i = childrens.Count - 1; i > 0; i--) {
            int k = Random.Range(0, i);
            INode node = childrens[i];
            childrens[i] = childrens[k];
            childrens[k] = node;
        }
        */
        return base.Evaluate();
    }
}
