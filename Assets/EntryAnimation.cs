using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryAnimation : MonoBehaviour
{

    public PointAndDuck script;
    public float animationKey = 0f;
    public GameObject OriginFrame;
    private GameObject[] Frames;
    private Vector3 nextCoord;
    private Vector3 currCoord;
    private float runner;
    private int fIndex = 0;

    public float[,] tracks = new float[5, 3] { /* x_min, x_max, y */
    {-6.52f, -4.01f, 4.44f},
    {-7.72f, -2.44f, 3.73f},
    {-8.92f, -0.23f, 2.98f},
    {-10.01f, 0.6f, 2.23f},
    {-11.06f, 1.34f, 1.41f}};

    // Start is called before the first frame update
    void Start()
    {
        Frames = new GameObject[20];
        for (int i = 0; i < 20; i++) {
        Frames[i] = Instantiate(OriginFrame, transform);
        Frames[i].transform.position += new Vector3(Random.Range(0.02f, 0.07f), Random.Range(0.02f, 0.07f), 0f);
        Frames[i].transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 45f));
        Frames[i].GetComponent<SpriteRenderer>().enabled = true;
        }
    runner = 0.01f;
    currCoord = Frames[0].transform.position;
    nextCoord = new Vector3(tracks[0,0], tracks[0,2], 0f);
    }

    // Update is called once per frame
    void Update()
    {

        if (runner > 0.0 && runner < 1f) {
        Frames[fIndex].transform.eulerAngles /= 1.1f;
        Frames[fIndex].transform.position = (1 - runner) * currCoord + runner * nextCoord;
        runner += 0.01f;
        } else if (runner > 1f) {
        fIndex = fIndex < 19 ? fIndex + 1 : 18;
        currCoord = Frames[fIndex].transform.position;
        nextCoord = new Vector3(tracks[(int)Random.Range(0f, 5f), 0], tracks[(int)Random.Range(0f, 5f), 2], 0f);
        runner = 0.1f;
        }

    }
}
