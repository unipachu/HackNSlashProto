using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates combo graphs that are made from <see cref="IComboNode_CpHumanoid"/>.
/// </summary>
public static class ComboGraphFactory {
    /// <summary>
    /// NOTE: When you create new combo node classes, always add a corresponding enum and a case in this switch.
    /// </summary>
    /// <param name="ctx">
    /// Generally the hand item that uses this combo graph, but can be any object containing the data the combo
    /// node needs for initialization. Individual nodes cast this to get the initialization data they need, so
    /// check the individual nodes for data required from <paramref name="ctx"/>.
    /// </param>
    /// <returns>Ref to generated combo graph.</returns>
    public static List<IComboNode_CpHumanoid> GenerateComboGraph(Object ctx, ComboGraphT comboGraphT)
        => comboGraphT switch {
            ComboGraphT.LaserAimNShoot => LaserAimNShoot(ctx),
            ComboGraphT.Melee3HorHit => Melee3HorHit(ctx),
            ComboGraphT.MeleeSingleHorHit => MeleeSingleHorHit(ctx),
            ComboGraphT.MeleeSingleVerHit => MeleeSingleVerHit(ctx),
            ComboGraphT.ShootProj => ShootProj(ctx),
            _ => GeneralUtils.LogErrorForInput<ComboGraphT, List<IComboNode_CpHumanoid>>(comboGraphT)
        };

    static List<IComboNode_CpHumanoid> LaserAimNShoot(Object ctx) {
        var i0 = new ComboNode_RotateToLastNonZeroInput(CpHumanoidAnimInfoT.atk_GunShoot_Windup);
        var i1 = new ComboNode_LaserAimNShootProj(CpHumanoidAnimInfoT.atk_GunShoot_AimPose, ctx);
        var i2 = new ComboNode_BasicRecovery(CpHumanoidAnimInfoT.atk_GunShoot_Recovery);
        i0.Transitions = new ComboNode_Transitions(null, null, i1, null, null);
        i1.Transitions = new ComboNode_Transitions(null, null, i2, null, null);
        List<IComboNode_CpHumanoid> nodes = new List<IComboNode_CpHumanoid> { i0, i1, i2 };
        return nodes;
    }

    static List<IComboNode_CpHumanoid> Melee3HorHit(Object ctx) {
        var i0 = new ComboNode_RotateToWhenLastSwitchedStInputDir(CpHumanoidAnimInfoT.atk_HorSlash0_Windup);
        var i1 = new ComboNode_BasicImpact(CpHumanoidAnimInfoT.atk_HorSlash0_Impact, ctx);
        var i2 = new ComboNode_BasicRecovery(CpHumanoidAnimInfoT.atk_HorSlash0_Recovery);
        var i3 = new ComboNode_BasicImpact(CpHumanoidAnimInfoT.atk_HorSlash1_Impact, ctx);
        var i4 = new ComboNode_BasicRecovery(CpHumanoidAnimInfoT.atk_HorSlash1_Recovery);
        var i5 = new ComboNode_BasicImpact(CpHumanoidAnimInfoT.atk_HorSlash2_Impact, ctx);
        i0.Transitions = new ComboNode_Transitions(null, null, i1, null, null);
        i1.Transitions = new ComboNode_Transitions(null, null, i2, i3, null);
        i3.Transitions = new ComboNode_Transitions(null, null, i4, i5, null);
        i5.Transitions = new ComboNode_Transitions(null, null, i2, null, null);
        return new List<IComboNode_CpHumanoid> {i0, i1, i2, i3, i4, i5};
    }

    // TODO MINOR: Combine single hit combo generators into one method.
    static List<IComboNode_CpHumanoid> MeleeSingleHorHit(Object ctx) {
        var i0 = new ComboNode_RotateToWhenLastSwitchedStInputDir(CpHumanoidAnimInfoT.atk_HorSlash0_Windup);
        var i1 = new ComboNode_BasicImpact(CpHumanoidAnimInfoT.atk_HorSlash0_Impact, ctx);
        var i2 = new ComboNode_BasicRecovery(CpHumanoidAnimInfoT.atk_HorSlash0_Recovery);
        i0.Transitions = new ComboNode_Transitions(null, null, i1, null, null);
        i1.Transitions = new ComboNode_Transitions(null, null, i2, null, null);
        return new List<IComboNode_CpHumanoid> {i0, i1, i2 };
    }

    static List<IComboNode_CpHumanoid> MeleeSingleVerHit(Object ctx) {
        var i0 = new ComboNode_RotateToWhenLastSwitchedStInputDir(CpHumanoidAnimInfoT.atk_VerSlash0_Windup);
        var i1 = new ComboNode_BasicImpact(CpHumanoidAnimInfoT.atk_VerSlash0_Impact, ctx);
        var i2 = new ComboNode_BasicRecovery(CpHumanoidAnimInfoT.atk_VerSlash0_Recovery);
        i0.Transitions = new ComboNode_Transitions(null, null, i1, null, null);
        i1.Transitions = new ComboNode_Transitions(null, null, i2, null, null);
        return new List<IComboNode_CpHumanoid> {i0, i1, i2 };
    }

    static List<IComboNode_CpHumanoid> ShootProj(Object ctx) {
        var i0 = new ComboNode_ShootProj(CpHumanoidAnimInfoT.atk_GunShoot_Windup, ctx);
        var i1 = new ComboNode_BasicRecovery(CpHumanoidAnimInfoT.atk_GunShoot_Recovery);
        i0.Transitions = new ComboNode_Transitions(null, null, i1, null, null);
        return new List<IComboNode_CpHumanoid> {i0, i1};
    }
}
