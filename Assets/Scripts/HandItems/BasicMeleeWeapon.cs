using System;
using UnityEngine;

public class BasicMeleeWeapon : MonoBehaviour, IComboUser {
    ComboMoveTree comboTree;

    public IComboNode LShldrComboStart => throw new NotImplementedException();

    public IComboNode RShldrComboStart => throw new NotImplementedException();

    public IComboNode RTrgComboStart => throw new NotImplementedException();

    public Transform Trf => throw new NotImplementedException();
}
