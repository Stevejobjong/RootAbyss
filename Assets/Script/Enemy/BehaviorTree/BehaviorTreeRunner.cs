using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeRunner {
    INode rootNode;
    public BehaviorTreeRunner(INode rootNode) {
        this.rootNode = rootNode;
    }

    public void Run() {
        rootNode.Evaluate();
    }

}
