using UnityEngine;

public class theDuckScript : MonoBehaviour
{

    private duckStats theDuckStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    
    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        /*
        //If duckState is 0, then squash and stretch the duck
        int duckState = 0; // Example state variable, ensure this is defined elsewhere in your class

        if (duckState == 0)
        {
            // Squash and stretch the duck
            float squashFactor = Mathf.Sin(Time.time * 5) * 0.2f + 1.0f; // Oscillate scale
            transform.localScale = new Vector3(1.0f, squashFactor, 1.0f / squashFactor);
        }
        //If duckState is 1, then make the duck rainbow colored
        */
    }

}
