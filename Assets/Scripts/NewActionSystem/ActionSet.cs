using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Different actions available to some game object controlled by an ActionController.
/// </summary>
[CreateAssetMenu]
public class ActionSet : ScriptableObject
{
    public List<ActionDefinition> Actions;
}
