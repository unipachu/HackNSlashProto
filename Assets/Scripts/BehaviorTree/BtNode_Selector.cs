using UnityEngine;

public class BtNode_Selector : BtNode {
    BtNode[] children;
    int curChild;
    string dbgName;

    public string DbgName => dbgName;

    public BtNode_Selector(string dbgName, params BtNode[] children) {
        this.dbgName = dbgName;
        this.children = children;
    }

    public BtResult Eval() {
        while(curChild < children.Length) {
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
