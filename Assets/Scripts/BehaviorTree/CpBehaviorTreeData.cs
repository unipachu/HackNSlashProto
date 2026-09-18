/// <summary>
/// Can construct all the possible behavior trees NPCs can use.
/// </summary>
// TODO: Rename to BtData
public static class CpBehaviorTreeData{
    public static BtNode Get(BtT t, CpRegisterer cp, AiCtrl aiCtrl) {
        return t switch {
            BtT.FollowNAttack => GetBt_FollowNAttack(cp, aiCtrl),
            _ => GeneralUtils.LogErrorForInput<BtT, BtNode>(t)
        };
    }

    static BtNode GetBt_FollowNAttack(CpRegisterer cp, AiCtrl aiCtrl) {
        return new BtNode_Selector(
            "RootSelector",
            new BtNode_Sequence(
                "LockedOnTgtSequence",
                new BtNode_TryFindLockOnTgt(cp),
                new BtNode_Selector(
                    "TargetActionSelector",
                    new BtNode_Sequence(
                        "AtkSequence",
                        new BtNode_Cond_InAtkRange(cp),
                        new BtNode_Cmd_Atk1(cp, aiCtrl)
                    ),
                    new BtNode_Sequence(
                        "FollowSequence",
                        new BtNode_InAggroRange(cp),
                        new BtNode_MovToTgt(cp, aiCtrl)
                    )
                )
            ),
            new BtNode_Cmd_Idle(cp, aiCtrl)
        );
    }
}
