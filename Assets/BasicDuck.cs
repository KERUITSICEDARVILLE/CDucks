using UnityEngine;

public class BasicDuck : MonoBehaviour
{
    public DuckController Controller;
    public WorldGrid World;

    private bool eventKill;
    public float power;
    public float speed;
    private float cooldown;

    public Vector2Int cell
    {
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
        Controller = GameObject.FindAnyObjectByType<DuckController>().GetComponent<DuckController>();
        Controller.Register(gameObject);
        eventKill = false;
        cooldown = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (eventKill) {
            Controller.Unregister(gameObject);
            Destroy(gameObject);
        }
        if (FindAnyObjectByType<GameController>().ringMenuBasis != null && Random.Range(0f, 50f) < 29f) {
            return;
        }
        if (cooldown > 0f) {
            cooldown -= speed * Time.deltaTime;
        }

        if (cooldown < 0f && World.CountAdjacentCellsWithType<BasicBlight>(cell) > 0)
        {
            WorldTile target = World.GetRandomAdjacentTileWithType<BasicBlight>(cell);
            World.GetObjectAtCell<BasicBlight>(target.tileCoord).GetComponent<BasicBlight>().enabled = true;
            World.GetObjectAtCell<BasicBlight>(target.tileCoord).GetComponent<BasicBlight>().Damage(power);
            
            FindAnyObjectByType<GameController>().money += (int) power;
            cooldown = 1.0f;
        }
    }

    public void Kill() {
        eventKill = true;
    }
}
