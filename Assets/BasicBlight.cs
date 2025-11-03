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
        if (Growth <= 0.0)
        {
            Die();
        }
        else if (Growth < MaxGrowth)
        {
            Growth += Time.deltaTime * GrowthRate;
        }
        else
        {
            BlightSpread();
        }
        
    }

    public void BlightSpread()
    {
        WorldGrid world = transform.parent.parent.GetComponent<WorldGrid>();
        if (world.CountEmptyAdjacent(cell) > 0)
        {
            Growth = MaxGrowth / 3;
            BasicBlight baby = Instantiate(this);

            if (!world.AddToAdjacentEmptyCell(baby.gameObject, cell))
            {
                Destroy(baby.gameObject);
            }
            
        }
    }

    public void OnMouseDown()
    {
        Growth -= 1;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
