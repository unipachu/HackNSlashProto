using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BtNode_Sequence : IBtNode{
    IBtNode[] children;
    int curChild;
    string dbgName;

    public string DbgName => dbgName;

    public BtNode_Sequence(string dbgName, params IBtNode[] children) {
        this.dbgName = dbgName;
        this.children = children;
    }

    public BtResult Eval() {
        while (curChild < children.Length) {
            switch (children[curChild].Eval()) {
                case BtResult.Success:
                    curChild++;
                    break;
                case BtResult.Failure:
                    curChild = 0;
                    return BtResult.Failure;
                case BtResult.Running:
                    return BtResult.Running;
                default:
                    Debug.LogError("Switch defaulted");
                    break;
            }
        }
        curChild = 0;
        return BtResult.Success;
    }

    public void Reset() {
        curChild = 0;
        foreach (IBtNode node in children)
            node.Reset();
    }
}
