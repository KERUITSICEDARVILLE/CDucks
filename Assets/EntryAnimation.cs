using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryAnimation : MonoBehaviour
{

    public PointAndDuck script;
    public float animationKey = 0f;
    public GameObject OriginFrame;
    public GameObject Blob;
    private GameObject[] Frames;
    private Vector3 nextCoord;
    private Vector3 currCoord;
    private float runner;
    private float brunner;
    private int fIndex = 0;
    private float increment = 0.005f;

    public float[,] tracks = new float[7, 3] { /* x_min, x_max, y */
    {-2.09f, -4.01f, 2.54f},
    {-2.51f, -2.44f, 2.22f},
    {-2.79f, -0.23f, 1.9f},
    {-3.12f, 0.6f, 1.58f},
    {-3.45f, 1.34f, 1.26f},
    {-3.81f, 1.34f, 0.95f},
    {-3.72f, 0f, 0.66f}};

    // Start is called before the first frame update
    void Start()
    {
        Frames = new GameObject[20];
        for (int i = 0; i < 20; i++) {
        Frames[i] = Instantiate(OriginFrame, transform);
        Frames[i].transform.localPosition += new Vector3(Random.Range(0.02f, 0.07f), Random.Range(0.02f, 0.07f), 0f);
        //Frames[i].transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 180f));
        }
    brunner = increment;
    runner = 0f;
    currCoord = Frames[0].transform.localPosition;
    nextCoord = new Vector3(tracks[0,0], tracks[0,2], 0f);
    }

    // Update is called once per frame
    void Update()
    {
    int which = (int)Random.Range(0f, 6.99f);

        if (brunner < 1f && brunner > 0f) {
        brunner += increment;
        Blob.transform.localScale = brunner * (new Vector3(1f, 1f, 1f)) / 2f;
        }

        if (brunner > 1f && brunner < 2f) {
        brunner += increment;
        }

        if (brunner < 1f + 2 * increment && brunner > 1f) {
        brunner += increment;
        runner = increment;   // start frame whipping animation
        }

        if (runner < increment && brunner > 1f) {
        brunner += increment;
        }

        if (brunner > 3f) {
        brunner += increment;
        Blob.transform.localScale = (4 - brunner) * (new Vector3(1f, 1f, 1f)) / 2f;
        }

        if (brunner > 4f) {
        brunner = 0f;
        }

        if (runner > 0.0 && runner < 1f) {
            Frames[fIndex].GetComponent<SpriteRenderer>().enabled = true;
            Frames[fIndex].transform.eulerAngles /= 1.01f; // INTERPOLATION vvvvvv (!!!!!)
            Frames[fIndex].transform.localPosition = (1 - runner) * currCoord + runner * nextCoord;
            runner += increment;
        } else if (runner > 1f) {
            Frames[fIndex].transform.eulerAngles = new Vector3(0f, 0f, 0f);
            fIndex++;
                if (fIndex == 20) {
                runner = 0f;
                return;
                }
            currCoord = Frames[fIndex].transform.localPosition;
            nextCoord = new Vector3(tracks[which, 0], tracks[which, 2], 0f);
            runner = increment;
        }



    }
}
