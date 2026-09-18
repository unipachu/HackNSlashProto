using UnityEngine;

public class AiCtrlMgr : Singleton<AiCtrlMgr>{
    [Header("Settings")]
    [Tooltip("Initial capacity of entities")]
    public int initCapacity = 1;

    [HideInInspector] public BtNode[] bt;
    [HideInInspector] public CtrlInputData[] ctrlInputData;
    [HideInInspector] public AiCtrl[] handle;

    int entityCount;

    public void Init() {
        ctrlInputData = new CtrlInputData[initCapacity];
        handle = new AiCtrl[initCapacity];
        bt = new BtNode[initCapacity];

        entityCount = 0;
    }

    /// <summary>
    /// Registers an AI controller and creates its per-entity runtime data.
    /// </summary>
    public void Register(AiCtrl newCtrl, BtNode newBt) {
        Debug.Assert(newCtrl != null);
        int newId = entityCount;
        ArrayUtils.Add(
            ref ctrlInputData,
            newId,
            default
        );
        ArrayUtils.Add(
            ref handle,
            newId,
            newCtrl
        );
        ArrayUtils.Add(
            ref bt,
            newId,
            newBt
        );
        newCtrl.Id = newId;
        entityCount++;
    }

    public void Tick() {
        ResetWasPressedThisFrameInputs();
        TickBehaviorTrees();
    }

    /// <summary>
    /// NOTE: Reset "WasPressedThisFrame" inputs.
    /// </summary>
    void ResetWasPressedThisFrameInputs() {
        for (int i = 0; i < ctrlInputData.Length; i++) {
            ctrlInputData[i].input_Atk_Light = false;
            ctrlInputData[i].input_Atk_Heavy = false;
            ctrlInputData[i].input_Atk_Ult = false;
            ctrlInputData[i].input_Dodge = false;
        }
    }

    void TickBehaviorTrees() {
        for ( int i = 0; i < bt.Length; i++) {
            switch (bt[i].Eval()) {
                case BtResult.Success:
                    bt[i].Reset();
                    break;
                case BtResult.Failure:
                    bt[i].Reset();
                    break;
                case BtResult.Running:
                    Dbg.Log($"Bt {i} running.");
                    break;
                default:
                    Debug.LogError($"Switch defaulted");
                    break;
            }
        }
    }

    /// <summary>
    /// Unregisters an AI controller, removes its runtime data and destroys
    /// the controller GameObject.
    /// </summary>
    public void Unregister(int id) {
        if (id < 0 || id >= entityCount) {
            Debug.LogError(
                $"Invalid AiCtrl id {id}. Ctrl count was {entityCount}."
            );
            return;
        }
        int lastId = entityCount - 1;
        AiCtrl swappedCtrl = id != lastId
            ? handle[lastId]
            : null;
        ArrayUtils.RemoveAtSwapBack(ctrlInputData,entityCount, id);
        ArrayUtils.RemoveAtSwapBack(handle, entityCount, id);
        ArrayUtils.RemoveAtSwapBack(bt, entityCount, id);
        entityCount--;
        if (swappedCtrl != null)
            swappedCtrl.Id = id;
    }
}
