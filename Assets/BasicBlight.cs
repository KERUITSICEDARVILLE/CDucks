using UnityEngine;

public class BasicBlight : MonoBehaviour
{
    public BlightController Controller;
    public WorldGrid World;

    public Animation minus;

    private bool once;
    private float growth;
    public float Growth {
        set
        {
            GetComponent<SpriteRenderer>().size = new Vector2(0.25f + 0.75f * growth / MaxGrowth, 0.25f + 0.75f * growth / MaxGrowth);
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

    public int Lineage;

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
        if (transform.parent != null) {
            if (transform.parent.parent != null) {
                World = transform.parent.parent.GetComponent<WorldGrid>();
            }
        }

        //minus = GetComponent<Animation>();

        Controller = GameObject.FindAnyObjectByType<BlightController>().GetComponent<BlightController>();
        Controller.Register(gameObject);
        transform.localScale = new Vector3(1f, 1f, 1f);
        Time.fixedDeltaTime = 0.02f * 5f;
        Growth = MaxGrowth / 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Controller.enabled) {
            return; 
        }
        if (Growth <= 0.0)
        {
            FindAnyObjectByType<GameController>().wallet += 2;
            Controller.Unregister(gameObject);
            Destroy(gameObject);
        }
        else if (Growth < MaxGrowth)
        {
            Growth += Time.deltaTime * GrowthRate;
        }
        else if (Random.value > .9)
        {
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
                if (!duck.GetComponent<BasicDuck>().Damage(0.2f)) {
                    Destroy(baby.gameObject);
                    return;
                }
            }

            // Add baby to the tile
            World.AddAtCell(baby.gameObject, neighbor);
            baby.transform.localScale = new Vector3(1f, 1f, 1f);

        }
    }

    public void Damage(float amount)
    {
        //minus.Play();
        Growth -= amount;
    }

    public void Wake() {

    }

    public void Sleep() {
        
    }

}
