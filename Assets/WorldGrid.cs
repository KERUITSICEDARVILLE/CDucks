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
    public List<List<WorldTile>> rows;
    public Vector3[] rowAnimPs;

    const float toppleTime = 2f;
    private float toppleControlTime;
    public Vector3 waveNormal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toppleControlTime = 0f;

        discoverySet = new HashSet<WorldTile>();
        rows = new List<List<WorldTile>>();
        List<WorldTile> row;
        WorldTile iChild;
        Vector2Int upperRight;
        WorldTile upperRightTile;

        for (int i = 0; i < transform.childCount; i++) {
        iChild = transform.GetChild(i).GetComponent<WorldTile>();

        // set creation
        discoverySet.Add(iChild);

        // row creation
            if (!iChild.isDiscovered) {
                row = new List<WorldTile>();
                upperRight = iChild.tileCoord;
                upperRightTile = GetTile(upperRight);
                // above necessary for following check
                while (upperRightTile != null) {
                    upperRightTile.isDiscovered = true;
                    row.Add(upperRightTile);
                    upperRight += new Vector2Int(1, upperRight.x % 2);
                    upperRightTile = GetTile(upperRight);
                }

                rows.Add(row);
            }
        }

        rowAnimPs = new Vector3[rows.Count];

        for (int i = 0; i < rows.Count; i++) {
            rowAnimPs[i] = new Vector3(
                                    Random.Range(5f, 6f),
                                    Random.Range(-41f, -40f),
                                    Random.Range(4f, 5f));
        }

        ResetDiscoveryChannels();
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

        if (Application.isPlaying) {
        AnimateRows(toppleControlTime, toppleControlTime + Time.deltaTime);
        }

        if (toppleControlTime < toppleTime) {
            toppleControlTime += Time.deltaTime;
        } else {
            toppleControlTime = 0f;
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

    private void BezierBoil(int order, Vector2[] controls, float t) { // puts result in controls[0]
        for (int i = order - 1; i == 0 ? false : true; i--) {
            for (int j = 0; j < i; j++) {
            controls[j].x = (1 - t) * controls[j].x + t * controls[j + 1].x;
            controls[j].y = (1 - t) * controls[j].y + t * controls[j + 1].y;
            }
        }
    }

    private void AnimateRows(float time1, float time2) {
        List<WorldTile>[] rowArray = rows.ToArray();
        WorldTile[] singleRow;
        float thetaRange, halfPlane, theta1, theta2;
        float r, realX, tilexValue;
        Vector2[] closestDeltasArr;
        Vector2 delta;    

        Vector2[] ctl1Points = new Vector2[3];
        Vector2[] ctl2Points = new Vector2[3];

        for (int i = 0; i < rowArray.Length; i++) {
        realX = rowAnimPs[i].x + rowAnimPs[i].z;
        halfPlane = Mathf.Atan(Mathf.Abs(rowAnimPs[i].y / realX));
        thetaRange = (Mathf.PI - 2f * halfPlane);
        theta1 = time1 / toppleTime * thetaRange + halfPlane;
        theta2 = time2 / toppleTime * thetaRange + halfPlane;

        r = new Vector2(realX, rowAnimPs[i].y).magnitude;

        singleRow = rowArray[i].ToArray();

        closestDeltasArr = new Vector2[singleRow.Length];

            for (float t = 0; t < 1f; t += 0.001f) { // test out points on row

            ctl1Points[0] = new Vector2(0f, 0f);
            ctl1Points[1] = new Vector2(r * Mathf.Cos(theta1) + rowAnimPs[i].x,
                                        r * Mathf.Sin(theta1) + rowAnimPs[i].y);
            ctl1Points[2] = new Vector2(2f * rowAnimPs[i].x, 0f);

            ctl2Points[0] = new Vector2(0f, 0f);
            ctl2Points[1] = new Vector2(r * Mathf.Cos(theta2) + rowAnimPs[i].x,
                                        r * Mathf.Sin(theta2) + rowAnimPs[i].y);
            ctl2Points[2] = new Vector2(2f * rowAnimPs[i].x, 0f);

            BezierBoil(3, ctl1Points, t);
            BezierBoil(3, ctl2Points, t);

            ctl1Points[0].x = 0f;

            delta = ctl2Points[0];// - ctl1Points[0];

                for (int c = 0; c < singleRow.Length; c++) {
                    tilexValue = 2f * realX * ((float)c + 0.5f) / (float)singleRow.Length - rowAnimPs[i].z;
                    if (closestDeltasArr[c] == null) {
                        closestDeltasArr[c] = delta;
                    } else {
                        if (Mathf.Abs(delta.x - tilexValue) < Mathf.Abs(closestDeltasArr[c].x - tilexValue)) {
                            closestDeltasArr[c] = delta;
                        }
                    }
                }

            }

            for (int c = 0; c < singleRow.Length; c++) {
                //if (singleRow[c].tileCoord == new Vector2Int(4, 3)) {
                //singleRow[c].transform.localPosition += closestDeltasArr[c].y * waveNormal;
                //singleRow[c].transform.position += new Vector3(0f, closestDeltasArr[c].y, 0f);
                singleRow[c].transform.localPosition = singleRow[c].initialTransform + closestDeltasArr[c].y * waveNormal;
                //}
            }


        }
    }
}
