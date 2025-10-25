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
private float frameBlightVal = 0f;
private float rainbowDucky = 0f;

public EntryAnimation parentScript;
public Sprite BlightF1, BlightF2;
private Sprite FrameSprite;

    // Start is called before the first frame update
    void Start()
    {
        FrameSprite = GetComponent<SpriteRenderer>().sprite;
        which = (int)transform.position.z;
    }

    void OnMouseEnter()
    {
        if (rainbowDucky > 0f) {
        frameBlightVal = 0f;
        GetComponent<SpriteRenderer>().sprite = FrameSprite;
        transform.localScale = new Vector3(0.154f, 0.154f, 0.154f);
        }
    }

    void OnMouseOver() {
    // take distance from cursor and go opposite direction that distance.
    // move away from cursor
    }

    void OnMouseDown() {
    frameBlightVal = 0f;
    GetComponent<SpriteRenderer>().sprite = FrameSprite;
    transform.localScale = new Vector3(0.154f, 0.154f, 0.154f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0f, 1f) < 0.0001f) {
        rainbowDucky = increment;
        }

        if (rainbowDucky > 0f && rainbowDucky < 500f) {
        rainbowDucky += increment;
        }

        if (rainbowDucky > 500f) {
        rainbowDucky = 0f;
        }

        // basing too many things on frame updates. Lock to timesteps instead
        if (Random.Range(0f, 1f) < 0.0001f && frameBlightVal < increment) {
        frameBlightVal = increment;
        GetComponent<SpriteRenderer>().sprite = BlightF1;
        transform.localScale *= 1f - Random.Range(0f, 0.5f);
        }

        if (frameBlightVal > 0f /*&& frameBlightVal < 1f*/) {
        frameBlightVal += increment;
           if ( (frameBlightVal * 100) % 5 < 2) {
           GetComponent<SpriteRenderer>().sprite = BlightF1;
           } else {
           GetComponent<SpriteRenderer>().sprite = BlightF2;
           }
        }

        if (frameBlightVal > 1f && frameBlightVal < 2f) {
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
