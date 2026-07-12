using UnityEngine;

// TODO: This is not type safe. 
public class PCInputBuffer : MonoBehaviour
{
    [SerializeField] PC pc;

    string bufferedAction;
    float remainingTime;

    // Update is called once per frame
    void Update()
    {
        if (remainingTime <= 0) return;
        remainingTime -= Time.deltaTime;
        //Debug.Log("remaining time: " + remainingTime);
        if (remainingTime <= 0) Clear();
    }

    public void BufferInput(string actionName)
    {
        bufferedAction = actionName;
        remainingTime = pc.baseData.InputBufferDuration;
    }

    public void Clear()
    {
        bufferedAction = null;
        remainingTime = 0;
    }

    /// <returns>
    /// True if action was in the input buffer and was consumed.
    /// </returns>
    public bool TryConsumeInput(string actionName)
    {
        if(HasInput(actionName))
        {
            Clear();
            return true;
        }
        return false;
    }

    public bool HasInput(string actionName)
    {
        return !string.IsNullOrEmpty(actionName) && actionName == bufferedAction;

    }
}
