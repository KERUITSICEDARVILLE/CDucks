using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlightFrame : MonoBehaviour
{
private float t = 0f;
private float increment = 0.005f;
private Vector3 nextCoord;
private Vector3 currCoord;
private int which;

public EntryAnimation parentScript;

    // Start is called before the first frame update
    void Start()
    {
        which = (int)transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<SpriteRenderer>().enabled && t < increment) {
        nextCoord = parentScript.giveCoord(which);
        currCoord = transform.localPosition;
        t = increment;
        }

        if (t > 0f && t < 1f) {
        t += increment;
        transform.eulerAngles /= 1.01f;
        transform.localPosition = (1 - t) * currCoord + t * nextCoord;
        }

        if (t > 1f && t < 1f + increment) {
        t += increment;
        Debug.Log(which);
        Debug.Log("Done once");
        parentScript.doneSignal(which);
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }
}
