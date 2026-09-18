using System.Collections.Generic;
using UnityEngine;

public class BtNode_Selector : BtNode {
    List<BtNode> children;
    int curChild;
    string dbgName;

    public string DbgName => dbgName;

    public BtNode_Selector(string dbgName, List<BtNode> children) {
        this.dbgName = dbgName;
        this.children = children;
    }

    public BtResult Eval() {
        while(curChild < children.Count) {
            switch (children[curChild].Eval()) {
                case BtResult.Success:
                    curChild = 0;
                    return BtResult.Success;
                case BtResult.Failure:
                    curChild++;
                    break;
                case BtResult.Running:
                    return BtResult.Running;
                default:
                    Debug.LogError("Switch defaulted");
                    break;
            }
        }
        curChild = 0;
        return BtResult.Failure;
    }

    public void Reset() {
        curChild = 0;
        foreach(BtNode node in children)
            node.Reset();
    }
}
