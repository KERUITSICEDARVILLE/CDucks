using UnityEngine;

using System.Collections.Generic;

public class Him : MonoBehaviour
{
    private bool once;

    public WorldGrid World;
    public BlightController Controller;
    private WorldTile TargetTile;
    private List<WorldTile> Path;
    public Vector2Int[] mapPath;

    public int pathIndex;
    public float moveTimeMax;
    private float moveTimer;

    public Vector2Int cell {
        get { return transform.parent.GetComponent<WorldTile>().tileCoord; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!once) { // we cannot have Awake here due to how Instantiate() does things
            once = true;
        } else {
            return;
        }

        transform.localScale = new Vector3(0.2198616f, 0.2198616f, 0.2198616f);
        World = FindAnyObjectByType<WorldGrid>().GetComponent<WorldGrid>();
        Controller = FindAnyObjectByType<BlightController>().GetComponent<BlightController>();
        moveTimer = moveTimeMax;
        pathIndex = 0;
        NextLoca();

    }

    // Update is called once per frame
    void Update()
    {
        if (!Controller.enabled) {
            return; 
        }
        // if GameController.regionIndex != -1 then show HUD alert

        if (TargetTile == null || Path == null) {
            return;
        }
        GameObject loadTarget = World.GetObjectAtCell<MonoBehaviour>(TargetTile.tileCoord);
        if (loadTarget != null || transform.parent == World.GetTile(mapPath[pathIndex]).gameObject) {
            NextLoca();
        }
        WorldTile[] enhancees = World.GetAdjacentTileRangeWithType<BasicBlight>(cell, Random.Range(5, 7));
        BasicBlight objBlight;

        foreach (WorldTile enhancee in enhancees) {
            objBlight = World.GetObjectAtCell<BasicBlight>(enhancee.tileCoord).GetComponent<BasicBlight>();
            objBlight.GrowthRate += Random.Range(3.0f, 4.0f) * Time.deltaTime;
            objBlight.MaxGrowth += Random.Range(4.0f, 5.0f) * Time.deltaTime;
            objBlight.gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f / objBlight.GrowthRate, 1f / objBlight.GrowthRate, 1f);
            objBlight.Lineage = Controller.GiveMeUniqueID();
        }

        if (moveTimer > 0f && Path.Count > 0) {
            moveTimer -= Time.deltaTime;
        } else {
            moveTimer = moveTimeMax;
            if (Path.Count == 0) {
                return;
            }
            WorldTile next = Path[0];
            if (World.GetObjectAtCell<MonoBehaviour>(Path[0].tileCoord)) {
                NextLoca();
                return;
            }
            transform.SetParent(next.gameObject.transform);
            transform.localPosition = new Vector3(0f, 0f, -2.820513f);
            Path.RemoveAt(0);
        }

        // if near duck, move faster
    }

    public void NextLoca() {
        Debug.Log("went to next");
        if (pathIndex + 1 == mapPath.Length) {
            Debug.Log("destroyed 0");
            Destroy(gameObject);
            return;
        }
        transform.SetParent(World.GetTile(mapPath[pathIndex]).gameObject.transform);
        transform.localPosition = new Vector3(0f, 0f, -2.820513f);
        WorldTile endpt = World.BFSstopstart<BasicBlight>(World.GetTile(mapPath[pathIndex + 1]),
                                                            World.GetTile(mapPath[pathIndex]), true, 0);
        Debug.Log(endpt);
        List<WorldTile> path = World.Gather(endpt);
        World.ResetDiscoveryChannels();
        if (path == null) {
            Debug.Log("destroyed 1 " + pathIndex);
            Destroy(gameObject);
            return;
        }
        path.Reverse();
        path.Add(World.GetTile(mapPath[pathIndex]));
        Path = path;
        TargetTile = endpt;
        pathIndex++;
    }
}
