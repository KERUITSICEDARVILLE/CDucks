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
        transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        World = FindAnyObjectByType<WorldGrid>().GetComponent<WorldGrid>();
        Controller = FindAnyObjectByType<BlightController>().GetComponent<BlightController>();
        Controller.RegisterMutation(gameObject);
        Controller.GiveMeTarget(this);
        moveTimer = moveTimeMax;
    }

    // Update is called once per frame
    void Update()
    {
        // if GameController.regionIndex != -1 then show HUD alert

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
            transform.localPosition = new Vector3(0f, 0f, -2.820513f);
            Path.RemoveAt(0);
        }

        BasicBlight obj = World.GetObjectAtCell<BasicBlight>(cell).GetComponent<BasicBlight>();
        if (obj != null)
        {
            // begin transformation
            obj.GrowthRate += Random.Range(1.0f, 1.8f);
            obj.MaxGrowth += Random.Range(2.0f, 5.0f);
            GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f / obj.GrowthRate, 1f / obj.GrowthRate, 1f);
            obj.Lineage = Controller.GiveMeUniqueID();
            // end transformation
            Controller.UnregisterMutation(gameObject);
            Destroy(gameObject);
        }

        // if near duck, move faster

    }
}
