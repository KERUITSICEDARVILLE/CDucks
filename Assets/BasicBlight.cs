using Unity.VisualScripting;
using UnityEngine;

public class BasicBlight : MonoBehaviour
{
    public BlightController Controller;
    public WorldGrid World;

    public float growth;
    public float Growth {
        set
        {
            GetComponent<SpriteRenderer>().size = new Vector2( 0.25f + 0.75f * growth / MaxGrowth, 0.25f + 0.75f * growth / MaxGrowth);
            growth = value;
        }
        get
        {
            return growth;
        }
    }
    public float MaxGrowth;
    public float GrowthRate;
    public float tolerance;
    public float Taut;

    public Vector2Int cell {
        get { return transform.parent.GetComponent<WorldTile>().tileCoord; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.parent != null) {
            if (transform.parent.parent != null) {
                World = transform.parent.parent.GetComponent<WorldGrid>();
            }
        }

        Controller = GameObject.FindAnyObjectByType<BlightController>().GetComponent<BlightController>();
        Controller.Register(gameObject);
        transform.localScale = new Vector3(1f, 1f, 1f);
        Time.fixedDeltaTime = 0.02f * 5f;
        Growth = MaxGrowth / 3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (FindAnyObjectByType<GameController>().ringMenuBasis != null && Random.Range(0f, 50f) < 29f) {
            return;
        }
        if (Growth <= 0.0)
        {
            Controller.Unregister(gameObject);
            Destroy(gameObject);
        }
        else if (Growth < MaxGrowth)
        {
            Growth += Time.deltaTime * GrowthRate;
        }

        if (Random.Range(MaxGrowth - tolerance, MaxGrowth) > MaxGrowth - tolerance * Mathf.Pow(Growth / MaxGrowth, Taut)) {
            BlightSpread();
        }
        
    }

    public void BlightSpread()
    {
        if (World.CountAdjacentCellsWithoutType<BasicBlight>(cell) > 0) {
            Growth = MaxGrowth / 3;
            BasicBlight baby = Instantiate(this);

            // Get a random adjacent tile without a blight
            Vector2Int neighbor = World.GetRandomAdjacentTileWithoutType<BasicBlight>(cell);

            // If it has a duck KILL IT
            GameObject duck = World.GetObjectAtCell<BasicDuck>(neighbor);
            if (duck != null)
            {
                World.RemoveDuckRing(World.GetTile(neighbor));
                duck.GetComponent<BasicDuck>().enabled = true;
                duck.GetComponent<BasicDuck>().Kill();
            }

            // Add baby to the tile
            World.AddAtCell(baby.gameObject, neighbor);
            baby.transform.localScale = new Vector3(1f, 1f, 1f);

        }
    }

    public void Damage(float amount)
    {
        Growth -= amount;
    }

}
