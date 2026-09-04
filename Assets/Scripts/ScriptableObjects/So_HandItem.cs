using UnityEngine;

// TODO: Make this a interface instead of a superclass. Except wait... then you cannot add these to one
// TODO C: catalogue since Serialize Fields do not support interfaces. Unity and C#, what are you doing?
// TODO: To maybe allow for serialized interface classes and easier modification of list items, check this:
// https://youtu.be/6qd22ulEds4?si=n2eiBVmvLCBQ9ckE
// and this:
// https://discussions.unity.com/t/favourite-way-to-serialize-interfaces/689351/2
// ACTUALLY you could probably use this (since scriptable object is an object):
// https://youtu.be/xcGPr04Mgm4?si=u8P9E-d_yADHcY8v
public abstract class So_HandItem : ScriptableObject {
    [SerializeField] string id;

    public string Id => id;

    public abstract IHandItemHandle InstantiateNRegister();
}
