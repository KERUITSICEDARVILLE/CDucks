using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryAnimation : MonoBehaviour
{

// make generation from same side of map as rows grow
// make frames ripple. Maybe some visually interesting waves?
// find insertion point for hexagon tilemap
// 

    public PointAndDuck script;
    public GameObject OriginFrame;
    public GameObject Blob;
    private GameObject[] Frames;
    private Vector3 nextCoord;
    private Vector3 currCoord;
    private float runner;
    private float brunner;
    private int fIndex = 0;
    private float increment = 0.01f;
    private const int FRAME_MAX = 84;
    private const int TRACK_MAX = 10;
    private const int FRAME_BTC = 5;

    // set forward conversion between Frame index and gridspace
    // set backward conversion

    private int[] prime_cache = new int[10] {
    2, 3, 5, 7, 11, 13, 17, 19, 23, 29
    }; // must be bigger than FRAME_BTC

    private bool[] doneFrames = new bool[FRAME_BTC];

    private int batch_iterator = 0;

    private Vector3[] BatchCoords = new Vector3[FRAME_BTC];

    private float[,] track_bases = new float[TRACK_MAX, 2] {
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

    private int[] track_maxes = new int[TRACK_MAX] {
    13, 5, 8, 9, 11, 12, 13, 9, 7, 6
    };

    private int[] track_iterators = new int[TRACK_MAX] {
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    public Vector3 giveCoord(int index) {
    return BatchCoords[index % FRAME_BTC];
    }

    public void doneSignal(int index) {
    doneFrames[index % FRAME_BTC] = true;
    }

    private bool doneQuery() {
    bool ret = true;
        for (int i = 0; i < FRAME_BTC; i++) {
        ret &= doneFrames[i];
        }
    return ret;
    }

    private void wipeBatch() {
        for (int i = 0; i < FRAME_BTC; i++) {
        doneFrames[i] = false;
        }
    }

    private void genBatchCoords() {
    /*int[] which = new int[FRAME_BTC];
    int prev;
    int inter;
        for (int i = 0; i < FRAME_BTC; i++) {
        which[i] = i;
        }
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < FRAME_BTC; j++) {
                if (Random.Range(0f, 1f) < 0.5f) {
                prev = j - 1 == -1 ? 0 : j - 1;
                inter = which[prev]; 
                which[prev] = which[j];
                which[j]    = inter; // swap
                }
            }
        }
        for (int i = 0; i < FRAME_BTC; i++) {
        BatchCoords[i] = new Vector3(track_bases[which[i], 0], track_bases[which[i], 1], 0f);
        }*/
        int startFrame = batch_iterator * FRAME_BTC;
        int which;
        for (int i = 0; i < FRAME_BTC; i++) {
        which = (int)Random.Range(0f, (float)TRACK_MAX - increment);
            while (track_iterators[which] == track_maxes[which]) {
            which = (int)Random.Range(0f, (float)TRACK_MAX - increment);
            }
        BatchCoords[i] = new Vector3(track_bases[which, 0] + track_iterators[which] * 0.35f, track_bases[which, 1], (float)(i + startFrame) + 0.5f);
        track_iterators[which]++;
        }
    }

    void Start()
    {
        Frames = new GameObject[FRAME_MAX];
        for (int i = 0; i < FRAME_MAX; i++) {
        Frames[i] = Instantiate(OriginFrame, transform);
        Frames[i].transform.localPosition += new Vector3(Random.Range(0.02f, 0.07f), Random.Range(0.02f, 0.07f), (float)i + 0.5f);
        Frames[i].transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 180f));
        }

        for (int i = 0; i < FRAME_BTC; i++) {
        doneFrames[i] = true;
        }

    }

    void Update()
    {
        // wait for last batch to complete.
        if (doneQuery()) {
            wipeBatch();
            genBatchCoords();

            // activate next FRAME_BTC number of Frames
            for (int i = batch_iterator * FRAME_BTC;
                 i < (batch_iterator + 1) * FRAME_BTC && i < FRAME_MAX;
                 i++) {
            Frames[i].GetComponent<SpriteRenderer>().enabled = true;
            }
            batch_iterator++;
        }
    }
}
