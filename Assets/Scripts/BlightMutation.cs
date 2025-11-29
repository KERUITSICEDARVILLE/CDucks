using UnityEngine;
using System.Collections.Generic;

public class BlightMutation : MonoBehaviour
{
    private bool once;

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
        if (!once) {
            once = true;
        } else {
            return;
        }
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
        if (!Controller.enabled) {
            return; 
        }
        // if GameController.regionIndex != -1 then show HUD alert

        if (TargetTile == null || Path == null) {
            return;
        }
        GameObject loadTarget = World.GetObjectAtCell<BasicBlight>(TargetTile.tileCoord);
        if (loadTarget == null) {
            Controller.GiveMeTarget(this);
            //Controller.RetargetNear(this, TargetTile);
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

        GameObject obj = World.GetObjectAtCell<BasicBlight>(cell);
        BasicBlight objBlight;
        if (obj != null)
        {
            objBlight = obj.GetComponent<BasicBlight>();
            // begin transformation
            objBlight.GrowthRate += Random.Range(3.0f, 4.0f);
            objBlight.MaxGrowth += Random.Range(4.0f, 5.0f);
            obj.GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f / objBlight.GrowthRate, 1f / objBlight.GrowthRate, 1f);
            objBlight.Lineage = Controller.GiveMeUniqueID();
            // end transformation
            Controller.UnregisterMutation(gameObject);
            Destroy(gameObject);
        }

        // if near duck, move faster

    }
}
