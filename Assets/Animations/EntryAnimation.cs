using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryAnimation : MonoBehaviour
{

// make generation from same side of map as rows grow
// make frames ripple. Maybe some visually interesting waves?
// find insertion point for hexagon tilemap
// 

// plans for ducks as of October 24th 2025:
// duck bill will have collider so the tip of the mouse
// --- doesn't govern where we delete blight.
// duck feet will have collider that turns on with click.
// --- in rainbow mode the duck will rapidly ascend and 
// --- descend stomping its feet

// Eccentric animations such as:
// --- A random pair of npc's (cedarcouple) are walking
// --- when all of the sudden a duck descends and picks up
// --- one individual by the shoulders and drags them
// --- offscsreen

    // copy-able game objects (make prefabs)
    public GameObject OriginFrame;
    public GameObject Blob;

    // cursor sprites
    public Texture2D duckHammer;
    public Texture2D duckHammerDown;
    public Texture2D rainbowDuckTex;
    public Texture2D rainbowDuckTexDown;
    public CursorMode cMode = CursorMode.Auto;
    public int whichDucky = 0; // cursor controller
    private Texture2D[,] SpriteSets = new Texture2D[2,2];

    // cursed variables that take up way too much memory
    private const int POINT_MAX = 100;
    public int pMAX = POINT_MAX;
    public Vector2[,] cPoints = new Vector2[TRACK_MAX,POINT_MAX];
    private Vector2[][] curve = new Vector2[TRACK_MAX][];
    private float[] t_rotate = new float[TRACK_MAX];
    // for bezier curve waves

    // Blight Boxes (Frame board tiles)
    private Vector3 nextCoord;
    private Vector3 currCoord;
    private float increment = 0.01f;
    private const int FRAME_MAX = 82;
    private const int TRACK_MAX = 10;
    private const int FRAME_BTC = 5;
    private GameObject[] Frames = new GameObject[FRAME_MAX];
    // and related Frame index to 2d coordinate array plus
    // coordinate to Frame index array (backwards)
    private int[,]  forwardConversion  = new int[FRAME_MAX,2];
    private int[][] backwardConversion = new int[TRACK_MAX][];



    private float[,] track_bases = new float[TRACK_MAX, 2] {
    {-2.09f, 2.54f},
    {-2.51f, 2.22f},
    {-2.79f, 1.9f},
    {-3.12f, 1.58f},
    {-3.45f, 1.26f},
    {-3.81f, 0.95f},
    {-3.77f, 0.63f},
    {-2.03f, 0.32f},
    {-1.23f, -0.03f},
    {-0.78f, -0.37f}};

    private int[] track_maxes = new int[TRACK_MAX] {
    3, 5, 7, 9, 11, 12, 13, 9, 7, 6
    };

    // START FRAME TILE ADDITION UTILITIES
    private bool[] doneFrames = new bool[FRAME_BTC];
    private Vector3[] BatchCoords = new Vector3[FRAME_BTC];
    private int batch_iterator = 0; // we add in batches of FRAME_BTC

    private float[] track_widths = new float[TRACK_MAX];
    private int[] track_iterators = new int[TRACK_MAX];
    private bool[] track_usage = new bool[TRACK_MAX];

    public bool tracksFull() {
    bool ret = true;
        for (int i = 0; i < TRACK_MAX; i++) {
        ret &= track_usage[i];
        }
    return ret;
    }

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
        int startFrame = batch_iterator * FRAME_BTC;
        int which;
        for (int i = 0; i < FRAME_BTC; i++) {
        which = (int)Random.Range(0f, (float)TRACK_MAX - increment);
            while (!tracksFull()) {
                if (track_iterators[which] != track_maxes[which]) {
                forwardConversion[startFrame + i, 0] = which;
                forwardConversion[startFrame + i, 1] = track_iterators[which];
                backwardConversion[which][track_iterators[which]] = startFrame + i;
                break;
                } else {
                track_usage[which] = true;
                }
            which = (int)Random.Range(0f, (float)TRACK_MAX - increment);
            }
        BatchCoords[i] = new Vector3(track_bases[which, 0] + track_iterators[which] * 0.35f, track_bases[which, 1], (float)(i + startFrame) + 0.5f);
        track_iterators[which]++;
        }
    }
    // END FRAME TILE ADDITION UTILITIES

    // Wave animations are due to this (see lines 220-242)
    private void bezierBoil(int order, Vector2[] controls, float t) { // puts result in controls[0]
        for (int i = order - 1; i == 0 ? false : true; i--) {
            for (int j = 0; j < i; j++) {
            controls[j].x = (1 - t) * controls[j].x + t * controls[j + 1].x;
            controls[j].y = (1 - t) * controls[j].y + t * controls[j + 1].y;
            }
        }
    }

    private int intAbs(int x) {
    return x < 0 ? -x : x;
    }

    // BEGIN PUBLIC MAP QUERIES (accessible by frame tiles)
    public int whichTrack(int index) { // sanitize
    return forwardConversion[intAbs(index % FRAME_MAX), 0];
    }

    public int[] myLatticeCoord(int index) {
    index = intAbs(index % FRAME_MAX);
    return new int[2] {forwardConversion[index, 0],
                       forwardConversion[index, 1]};
    }

    public int latticeToFrame(int track, int whichx) {
    track = intAbs(track % TRACK_MAX);
    whichx = intAbs(whichx % track_maxes[track]);
    return backwardConversion[track][whichx];
    }

    public int positionalNeighbors(int index, float radius, int want, GameObject[] neighbors) {
    index = intAbs(index % FRAME_MAX);
    int ret = 0;
    Vector2 queryOrigin = Frames[index].transform.localPosition;
    Vector2 distVec;
        for (int i = 0; i < FRAME_MAX && ret < want; i++) {
        distVec = (Vector2)Frames[i].transform.localPosition - queryOrigin;
            if (distVec.magnitude < radius) {
            neighbors[ret] = Frames[i];
            ret++;
            }
        }
    return ret;
    }
    // END PUBLIC MAP QUERIES

    void Start()
    {
        /*
        SpriteSets[0,0] = duckHammer;
        SpriteSets[0,1] = duckHammerDown;
        SpriteSets[1,0] = rainbowDuckTex;
        SpriteSets[1,1] = rainbowDuckTexDown;

        Cursor.SetCursor(SpriteSets[whichDucky, 0], Vector2.zero, cMode);
        */
        for (int i = 0; i < TRACK_MAX; i++) {
        t_rotate[i] = Mathf.PI; // always start above animation cap
        track_iterators[i] = 0; // haven't added anything onto tracks yet
        track_usage[i] = false; // ---

        // allocations
        curve[i] = new Vector2[3];
        backwardConversion[i] = new int[track_maxes[i]];
        }
 
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

        for (int i = 0; i < TRACK_MAX; i++) {
        track_widths[i] = Frames[backwardConversion[i][track_maxes[i] - 1]].transform.localPosition.x
                           - Frames[backwardConversion[i][0]].transform.localPosition.x;
        }

        if (Input.GetMouseButtonDown(0)) {
        Cursor.SetCursor(SpriteSets[whichDucky, 1], Vector2.zero, cMode);
        }

        if (Input.GetMouseButtonUp(0)) {
        Cursor.SetCursor(SpriteSets[whichDucky, 0], Vector2.zero, cMode);
        }

        if (!Input.GetMouseButtonUp(0) && !Input.GetMouseButtonDown(0)) {
          if (Random.Range(0f, 60f) < 1f) {
          Cursor.SetCursor(SpriteSets[whichDucky, 0], Vector2.zero, cMode);
          }
        }

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

        // Wave animation
        float offset = 5f;
        Vector2 circleVec;
        for (int track = 0; track < TRACK_MAX; track++) {
            circleVec = new Vector2(track_widths[track] / 2f + offset, -40f);
            // this circle origin defines the height of wave

            if (t_rotate[track] > Mathf.PI - Mathf.Atan(Mathf.Abs(circleVec.y / circleVec.x))) {
            t_rotate[track] = Mathf.Atan(Mathf.Abs(circleVec.y / circleVec.x));
            }

            t_rotate[track] += Random.Range(0f, 0.00002f) + 0.00005f * circleVec.magnitude * (Mathf.PI - 2f * Mathf.Atan(Mathf.Abs(circleVec.y / circleVec.x)));

            for (int i = 0; i < POINT_MAX; i++) {
            curve[track][0] = new Vector2(-0.5f, 0f);

            curve[track][1] = new Vector2((circleVec.magnitude) * Mathf.Cos(t_rotate[track]) + circleVec.x - offset,
                                          (circleVec.magnitude) * Mathf.Sin(t_rotate[track]) + circleVec.y);
            curve[track][2] = new Vector2(2f * (circleVec.x - offset) + 0.02f, 0f);
            bezierBoil(3, curve[track], (float)i / (float)POINT_MAX);
            cPoints[track,i] = curve[track][0] + new Vector2(track_bases[track, 0], track_bases[track, 1]);
            }
        }
    }
}
