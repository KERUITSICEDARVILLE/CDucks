using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[ExecuteInEditMode]
public class WorldGrid : MonoBehaviour
{
    public GameObject tile;
    public float shiftLeft;
    public float shiftUp;

    public int xmin;
    public int xmax;
    public int ymin;
    public int ymax;

    public bool build;

    public Color color1;
    public Color color2;
    public Color color3;

    public HashSet<WorldTile> discoverySet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        discoverySet = new HashSet<WorldTile>();
        for (int i = 0; i < transform.childCount; i++) {
        discoverySet.Add(transform.GetChild(i).GetComponent<WorldTile>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (build && !Application.isPlaying)
        {
            build = false;
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

        newWorldTile.isDiscovered = false;
        newWorldTile.discoveryParentCoord = new Vector2Int(0, 0);
        newWorldTile.lengthToOrigin = 0;
        discoverySet.Add(newWorldTile);
        Debug.Log(discoverySet.Count);
    }

    public Color GetColorForTile(Vector2Int pos)
    {
        Color color;
        if ((pos.x + 200) % 2 == 0)
        {
            if ((pos.y + 300) % 3 == 0)
            {
                color = color1;
            }
            else if ((pos.y + 300) % 3 == 1)
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
            if ((pos.y + 300) % 3 == 0)
            {
                color = color3;
            }
            else if ((pos.y + 300) % 3 == 1)
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

    public GameObject GetObjectAtCell<T>(Vector2Int cell)
    {
        WorldTile tile = GetTile(cell);
        for (int j = 0; j < tile.transform.childCount; j++)
        {
            if (tile.transform.GetChild(j).GetComponent<T>() != null)
            {
                return tile.transform.GetChild(j).gameObject;
            }
        }
        return null;
    }

    public bool AddToRandomEmptyCell<T>(GameObject entity)
    {
        if (!IsFull<T>())
        {
            WorldTile tile = GetRandomTile();

            while (tile.transform.GetComponentInChildren<T>() != null)
            {
                tile = GetRandomTile();
            }

            AddAtTile(entity, tile);

            return true;
        }
        return false;
    }

    public Vector2Int[] sides(Vector2Int cell) // all boundary checks here
    {
        List<Vector2Int> adjacent = new List<Vector2Int>{
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            };
        if ((cell.x + 200) % 2 == 0)
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

    public int CountAdjacentCellsWithType<T>(Vector2Int cell)
    {

        int count = 0;
        foreach (Vector2Int side in sides(cell))
        {
            if (GetObjectAtCell<T>(cell + side) != null)
            {
                count++;
            }
        }
        return count;
    }

    public int CountAdjacentCellsWithoutType<T>(Vector2Int cell)
    {

        int count = 0;
        foreach (Vector2Int side in sides(cell))
        {
            if (GetObjectAtCell<T>(cell + side) == null)
            {
                count++;
            }
        }
        return count;
    }

    public WorldTile[] GetAdjacentTilesWithoutType<T>(Vector2Int cell)
    {
        if (CountAdjacentCellsWithoutType<T>(cell) == 0)
        {
            return null;
        }

        Vector2Int[] neighborDeltas = sides(cell);

        List<WorldTile>ret = new List<WorldTile>();

        foreach (Vector2Int neighbor in neighborDeltas) {
            if (GetObjectAtCell<T>(cell + neighbor) == null) {
                ret.Add(GetTile(cell + neighbor));
            }
        }

        return ret.ToArray();
    }

    public WorldTile[] GetAdjacentTilesWithType<T>(Vector2Int cell)
    {
        if (CountAdjacentCellsWithType<T>(cell) == 0)
        {
            return null;
        }

        Vector2Int[] neighborDeltas = sides(cell);

        List<WorldTile>ret = new List<WorldTile>();

        foreach (Vector2Int neighbor in neighborDeltas) {
            if (GetObjectAtCell<T>(cell + neighbor) != null) {
                ret.Add(GetTile(cell + neighbor));
            }
        }

        return ret.ToArray();
    }

    public Vector2Int GetRandomAdjacentTileWithoutType<T>(Vector2Int cell)
    {
        WorldTile[] ret = GetAdjacentTilesWithoutType<T>(cell);
        return ret[Random.Range(0, ret.Length)].tileCoord;
    }

    // these having different signatures unnerves me slightly

    public WorldTile GetRandomAdjacentTileWithType<T>(Vector2Int cell)
    {
        WorldTile[] ret = GetAdjacentTilesWithType<T>(cell);
        return ret[Random.Range(0, ret.Length)];
    }

    public bool IsFull<T>()
    {
        return EntityCount<T>() == transform.childCount;
    }

    public int EntityCount<T>()
    {
        int count = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform tile = transform.GetChild(i);
            for (int j = 0; j < tile.childCount; j++)
            {
                if (tile.GetChild(j).GetComponent<T>() != null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private bool OnGrid(Vector2Int cell)
    {
        return GetTile(cell) != null;
    }

    public WorldTile BFSstopstart<T>(WorldTile stop, WorldTile start) {
        if (start.isDiscovered) { // enforce start not being discovered
            return null;
        }
        Queue<WorldTile> q = new Queue<WorldTile>();
        HashSet<WorldTile> unChildren;
        HashSet<WorldTile> disChildren;
        WorldTile[] children;
        WorldTile[] children2;
        WorldTile parent;
        start.lengthToOrigin = 1;
        start.isDiscovered = true;
        q.Enqueue(start);

        while (q.Count > 0) {
            parent = q.Dequeue();
            children = GetAdjacentTilesWithType<T>(parent.tileCoord);
            children2 = new WorldTile[children.Length];
            for (int i = 0; i < children.Length; i++) {
                if (children[i].isDiscovered) {
                    children2[i] = children[i];
                    children[i] = null;
                } else {
                    children2[i] = null;
                }
            }
            unChildren = new HashSet<WorldTile>(children);   // undiscovered/discovered
            disChildren = new HashSet<WorldTile>(children2); // children
            unChildren.Remove(null);
            disChildren.Remove(null);
            foreach (WorldTile iChild in unChildren) {
                iChild.discoveryParentCoord = parent.tileCoord;
                iChild.lengthToOrigin = parent.lengthToOrigin + 1;
                iChild.isDiscovered = true;
                q.Enqueue(iChild);
            }
            foreach (WorldTile iChild in disChildren) {
                if (iChild == stop && parent.lengthToOrigin > 6) {
                    return parent;
                }
            }
        }
        return null;
    }

    public Vector2Int[] CheckDuckRing(WorldTile origin) {
        // returns null if no ring found
        // the significance of returning a set of V2's in a ring
        // is not such that there is only one ring. It is to return
        // the shortest ring and leave enough information on the
        // WGrid to generate other paths later.
        origin.lengthToOrigin = 0;
        origin.isDiscovered = true;
        WorldTile BFSEnd;
        WorldTile[] arms = GetAdjacentTilesWithType<BasicDuck>(origin.tileCoord);
        if (arms == null) {
            return null;
        }
        for (int i = 0; i < arms.Length; i++) {
            BFSEnd = BFSstopstart<BasicDuck>(origin, arms[i]);
            if (BFSEnd != null) {
            Debug.Log(arms[i]);
            return new Vector2Int[1] { BFSEnd.tileCoord };
            }
        }
        return null;
    }

    public void ResetDiscoveryChannels() {
        foreach (WorldTile iWorldTile in discoverySet) {
            iWorldTile.isDiscovered = false;
            iWorldTile.lengthToOrigin = 0;
        }
    }
}
