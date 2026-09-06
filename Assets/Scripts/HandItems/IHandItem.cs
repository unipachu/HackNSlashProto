using System;
using UnityEngine;

public interface IHandItem {
    Transform Trf { get; }
}


// TODO: Delete
[Obsolete("items no longer use databases")]
public interface IHandItemDatabase{
    //List<HandItemData> HandItemData { get; }
}
