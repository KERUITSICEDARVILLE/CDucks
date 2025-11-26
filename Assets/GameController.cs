using TMPro;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.LightTransport;

using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Game State")]
    public bool gameEnd;
    public GameObject CameraObject;
    public Vector3 cameraOrigin;
    public Vector3 scaleOrigin;
    const int RoundMax = 7;
    const int numDucks = 6;
    public int Round;
    private int moneyAmount;
    public int money {
        set {
            moneyAmount = value;
            MoneyDisplay.text = "" + moneyAmount;
        }
        get {
            return moneyAmount;
        }
    }

    public List<WorldTile> ringMenuBasis;

    public bool borderCleanse;
    public int cursorMode;

    [Header("Map Region State")]
    public int RegionNum;
    public Vector3[] cameraMove;
    public Vector3[] controllerScale;
    private int regionIndex;
    private Vector3 prevCamera;
    private Vector3 prevScale;
    private Vector3 eventualCamera;
    private Vector3 eventualScale;

    [Header("Scene Setup")]
    public BlightController bController;
    public DuckController dController;
    public GameObject UI;
    public GameObject Shop;
    public GameObject Tangle;
    public GameObject RingMenu;
    public WorldGrid World;
    private GameObject Menu;
    private int selection;
    public int unlocks;
    private float uniTime;
    public float RegionZoomDuration;
    public float RoundMessageDuration;
    private float RegionZoomTimer;
    public float[] RoundDurations;
    private float RoundTimer;
    private float RoundStartMessageTimer;

    [Header("UI Elements")]
    public TMP_Text RoundTMP;
    public TMP_Text RoundTime;
    public TMP_Text Message;
    public TMP_Text MoneyDisplay;
    public Button SkipButton;

    [Header("Cursors")]
    public Texture2D cleanerCursor;
    public Texture2D panCursor;

    public Texture2D basicDuckCursor;
    public Texture2D armyDuckCursor;
    public Texture2D ninjaDuckCursor;
    public Texture2D superDuckCursor;
    public Texture2D robotDuckCursor;
    public Texture2D mythicDuckCursor;

    public Texture2D bleachPowerCursor;
    public Texture2D damagePowerCursor;
    public Texture2D speedPowerCursor;
    public Texture2D duckScooper;
    public Texture2D specialPowerCursor;

    [Header("Item Costs")]
    public int Duck1Cost;
    public int Duck2Cost;
    public int Duck3Cost;
    public int Duck4Cost;
    public int Duck5Cost;
    public int Duck6Cost;

    public int Power1Cost;
    public int Power2Cost;
    public int Power3Cost;
    public int Power4Cost;

    [Header("Item Prefabs")]
    public GameObject Duck1;
    public GameObject Duck2;
    public GameObject Duck3;
    public GameObject Duck4;
    public GameObject Duck5;
    public GameObject Duck6;

    public GameObject Power1;
    public GameObject Power2;
    public GameObject Power3;
    public GameObject Power4;

    [Header("Enemies")]
    public GameObject[] Blights;
    public GameObject BlightMutation;

    void Start()
    {
        gameEnd = false;
        uniTime = 0f;
        Menu = null;
        unlocks = 1;
        Round = 0;
        money = 0;
        selection = -1;
        regionIndex = -1;
        ringMenuBasis = null;
        borderCleanse = false;
        RegionZoomTimer = 0;
        RoundStartMessageTimer = 0;
        RoundTimer = 0;
        Cursor.SetCursor(GetCursorForMode(0), Vector2.zero, CursorMode.Auto);
        eventualScale = scaleOrigin;
        eventualCamera = cameraOrigin;
    }

    // Update is called once per frame
    void Update()
    {

        // Animate zoom
        float t;
        if (RegionZoomTimer > 0) {
            t = RegionZoomTimer / RegionZoomDuration;
            CameraObject.transform.localPosition = (1 - t) * eventualCamera + t * prevCamera;
            transform.localScale = (1 - t) * eventualScale + t * prevScale;
            RegionZoomTimer -= Time.deltaTime;
        }

        if (gameEnd) {
            return;
        }

        // Have we lost yet? Progress to next round if no blight or timer < 0f
        int divvy = (int)RoundTimer;
        if (RoundTimer > 0f) {
            RoundTime.text = ( (divvy < 60) ? ("") : (divvy / 60 + ":") ) + ((divvy % 60 > 9) ? (divvy % 60):("0" + divvy % 60));
            RoundTimer -= Time.deltaTime;
            if (RoundTimer < 10f) {
                RoundTime.transform.localPosition = new Vector3(0f, (10f - RoundTimer) / 2f * (RoundTimer - divvy) * Mathf.Sin(RoundTimer * 10f * Mathf.PI), 0f);
            }
        }
        if (World.EntityCount<BasicBlight>() == 0)
        {
            SkipButton.interactable = true;
        }

        if (RoundStartMessageTimer > 0)
        {
            RoundStartMessageTimer -= Time.deltaTime;
            if (RoundStartMessageTimer <= 0)
            {
                RoundStartMessageTimer = 0;

            }

            Message.color = new Color(1.0f, 1.0f, 1.0f, RoundStartMessageTimer / RoundMessageDuration);
        }

        // scuffed old system inputs
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f) {
            if (scroll < 0f) {
                selection = (selection + 1) % unlocks;
            } else {
                selection = selection < 1 ? unlocks - 1 : selection - 1;
            }
            SetCursorMode(selection);
            /* if we are hovering over a tile then
            Tile.OnMouseExit();
            Tile.OnMouseEnter();
            */
        }

        if (Input.GetMouseButton(1)) {
            MapUnfocus();
        }
        
        if (Input.GetKeyDown("escape")) {
            UI.GetComponent<Canvas>().enabled = !UI.GetComponent<Canvas>().enabled;
        }

        if ((Input.GetMouseButton(2) || (Input.GetMouseButton(0) && selection == -2)) && regionIndex != -1) {
            Vector3 perPixel =  ( Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)) -
                                Camera.main.ScreenToWorldPoint(new Vector3(64, 0, 0)) );
            CameraObject.transform.localPosition += Input.mousePositionDelta * perPixel.x / 32f;
        }
        // end scuffed old system inputs

        if (selection >= 0) {
            Tangle.transform.localPosition = Shop.transform.GetChild(selection).transform.localPosition + new Vector3(0f, 13f, 0f);
        }
        Tangle.SetActive(selection >= 0);

        if (RoundTimer <= 0f && Round > RoundMax) {
            bController.Nuke();
            MapUnfocus();
            WinGame();
            return;
        }

        if (bController.isFull())
        {
            dController.Nuke();
            MapUnfocus();
            LoseGame();
            return;
        }

        if (RoundTimer <= 0f)
        {
            StartNextRound();
        }

        // Duck Ring Menu System
        /*if (ringMenuBasis != null) {
            HeighlightRing();
            HandleRingMenu();
        }
        if (ringMenuBasis == null && Menu != null) {
            MenuToggle eventScript = Menu.transform.GetComponent<MenuToggle>();
            if (eventScript.readyDestroy) {
                Destroy(Menu);
                Menu = null;
            }
        }*/

    }

    private void DisplayRound()
    {
        Message.text = "Round " + Round;
        RoundStartMessageTimer = RoundMessageDuration;
        Message.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void SpawnRound()
    {
        if (Round < 7)
        {
            int EnemyCount = 13 + 2 * Round + Round * Round / 5;
            for (int i = 0; i < EnemyCount; i++) {
                GameObject enemy = Instantiate(Blights[Round-1]);
                AddBlightToRandomCell(enemy);
            }
            for (int i = 1; i < Round; i++) {
                GameObject mut = Instantiate(BlightMutation);
                AddBlightToRandomCell(mut);
            }
        }
    }

    private void AddBlightToRandomCell(GameObject enemy)
    {
        // Get a random tile without a blight
        WorldTile location = World.GetRandomTile();
        while (!World.IsFull<BasicBlight>() && World.GetObjectAtCell<BasicBlight>(location.tileCoord) != null)
        {
            location = World.GetRandomTile();
        }
        if (World.IsFull<BasicBlight>())
        {
            Destroy(enemy);
        }

        // If it has a duck KILL IT
        GameObject duck = World.GetObjectAtCell<BasicDuck>(location.tileCoord);
        if (duck != null)
        {
            World.RemoveDuckRing(location);
            duck.GetComponent<BasicDuck>().Kill();
        }

        // Add baby to the tile
        World.AddAtTile(enemy, location);
    }

    public void LoseGame()
    {
        Round = RoundMax + 1;
        Message.text = "You Lose!";
        Message.color = new Color(5.0f, 0.0f, 0.0f, 1.0f);
        gameEnd = true;
    }

    public void WinGame()
    {
        Message.text = "You Win!";
        Message.color = new Color(0.0f, 5.0f, 5.0f, 1.0f);
        gameEnd = true;
    }

    public void HoverTile(WorldTile caller) {

        ringMenuBasis = World.WithinDuckRing(caller);

        GameObject occupant = null;
        GameObject suds = null;
        BasicBlight blight = null;
        Vector2Int[] tileset = null;

        for (int i = 0; i < caller.transform.childCount; i++) {
            blight = blight == null ? caller.transform.GetChild(i).GetComponent<BasicBlight>() : blight;
        }

        if (blight != null) {
            blight.enabled = true;
        }

        occupant = World.GetObjectAtCell<BasicBlight>(caller.tileCoord);
        if (occupant == null) {
        occupant = World.GetObjectAtCell<BasicDuck>(caller.tileCoord);
        }

        if (!Input.GetMouseButton(0) && cursorMode > 0 && cursorMode < 7) {
            if (occupant == null) {
            tileset = World.CellNeighborhoodStripe(caller.tileCoord, GetDuckForMode(cursorMode).GetComponent<BasicDuck>().attackRange);
                foreach (Vector2Int cell in tileset) {
                    World.GetTile(cell).TileColor = new Color(1f, 0.25f, 0f, 1f);
                }
            tileset = World.CellNeighborhood(caller.tileCoord, GetDuckForMode(cursorMode).GetComponent<BasicDuck>().attackRange - 1);
                foreach (Vector2Int cell in tileset) {
                    World.GetTile(cell).TileColor = new Color(0f, 1f, 0.25f, 1f);
                }
            } else {
            tileset = World.CellNeighborhood(caller.tileCoord, GetDuckForMode(cursorMode).GetComponent<BasicDuck>().attackRange);
                foreach (Vector2Int cell in tileset) {
                    World.GetTile(cell).TileColor = new Color(1f, 0f, 0f, 1f);
                }
            }
        }

        if (Input.GetMouseButton(0) && cursorMode > 0 && cursorMode < 7 && occupant == null) {
            ClickTile(caller);
        }
    }

    public void ExitTile(WorldTile caller) {
        Vector2Int[] range = World.CellNeighborhood(caller.tileCoord, 5);
        foreach (Vector2Int cell in range) {
            World.GetTile(cell).TileColor = new Color(0f, 0f, 0f, 0f);
        }
        ringMenuBasis = null;
    }

    public void ClickTile(WorldTile caller)
    {   

        Vector2Int tile = caller.tileCoord;
        // Cursor mode is placing a duck
        if (cursorMode > 0 && cursorMode < 10)
        {
            if (World.GetObjectAtCell<BasicBlight>(tile) == null
                && World.GetObjectAtCell<BasicDuck>(tile) == null)
            {
                if (money >= GetCost(cursorMode))
                {
                    money -= GetCost(cursorMode);
                    World.AddAtCell(Instantiate(GetDuckForMode(cursorMode)), tile);
                    ringMenuBasis = World.CheckDuckRing(caller);
                    World.ResetDiscoveryChannels();
                }
            }
        }
        // Cursor mode is using a power
        else if (cursorMode > 10 && cursorMode < 15)
        {
            if (money >= GetCost(cursorMode))
            {
                money -= GetCost(cursorMode);
                World.AddAtCell(Instantiate(GetDuckForMode(cursorMode)), tile);
            }
        }
        // Cursor mode is cleaning
        else if (cursorMode == 0)
        {
            GameObject target = World.GetObjectAtCell<BasicBlight>(tile);
            if (target != null)
            {
                target.GetComponent<BasicBlight>().Damage(4.0f * Time.deltaTime);
                money += (Random.value < Time.deltaTime * 4.0f) ? 1:0;
            }
        }
        // duck remover
        else if (cursorMode == 15)
        {
            for (int i = 0; i < caller.transform.childCount; i++) {
                BasicDuck child = caller.transform.GetChild(i).GetComponent<BasicDuck>();
                if (child != null) {
                    money += (int)(0.5f * child.HP / child.MaxHealth * (float)GetCost(child.duckMode));
                    child.Kill();
                }
            }
        }

        Vector2Int[] range = World.CellNeighborhood(tile, 5);
        foreach (Vector2Int cell in range) {
            World.GetTile(cell).TileColor = new Color(0f, 0f, 0f, 0f);
        }

    }

    public void HeighlightRing() { // extremely dumb and complains constantly
        uniTime += Time.deltaTime;
        foreach (WorldTile toHighlight in ringMenuBasis) {
            World.GetObjectAtCell<BasicDuck>(toHighlight.tileCoord)
            .transform.GetComponent<SpriteRenderer>().color =
                new Vector4(1f, 1f, 1f, 0.5f + 0.25f * Mathf.Sin(6f * uniTime));
        }
    }

    public void HandleRingMenu() {
        if (Menu != null) {
            return;
        }

        Menu = Instantiate(RingMenu);
        Menu.transform.SetParent(CameraObject.transform);
        Menu.transform.GetComponent<MenuToggle>().Own(ringMenuBasis);
    }

    public void Upgrade() {
        if (unlocks > numDucks) {
            return;
        }
        for (int i = 1; i < Shop.transform.childCount; i++) {
            if (!Shop.transform.GetChild(i).GetComponent<Button>().interactable) {
                Shop.transform.GetChild(i).GetComponent<Button>().interactable = true;
                break;
            }
        }
        unlocks++;
    }

    public void DuckRingUpgrade(List<WorldTile> menuRing) {
        bool powerLevel = true;

        foreach (WorldTile iChild in menuRing) { // see if all are at unlocks power level
            BasicDuck child = World.GetObjectAtCell<BasicDuck>(iChild.tileCoord).GetComponent<BasicDuck>();
            powerLevel &= child.duckMode == unlocks - 1;
        }

        if (powerLevel) {
            foreach(WorldTile iChild in menuRing) { // delete all
                World.GetObjectAtCell<BasicDuck>(iChild.tileCoord).GetComponent<BasicDuck>().Kill();
            }
            World.AddAtTile(Instantiate(GetDuckForMode(unlocks)), menuRing[0]);
            World.RemoveDuckRing(menuRing[0]);
            ringMenuBasis = null;
            Upgrade();
        }
        Debug.Log("did stuff");
    }

    public void SetCursorMode(int mode)
    {
        // 0 = cleaner
        // 1 = place duck 1
        // 2 = place duck 2
        // 3 = place duck 3
        // 4 = place duck 4
        // 5 = place duck 5
        // 6 = place duck 6
        // 11 = use power 1
        // 12 = use power 2
        // 13 = use power 3
        // 14 super secret power or something
        // 15 = use duck collector

        cursorMode = mode % 20;
        Cursor.SetCursor(GetCursorForMode(cursorMode), Vector2.zero, CursorMode.Auto);
        if (cursorMode > 6) {
        selection = -1;
        } else {
            selection = cursorMode;
        }
        if (cursorMode == -2) {
            selection = -2;
        }
    }

    private Texture2D GetCursorForMode(int mode)
    {
        switch (mode)
        {
            case -2:
                return panCursor;
            case 0:
                return cleanerCursor;
            case 1:
                return basicDuckCursor;
            case 2:
                return armyDuckCursor;
            case 3:
                return ninjaDuckCursor;
            case 4:
                return superDuckCursor;
            case 5:
                return robotDuckCursor;
            case 6:
                return mythicDuckCursor;
            case 11:
                return bleachPowerCursor;
            case 12:
                return damagePowerCursor;
            case 13:
                return speedPowerCursor;
            case 14:
                return specialPowerCursor;
            case 15:
                return duckScooper;
            default:
                return cleanerCursor;
        }
    }

    public void ForceCursor() {
        Cursor.SetCursor(GetCursorForMode(cursorMode), Vector2.zero, CursorMode.Auto);
    }

    public void UnsetCursor() {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private int GetCost(int mode)
    {
        switch (mode)
        {
            case 0:
                return 0;
            case 1:
                return Duck1Cost;
            case 2:
                return Duck2Cost;
            case 3:
                return Duck3Cost;
            case 4:
                return Duck4Cost;
            case 5:
                return Duck5Cost;
            case 6:
                return Duck6Cost;
            case 11:
                return Power1Cost;
            case 12:
                return Power2Cost;
            case 13:
                return Power3Cost;
            case 14:
                return Power4Cost;
            default:
                return 0;
        }
    }

    private GameObject GetDuckForMode(int mode)
    {
        switch (mode)
        {
            case 0:
                return null;
            case 1:
                return Duck1;
            case 2:
                return Duck2;
            case 3:
                return Duck3;
            case 4:
                return Duck4;
            case 5:
                return Duck5;
            case 6:
                return Duck6;
            case 11:
                return Power1;
            case 12:
                return Power2;
            case 13:
                return Power3;
            case 14:
                return Power4;
            default:
                return null;
        }
    }

    public void ToggleTax() {
        borderCleanse = !borderCleanse;
    }

    public void MapFocus(int caller) {

        if (caller >= RegionNum || gameEnd) {
            regionIndex = -1;
            return;
        }

        regionIndex = caller;

        RegionZoomTimer = RegionZoomDuration;
        prevCamera = CameraObject.transform.localPosition;
        prevScale = transform.localScale;
        eventualCamera = cameraMove[caller];
        eventualScale = controllerScale[caller];
    }

    public void MapUnfocus() {
        regionIndex = -1;
        eventualCamera = cameraOrigin;
        eventualScale = scaleOrigin;
        prevCamera = CameraObject.transform.localPosition;
        prevScale = transform.localScale;
        RegionZoomTimer = RegionZoomDuration;
    }

    public void StartNextRound()
    {

        RoundTimer = RoundDurations[Round];
        Round += 1;
        DisplayRound();
        Upgrade();
        SpawnRound();
        RoundTMP.text = "" + Round;
        RoundTime.transform.localPosition = Vector3.zero;
        SkipButton.interactable = false;
    }
}
