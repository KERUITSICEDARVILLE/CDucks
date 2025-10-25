using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlightFrame : MonoBehaviour
{
private float t = 0f;
private float increment = 0.05f;
private Vector3 nextCoord;
private Vector3 currCoord;
private int which;
private float spontaneousGeneration = 0f;

public EntryAnimation parentScript;

    // Start is called before the first frame update
    void Start()
    {
        which = (int)transform.position.z;
    }

    void OnMouseEnter()
    {
        // take distance from cursor and go opposite direction that distance.
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0f, 1f) < 0.01f) {
        spontaneousGeneration = increment;
        // generate child object
        // put it on top of us and set as b2
        }

        if (spontaneousGeneration > 0f && spontaneousGeneration < 1f) {
        spontaneousGeneration += increment;
        // switch rapidly between b1, b2
        }

        if (spontaneousGeneration > 1f && spontaneousGeneration < 2f) {
        // fully formed algea bloom (bad!)
        }

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
        parentScript.doneSignal(which);
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }
}
