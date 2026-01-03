using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All states used by an custom animator. <br/>
/// NOTE: These states need to correspond to those used by the animator of the CustomAnimator.
/// </summary>
public class CustomAnimatorStates: MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private List<CustomAnimatorStateInfo> _animatorStates = new List<CustomAnimatorStateInfo>();

    private void Start()
    {
        ValidateAnimationStates();
    }

    private void ValidateAnimationStates()
    {
        HashSet<int> usedHashes = new(_animatorStates.Count);

        for (int i = 0; i <= 10; i++)
        {
            string stateName = _animatorStates[i].ThisAnimationName;

            if (string.IsNullOrWhiteSpace(stateName))
            {
                Debug.LogError("Animator state name is empty.", this);
                continue;
            }

            int stateHash = Animator.StringToHash(stateName);

            // NOTE: Only checks layer 0. You might want to loop through all layers.
            Debug.Assert(_animator.HasState(0, stateHash), "Animator didn't have state: " + stateName, this);
            
            if(!usedHashes.Add(stateHash))
            {
                Debug.LogError("State with the same hash already in the dictionary!", this);
            }
        }
    }
}
