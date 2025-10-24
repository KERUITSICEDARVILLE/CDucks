using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryAnimation : MonoBehaviour
{

// make generation from same side of map as rows grow

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
    private float increment = 0.01f;
    private const int FRAME_MAX = 83;
    private const int TRACK_MAX = 10;

    public float[,] tracks = new float[TRACK_MAX, 2] {
    {-2.09f, 2.54f},
    {-2.51f, 2.22f},
    {-2.79f, 1.9f},
    {-3.12f, 1.58f},
    {-3.45f, 1.26f},
    {-3.81f, 0.95f},
    {-3.77f, 0.63f},
    {-2.03f, 0.32f},
    {-1.23f, 0f},
    {-0.78f, -0.37f}};

    private int[] track_count = new int[TRACK_MAX] {
    3, 5, 8, 9, 11, 12, 13, 9, 7, 6
    };

    private int[] track_iterate = new int[TRACK_MAX] {
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    // Start is called before the first frame update
    void Start()
    {
        Frames = new GameObject[FRAME_MAX];
        for (int i = 0; i < FRAME_MAX; i++) {
        Frames[i] = Instantiate(OriginFrame, transform);
        Frames[i].transform.localPosition += new Vector3(Random.Range(0.02f, 0.07f), Random.Range(0.02f, 0.07f), 0f);
        Frames[i].transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 180f));
        }
    brunner = increment;
    runner = 0f;
    currCoord = Frames[0].transform.localPosition;
    nextCoord = new Vector3(tracks[0,0], tracks[0,1], 0f);
    }

    // Update is called once per frame
    void Update()
    {
    int which = (int)Random.Range(0f, (float)TRACK_MAX - 0.001f);
        while ( !(track_iterate[which] < track_count[which]) ) {
        which = (int)Random.Range(0f, (float)TRACK_MAX - 0.001f);
        }

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
            Frames[fIndex].transform.eulerAngles /= 1.01f;
            Frames[fIndex].transform.localPosition = (1 - runner) * currCoord + runner * nextCoord;
            runner += increment;
        } else if (runner > 1f) {
            Frames[fIndex].transform.eulerAngles = new Vector3(0f, 0f, 0f);
            fIndex++;
                if (fIndex == FRAME_MAX) {
                runner = 0f;
                return;
                }
            currCoord = Frames[fIndex].transform.localPosition;
            nextCoord = new Vector3(tracks[which, 0] + (float)track_iterate[which] * 0.35f, tracks[which, 1], 0f);
            track_iterate[which]++;
            runner = increment;
        }



    }
}
