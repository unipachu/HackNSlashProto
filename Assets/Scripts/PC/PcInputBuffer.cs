using UnityEngine;

// TODO: This is not type safe. 
public class PcInputBuffer : MonoBehaviour {
    [SerializeField] Pc pc;

    BufferableInput bufferedInput;
    float remainingTime;

    // Update is called once per frame
    void Update(){
        if (remainingTime <= 0)
            return;
        remainingTime -= Time.deltaTime;
        //Debug.Log("remaining time: " + remainingTime);
        if (remainingTime <= 0)
            Clear();
    }

    public void BufferInput(BufferableInput input) {
        bufferedInput = input;
        remainingTime = pc.Data.inputBufferDur;
    }

    public void Clear(){
        bufferedInput = BufferableInput.None;
        remainingTime = 0;
    }

    /// <returns>
    /// True if action was in the input buffer and was consumed.
    /// </returns>
    public bool TryConsumeInput(BufferableInput input){
        if(HasInput(input)){
            Clear();
            return true;
        }
        return false;
    }

    public bool HasInput(BufferableInput input)
        => input == bufferedInput;
}
