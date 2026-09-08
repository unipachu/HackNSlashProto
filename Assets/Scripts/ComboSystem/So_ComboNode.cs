// TODO: Delete
using System;
using UnityEngine;

[Obsolete("Not used anymore")]
public abstract class So_ComboNode : ScriptableObject{
    // TODO: AnimInfo instead
    public string animName;

    /// <summary>
    /// Generates a combo node using any needed data/refs from <paramref name="ctx"/> which represents
    /// the hand item using this combo.
    /// </summary>
    public abstract IComboNode GenerateNode<THandItem>(THandItem ctx) where THandItem : MonoBehaviour, IHandItem;
}
