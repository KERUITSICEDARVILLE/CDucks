using Unity.VisualScripting;
using UnityEngine;

public class BasicBlight : MonoBehaviour
{

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
        Growth = MaxGrowth / 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (FindAnyObjectByType<GameController>().ringMenuBasis != null && Random.Range(0f, 50f) < 29f) {
            return;
        }
        if (Growth <= 0.0)
        {
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
        WorldGrid world = transform.parent.parent.GetComponent<WorldGrid>();
        if (world.CountAdjacentCellsWithoutType<BasicBlight>(cell) > 0)
        {
            Growth = MaxGrowth / 3;
            BasicBlight baby = Instantiate(this);

            // Get a random adjacent tile without a blight
            Vector2Int neighbor = world.GetRandomAdjacentTileWithoutType<BasicBlight>(cell);

            // If it has a duck KILL IT
            GameObject duck = world.GetObjectAtCell<BasicDuck>(neighbor);
            if (duck != null)
            {
                world.RemoveDuckRing(world.GetTile(neighbor));
                Destroy(duck);
            }

            // Add baby to the tile
            world.AddAtCell(baby.gameObject, neighbor);
            baby.transform.localScale = new Vector3(1f, 1f, 1f);

        }
    }

    public void Damage(float amount)
    {
        Growth -= amount;
    }

}
