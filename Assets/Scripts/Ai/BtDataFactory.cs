/// <summary>
/// Can construct all the possible behavior trees NPCs can use.
/// </summary>
public static class BtDataFactory{
    public static IBtNode Construct(BtT t, AiCtrlHandle aiCtrl) {
        return t switch {
            BtT.FollowNAttack => GetBt_FollowNAttack(aiCtrl),
            _ => GeneralUtils.LogErrorForInput<BtT, IBtNode>(t)
        };
    }

    static IBtNode GetBt_FollowNAttack(AiCtrlHandle aiCtrl) {
        return new BtNode_Selector(
            "RootSelector",
            new BtNode_Sequence(
                "LockedOnTgtSequence",
                new BtNode_TryFindFollowTgt(aiCtrl),
                new BtNode_TryLockOnToFollowTgt(aiCtrl),
                new BtNode_Selector(
                    "TargetActionSelector",
                    new BtNode_Sequence(
                        "AtkSequence",
                        new BtNode_Cond_InAtkRange(aiCtrl),
                        new BtNode_Cmd_Atk1(aiCtrl)
                    ),
                    new BtNode_Sequence(
                        "FollowSequence",
                        new BtNode_Cond_InAggroRange(aiCtrl),
                        new BtNode_MovToTgt(aiCtrl)
                    )
                )
            ),
            new BtNode_Cmd_Idle(aiCtrl)
        );
    }
}
