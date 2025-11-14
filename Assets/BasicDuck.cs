using UnityEngine;

public class BasicDuck : MonoBehaviour
{

    public float power;
    private float exhaustionRecovery;
    private float exhaustionTimer;
    public float timer;

    public Vector2Int cell
    {
        get { return transform.parent.GetComponent<WorldTile>().tileCoord; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exhaustionTimer = timer;
        exhaustionRecovery = -0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (FindAnyObjectByType<GameController>().ringMenuBasis != null && Random.Range(0f, 50f) < 29f) {
            return;
        }
        if (exhaustionTimer < 0f) {
            exhaustionTimer = timer;
            exhaustionRecovery = timer;
        }

        exhaustionRecovery -= Time.deltaTime;

        WorldGrid world = transform.parent.parent.GetComponent<WorldGrid>();
        if (exhaustionRecovery < 0f && world.CountAdjacentCellsWithType<BasicBlight>(cell) > 0)
        {
            WorldTile target = world.GetRandomAdjacentTileWithType<BasicBlight>(cell);
            world.GetObjectAtCell<BasicBlight>(target.tileCoord).GetComponent<BasicBlight>().Damage(Time.deltaTime * power);
            if (Random.Range(0f, 20f) < 1f) {
                FindAnyObjectByType<GameController>().money++;
            }
            exhaustionTimer -= Time.deltaTime;
        }
    }
}
