using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldGrid : MonoBehaviour
{
    public GameObject tile;
    public float shiftLeft; // 0.5 - 0.05
    public float shiftUp;   // \sqrt{3}/(0.5 - 0.05)

    public int xmin;
    public int xmax;
    public int ymin;
    public int ymax;

    public bool build;
    private bool buildOld;

    public Color color1;
    public Color color2;
    public Color color3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    { // is this managing cell destruction between rounds or is it destroying each frame?
        if (build != buildOld && !Application.isPlaying)
        {
            buildOld = build;
            int ToDestroy = transform.childCount;
            for (int i = 0; i < ToDestroy; i++)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            for (int x = xmin; x <= xmax; x++)
            {
                for (int y = ymin; y <= ymax; y++)
                {
                    addTile(new Vector2Int(x, y));
                }
            }
        }
    }

    private Vector3 TileToPos(Vector2Int pos)
    {
        if (pos.x % 2 == 0)
        {
            return new Vector3(pos.x * shiftLeft, pos.y * shiftUp * 2f);
        }
        else
        {
            return new Vector3(pos.x * shiftLeft, pos.y * shiftUp * 2f + shiftUp, 0);
        }
    }

    private void addTile(Vector2Int pos)
    {
        GameObject newTile = Instantiate(tile);
        newTile.transform.SetParent(transform);
        newTile.transform.localPosition = TileToPos(pos);

        WorldTile newWorldTile = newTile.GetComponent<WorldTile>();
        newWorldTile.tileCoord = pos;
        newWorldTile.color = GetColorForTile(pos);
        newWorldTile.heighlight = Color.white;
    }

    public Color GetColorForTile(Vector2Int pos)
    {
        Color color;
        if (pos.x % 2 == 0)
        {
            if (pos.y % 3 == 0)
            {
                color = color1;
            }
            else if (pos.y % 3 == 1)
            {
                color = color2;
            }
            else
            {
                color = color3;
            }
        }
        else
        {
            if (pos.y % 3 == 0)
            {
                color = color3;
            }
            else if (pos.y % 3 == 1)
            {
                color = color1;
            }
            else
            {
                color = color2;
            }
        }
        return color;
    }

    public void AddAtCell(GameObject entity, Vector2Int cell)
    {
        WorldTile tile = GetTile(cell);
        AddAtTile(entity, tile);
    }

    public void AddAtTile(GameObject entity, WorldTile tile)
    { 
        entity.transform.SetParent(tile.transform);
        entity.transform.localPosition = Vector3.back;
    }

    public WorldTile GetTile(Vector2Int cell)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            WorldTile tile = transform.GetChild(i).gameObject.GetComponent<WorldTile>();
            if (tile.tileCoord == cell)
            {
                return tile;
            }
        }

        return null;
    }

    public WorldTile GetRandomTile()
    {
        return transform.GetChild(Random.Range(0, transform.childCount)).GetComponent<WorldTile>();
    }

    public GameObject GetObjectAtCell(Vector2Int cell)
    {

        WorldTile tile = GetTile(cell);
        if (tile.transform.childCount > 0)
        {
            return tile.transform.GetChild(0).gameObject;
        }
        return null;
    }

    public bool AddToRandomEmptyCell(GameObject entity)
    {
        if (!IsFull())
        {
            WorldTile tile = GetRandomTile();

            while (tile.transform.childCount > 0)
            {
                tile = GetRandomTile();
            }

            AddAtTile(entity, tile);

            return true;
        }
        return false;
    }

    public Vector2Int[] sides(Vector2Int cell)
    {
        List<Vector2Int> adjacent = new List<Vector2Int>{
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            };
        if (cell.x % 2 == 0)
        {
            adjacent.Add(new Vector2Int(1, -1));
            adjacent.Add(new Vector2Int(-1, -1));
        }
        else
        {
            adjacent.Add(new Vector2Int(1, 1));
            adjacent.Add(new Vector2Int(-1, 1));
        }
        for (int i = adjacent.Count - 1; i >= 0; i--)
        {
            if (!OnGrid(cell + adjacent[i]))
            {
                adjacent.RemoveAt(i);
            }
        }
        return adjacent.ToArray();
    }

    public int CountAdjacent(Vector2Int cell)
    {

        int count = 0;
        foreach (Vector2Int side in sides(cell))
        {
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
        foreach (Vector2Int side in sides(cell))
        {
            if (GetObjectAtCell(cell + side) == null)
            {
                count++;
            }
        }
        return count;
    }

    public bool AddToAdjacentEmptyCell(GameObject entity, Vector2Int cell)
    {
        if (CountEmptyAdjacent(cell) > 0)
        {
            Vector2Int[] CellSides = sides(cell);
            Vector2Int neighbor = cell + CellSides[Random.Range(0, CellSides.Length)];

            while (GetObjectAtCell(neighbor) != null)
            {
                neighbor = cell + CellSides[Random.Range(0, CellSides.Length)];
            }

            AddAtCell(entity, neighbor);
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount == 0)
            {
                return false;
            }
        }
        return true;
    }

    public int EntityCount()
    {
        int count = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount != 0)
            {
                count++;
            }
        }
        return count;
    }

    private bool OnGrid(Vector2Int cell)
    {
        return GetTile(cell) != null;
    }
}
