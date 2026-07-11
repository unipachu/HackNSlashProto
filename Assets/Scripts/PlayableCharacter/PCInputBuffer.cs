using UnityEngine;

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

    public bool ConsumeInput(string actionName)
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
