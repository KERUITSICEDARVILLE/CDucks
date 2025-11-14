using UnityEngine;

public class BasicDuck : MonoBehaviour
{

    public float power;

    public Vector2Int cell
    {
        get { return transform.parent.GetComponent<WorldTile>().tileCoord; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WorldGrid world = transform.parent.parent.GetComponent<WorldGrid>();
        if (world.CountAdjacentCellsWithType<BasicBlight>(cell) > 0)
        {
            WorldTile target = world.GetRandomAdjacentTileWithType<BasicBlight>(cell);
            world.GetObjectAtCell<BasicBlight>(target.tileCoord).GetComponent<BasicBlight>().Damage(Time.deltaTime * power);
            if (Random.value < 0.01)
            {
                FindAnyObjectByType<GameController>().money += 1;
            }
        }
    }
}
