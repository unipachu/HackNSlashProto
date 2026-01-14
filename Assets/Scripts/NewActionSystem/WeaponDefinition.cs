using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Weapon")]
public class WeaponDefinition : ScriptableObject
{
    // TODO: Playables might be a better idea?
    /// <summary>
    /// Animations specific to this weapon.
    /// </summary>
    public AnimatorOverrideController AnimatorOverride;
    /// <summary>
    /// What actions can be executed with this weapon?
    /// </summary>
    public ActionDefinition[] AvailableActions;
}
