using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class UsefulGrid : MonoBehaviour
{
    public int width;
    public int height;

    private Vector2Int[] sides = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

    public int EntityCount
    {
        get { return transform.childCount;  }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetCellPosition(Vector2Int cell)
    {
        // Converts a cell location (x, y) : 0 <= x < wdith, 0 <= y < height
        // to a position on the grid
        RectTransform rect = GetComponent<RectTransform>();
        return new Vector3(rect.sizeDelta.x * (cell.x + .5f) / width, rect.sizeDelta.y * (cell.y + .5f) / height, 0.0f);
    }

    public void AddAtCell(GameObject entity, Vector2Int cell)
    {
        entity.transform.SetParent(transform);
        entity.transform.localPosition = GetCellPosition(cell);
    }

    public GameObject GetObjectAtCell(Vector2Int cell)
    {
        
        Vector3 CellPosition = GetCellPosition(cell);
        

        for (int i = 0; i < transform.childCount; i++)
        {
            if ((transform.GetChild(i).transform.localPosition - CellPosition).sqrMagnitude < 0.0001)
            {
                return transform.GetChild(i).gameObject;
            }
        }
        
        return null;
    }

    public bool AddToRandomEmptyCell(GameObject entity)
    {
        if (transform.childCount < width * height)
        {
            Vector2Int cell = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            while (GetObjectAtCell(cell) != null)
            {
                cell = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            }

            AddAtCell(entity, cell);

            return true;
        }
        return false;
    }

    public int CountAdjacent(Vector2Int cell)
    {

        int count = 0;
        foreach (Vector2Int side in sides){
            if (GetObjectAtCell(cell + side) != null)
            {
                count++;
            }
        }
        return count;
    }

    public int CountEmptyAdjacent(Vector2Int cell)
    {

        int count = 0;
        foreach (Vector2Int side in sides)
        {
            if (OnGrid(cell + side))
            {
                if (GetObjectAtCell(cell + side) == null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public bool AddToAdjacentEmptyCell(GameObject entity, Vector2Int cell)
    {
        if (CountEmptyAdjacent(cell) > 0)
        {
            Vector2Int neighbor = cell + sides[Random.Range(0, sides.Length)];

            while (GetObjectAtCell(neighbor) != null || !OnGrid(neighbor))
            {
                neighbor = cell + sides[Random.Range(0, sides.Length)];
            }

            AddAtCell(entity, neighbor);
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        return transform.childCount == width * height;
    }

    private bool OnGrid(Vector2Int cell)
    {
        return 0 <= cell.x && cell.x < width && 0 <= cell.y && cell.y < height;
    }
}
