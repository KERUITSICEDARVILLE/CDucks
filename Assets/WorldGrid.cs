using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
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

    public bool DoDrift;

    [Header("BFS and animation")]
    public bool DoWaving;
    public HashSet<WorldTile> discoverySet;
    public List<List<WorldTile>> duckRings;
    public List<List<WorldTile>> rows;
    public Vector3[] rowAnimPs;

    public float toppleTime;
    private float toppleControlTime;
    public Vector3 waveNormal;

    [Header("Performance Concerns")]
    private Dictionary<Vector2Int, WorldTile> tileMap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toppleControlTime = 0f;

        tileMap = new Dictionary<Vector2Int, WorldTile>();
        discoverySet = new HashSet<WorldTile>();
        duckRings = new List<List<WorldTile>>();
        rows = new List<List<WorldTile>>();
        List<WorldTile> row;
        WorldTile iChild;
        Vector2Int upperRight;
        WorldTile upperRightTile;

        for (int i = 0; i < transform.childCount; i++) {
        iChild = transform.GetChild(i).GetComponent<WorldTile>();
        tileMap.Add(iChild.tileCoord, iChild);
        }

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
            } // end row creation
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

        // potentially move to Controller
        if (DoWaving)
        {
            if (Application.isPlaying)
            {
                AnimateRows(toppleControlTime / toppleTime);
            }
        }
        
        if (toppleControlTime < toppleTime)
        {
            toppleControlTime += Time.deltaTime;
        }
        else
        {
            toppleControlTime = 0f;
            // slide everything left
            if (FindAnyObjectByType<GameController>().money > 0
                && FindAnyObjectByType<GameController>().borderCleanse)
            {
                ReparentRows();
                FindAnyObjectByType<GameController>().money--;
            } else {
                FindAnyObjectByType<GameController>().borderCleanse = false;
            }
        }
        // no seriously, please move this to the Controller

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
        if (tileMap.ContainsKey(cell)) {
            return tileMap[cell];
        }
        return null;
    }

    public WorldTile GetRandomTile()
    {
        return transform.GetChild(Random.Range(0, transform.childCount)).GetComponent<WorldTile>();
    }

    public GameObject GetObjectAtCell<T>(Vector2Int cell)
    {
        //if (empty.GetComponent<T>() != null) {
        //    return empty.gameObject;
        //}
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

    public bool boolint(int x) {
        return x == 0 ? false : true;
    }

    // START RANGE CAPABILITIES
    public Vector2Int[] CellNeighborhood(Vector2Int cell_origin, int range) {
        if (range < 1) {
            range = 1;
        }
        HashSet<Vector2Int>neighborhood_a = new HashSet<Vector2Int>();
        HashSet<Vector2Int>neighborhood_b;
        Vector2Int[] neighbors;
        neighborhood_a.Add(cell_origin);
        while (boolint(range--)) {
            neighborhood_b = new HashSet<Vector2Int>(neighborhood_a);
            foreach (Vector2Int cell in neighborhood_a) {
                neighbors = sides(cell);
                for (int i = 0; i < neighbors.Length; i++) {
                    neighborhood_b.Add(neighbors[i] + cell);
                }
            }
            neighborhood_a = neighborhood_b;
        }
        neighborhood_a.Remove(cell_origin);
        return new List<Vector2Int>(neighborhood_a).ToArray();
    }

    public int CountAdjacentCellRangeWithType<T>(Vector2Int cell, int range)
    {

        int count = 0;
        foreach (Vector2Int side in CellNeighborhood(cell, range))
        {
            if (GetObjectAtCell<T>(cell) != null)
            {
                count++;
            }
        }
        return count;
    }

    public WorldTile[] GetAdjacentTileRangeWithType<T>(Vector2Int cell, int range)
    {
        if (CountAdjacentCellRangeWithType<T>(cell, range) == 0)
        {
            return null;
        }

        List<WorldTile>ret = new List<WorldTile>();

        foreach (Vector2Int neighbor in CellNeighborhood(cell, range)) {
            if (GetObjectAtCell<T>(neighbor) != null) {
                ret.Add(GetTile(neighbor));
            }
        }

        return ret.ToArray();
    }

    public WorldTile GetRandomAdjacentTileRangeWithType<T>(Vector2Int cell, int range) {
        WorldTile[] cells = GetAdjacentTileRangeWithType<T>(cell, range);
        return cells[Random.Range(0, cells.Length)];
    }

    public WorldTile[] GetAdjacentTileStripeWithType<T>(Vector2Int cell, int stripe)
    {
        if (stripe == 1) {
            GetAdjacentTileRangeWithType<T>(cell, 1);
        }

        if (CountAdjacentCellRangeWithType<T>(cell, stripe) == 0)
        {
            return null;
        }

        List<WorldTile>rangeBigger = new List<WorldTile>();
        List<WorldTile>rangeLesser = new List<WorldTile>();

        foreach (Vector2Int neighbor in CellNeighborhood(cell, stripe)) {
            if (GetObjectAtCell<T>(neighbor) != null) {
                rangeBigger.Add(GetTile(neighbor));
            }
        }

        foreach (Vector2Int neighbor in CellNeighborhood(cell, stripe - 1)) {
            if (GetObjectAtCell<T>(neighbor) != null) {
                rangeLesser.Add(GetTile(neighbor));
            }
        }

        foreach (WorldTile removable in rangeLesser) {
            rangeBigger.Remove(removable);
        }

        if (rangeBigger.Count == 0) {
            return null;
        }

        return rangeBigger.ToArray();
    }
    // END RANGE CAPABILITIES

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
        if (ret == null) {
            return Vector2Int.zero;
        }
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
        return EntityCount<T>() >= transform.childCount;
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
                    break;
                }
            }
        }
        return count;
    }

    private bool OnGrid(Vector2Int cell)
    {
        return GetTile(cell) != null;
    }

    public WorldTile BFSstopstart<T>(WorldTile stop, WorldTile start, bool evade, int pathMinLength) {
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
            children = evade ? GetAdjacentTilesWithoutType<T>(parent.tileCoord)
                             : GetAdjacentTilesWithType<T>(parent.tileCoord);
            if (children == null) {
                children = new WorldTile[0];
            }
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
                if (iChild == stop && parent.lengthToOrigin >= pathMinLength) {
                    return parent;
                }
            }
        }
        return null;
    }

    public List<WorldTile> WithinDuckRing(WorldTile check) {
        foreach (List<WorldTile> ring in duckRings) {
            foreach (WorldTile tile in ring) {
                if (tile == check) {
                    return ring;
                }
            }
        }
        return null;
    }

    public bool RemoveDuckRing(WorldTile check) {
        int removeIndex = duckRings.Count;
        for (int i = 0; i < duckRings.Count; i++) {
            foreach (WorldTile tile in duckRings[i]) {
                if (tile == check) {
                    removeIndex = i;
                }
            }
        }
        if (removeIndex != duckRings.Count) {
            duckRings.Remove(duckRings[removeIndex]);
        }
        return (removeIndex != duckRings.Count);
    }

    public List<WorldTile> AddNewDuckRing(WorldTile endpt) {
        List<WorldTile> ring = new List<WorldTile>();

        WorldTile curr = endpt;

        while (curr != null) {
            ring.Add(curr);
            curr = GetTile(curr.discoveryParentCoord);
        }
        duckRings.Add(ring);
        return ring;
    }

    public List<WorldTile> CheckDuckRing(WorldTile origin) {
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
            BFSEnd = BFSstopstart<BasicDuck>(origin, arms[i], false, 5);
            if (BFSEnd != null) {
            arms[i].discoveryParentCoord = origin.tileCoord;
            return AddNewDuckRing(BFSEnd);
            }
        }
        return null;
    }

    public void ResetDiscoveryChannels() {
        foreach (WorldTile iWorldTile in discoverySet) {
            iWorldTile.discoveryParentCoord = Vector2Int.zero;
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

    private void AnimateRows(float sweep) {
        List<WorldTile> row;
        float thetaRange, halfPlane, theta;
        float r, realX, tilexValue, t;

        Vector2[] ctlPoints = new Vector2[3];

        for (int i = 0; i < rows.Count; i++) {
        row = rows[i];
        realX = rowAnimPs[i].x + rowAnimPs[i].z;
        halfPlane = Mathf.Atan(Mathf.Abs(rowAnimPs[i].y / realX));
        thetaRange = (Mathf.PI - 2f * halfPlane);
        theta = sweep * thetaRange + halfPlane;

        r = new Vector2(realX, rowAnimPs[i].y).magnitude;

        // find relevant x and t(x)

            for (int c = 0; c < row.Count; c++) {
                tilexValue = 2f * rowAnimPs[i].x * ((float)c + 0.5f) / (float)row.Count;
                ctlPoints[0] = new Vector2(0f, 0f);
                ctlPoints[1] = new Vector2( r * Mathf.Cos(theta) + rowAnimPs[i].x,
                                        r * Mathf.Sin(theta) + rowAnimPs[i].y);
                ctlPoints[2] = new Vector2(2f * rowAnimPs[i].x, 0f);
                t = (Mathf.Sqrt(/*b^2*/ ctlPoints[1].x * ctlPoints[1].x
                                /*4ac*/ + tilexValue * (ctlPoints[2].x - 2f * ctlPoints[1].x)) - ctlPoints[1].x)
                    / (ctlPoints[2].x - 2f * ctlPoints[1].x);
                BezierBoil(3, ctlPoints, t);
                //Debug.Log(ctlPoints[0].x + "same as?: " + tilexValue);
                row[c].transform.localPosition = row[c].initialTransform + ctlPoints[0].y * waveNormal * 0.5f;
            }

        }
    }

    public void ReparentRows() {
        GameObject victimObj;
        BasicBlight victim;
        WorldTile[] row;
        int localMoney;

        foreach (List<WorldTile> rowList in rows) {
            row = rowList.ToArray();
            victimObj = GetObjectAtCell<BasicBlight>(row[0].tileCoord);
            localMoney = FindAnyObjectByType<GameController>().money;
            if (victimObj != null) {
                FindAnyObjectByType<GameController>().money++;
                victim = victimObj.GetComponent<BasicBlight>();
                victim.Growth = -1f;
            }

            for (int i = 1; i < row.Length; i++) {
                if (row[i].transform.childCount != 0
                    && row[i - 1].transform.childCount == 0
                    && WithinDuckRing(row[i]) == null) {
                    while (row[i].transform.childCount > 0) {
                        row[i].transform.GetChild(0).SetParent(row[i - 1].transform);
                    }
                    for (int c = 0; c < row[i - 1].transform.childCount; c++) {
                        row[i - 1].transform.GetChild(c).localPosition = new Vector3(0f, 0f, -1f);
                    }
                }
            }
        }
    }
}
