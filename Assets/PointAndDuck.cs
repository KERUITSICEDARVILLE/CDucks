using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndDuck : MonoBehaviour
{

    public bool gameStart = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStart) {
        Debug.Log("monobehavior triggered over yonder");
        gameStart = false;
        }
    }
}
