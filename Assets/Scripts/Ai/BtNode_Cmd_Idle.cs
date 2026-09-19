using Unity.Mathematics;

public class BtNode_Cmd_Idle : IBtNode{
    AiCtrlHandle aiCtrl;

    public string DbgName => typeof(BtNode_Cmd_Idle).Name;

    public BtNode_Cmd_Idle(AiCtrlHandle aiCtrl) {
        this.aiCtrl = aiCtrl;
    }

    public BtResult Eval() {
        ref AiCtrlData data = ref AiCtrlMgr.GetData(aiCtrl);
        int cpId = data.cp.Id;
        int aiCtrlId = aiCtrl.Id;
        data.ctrlInputData.input_Mov = float2.zero;
        //Dbg.Log(
        //    $"{cpId} bt node: {typeof(BtNode_Cmd_Idle).Name}",
        //    data.cp,
        //    CpMgr.GetAos(cpId).enableDbgMsgs
        //);
        return BtResult.Success;
    }
}
