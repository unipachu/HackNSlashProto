using UnityEngine;

public class FPSTest : MonoBehaviour
{
    private int frames = 0;
    private float timer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Elden Ring worst parry window was 5 frames (in locked 60). That would be around 0.083 seconds.
        // TODO CONTD: Test how easy it is to push a button at the right time when you blink an object for 0.083 seconds every second.
        // TODO CONTD: Then check how much you need to slow down the game to make it super easy to hit at the right time.

        frames++;
        timer += Time.deltaTime;
        if(timer > 1)
        {
            timer = 1 - timer;
            //Debug.Log("Frames per second: " + frames);
            frames = 0;
        }
    }
}
