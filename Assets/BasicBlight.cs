using UnityEngine;

public class BasicBlight : MonoBehaviour
{
    public BlightController Controller;
    public WorldGrid World;

    public Animation minus;

    private float growth;
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

    public int Lineage;

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

        //minus = GetComponent<Animation>();

        Controller = GameObject.FindAnyObjectByType<BlightController>().GetComponent<BlightController>();
        Controller.Register(gameObject);
        transform.localScale = new Vector3(1f, 1f, 1f);
        Time.fixedDeltaTime = 0.02f * 5f;
        Growth = MaxGrowth / 3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject obj = World.GetObjectAtCell<BlightMutation>(cell);
        if (obj != null) {
            // begin transformation
            GrowthRate += Random.Range(1.0f, 1.8f);
            MaxGrowth += Random.Range(2.0f, 5.0f);
            GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f / GrowthRate, 1f / GrowthRate, 1f);
            Lineage = Controller.GiveMeUniqueID();
            // end transformation
            Controller.UnregisterMutation(obj);
            Destroy(obj);
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

        if (transform.parent.GetComponent<WorldTile>().isBeingPressed) {
            Growth -= Time.deltaTime * GrowthRate * 4f;
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

}
