using UnityEngine;

public class BasicDuck : MonoBehaviour
{

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
        cooldown = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (FindAnyObjectByType<GameController>().ringMenuBasis != null && Random.Range(0f, 50f) < 29f) {
            return;
        }
        if (cooldown > 0f) {
            cooldown -= speed * Time.deltaTime;
        }

        WorldGrid world = transform.parent.parent.GetComponent<WorldGrid>();
        if (cooldown < 0f && world.CountAdjacentCellsWithType<BasicBlight>(cell) > 0)
        {
            WorldTile target = world.GetRandomAdjacentTileWithType<BasicBlight>(cell);
            world.GetObjectAtCell<BasicBlight>(target.tileCoord).GetComponent<BasicBlight>().Damage(power);
            
            FindAnyObjectByType<GameController>().money += (int) power;
            cooldown = 1.0f;
        }
    }
}
