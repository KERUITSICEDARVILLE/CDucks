using UnityEngine;
using System.Collections.Generic;

public class BlightMutation : MonoBehaviour
{

    public WorldGrid World;
    public BlightController Controller;
    public WorldTile TargetTile;
    public List<WorldTile> Path;

    public float moveTimeMax;
    private float moveTimer;

    public Vector2Int cell {
        get { return transform.parent.GetComponent<WorldTile>().tileCoord; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        World = FindAnyObjectByType<WorldGrid>().GetComponent<WorldGrid>();
        Controller = FindAnyObjectByType<BlightController>().GetComponent<BlightController>();
        Controller.GiveMeTarget(this);
        moveTimer = moveTimeMax;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        transform.localPosition = new Vector3(0f, 0f, -2.820513f);
        if (TargetTile == null || Path == null) {
            return;
        }
        GameObject loadTarget = World.GetObjectAtCell<BasicBlight>(TargetTile.tileCoord);
        if (loadTarget == null) {
            Controller.RetargetNear(this, TargetTile);
        }
        GameObject CellBlight = World.GetObjectAtCell<BasicBlight>(cell);
        if (CellBlight != null) {
            CellBlight.GetComponent<BasicBlight>().enabled = true;
        }
        if (moveTimer > 0f && Path.Count > 0) {
            moveTimer -= Time.deltaTime;
        } else {
            moveTimer = moveTimeMax;
            if (Path.Count == 0) {
                return;
            }
            WorldTile next = Path[0];
            transform.SetParent(next.gameObject.transform);
            Path.RemoveAt(0);
        }

        // if near duck, move faster

    }
}
