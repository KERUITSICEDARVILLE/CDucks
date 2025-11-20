using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.LightTransport;

using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Game State")]
    public GameObject CameraObject;
    public Vector3 cameraOrigin;
    public Vector3 scaleOrigin;
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
    public bool haveSwipePower;
    public int cursorMode;

    [Header("Map Region State")]
    public GameObject[] Regions;
    public Vector3[] cameraMove;
    public Vector3[] controllerScale;
    private int regionIndex;
    private Vector3 prevCamera;
    private Vector3 prevScale;
    private Vector3 eventualCamera;
    private Vector3 eventualScale;

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
    public GameObject BasicBlight;

    [Header("Scene Setup")]
    private GameObject Menu;
    public GameObject UI;
    public GameObject Shop;
    public GameObject RingMenu;
    public WorldGrid World;
    private int selection;
    public int unlocks;
    private float uniTime;
    public float RegionZoomDuration;
    public float RoundMessageDuration;
    private float RegionZoomTimer;
    public float RoundDuration;
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

    void Start()
    {
        uniTime = 0f;
        Menu = null;
        unlocks = 2;
        Round = 0;
        money = 0;
        selection = -1;
        regionIndex = -1;
        ringMenuBasis = null;
        borderCleanse = false;
        haveSwipePower = false;
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

        // Animate zoom
        float t;
        if (RegionZoomTimer > 0) {
            t = RegionZoomTimer / RegionZoomDuration;
            CameraObject.transform.localPosition = (1 - t) * eventualCamera + t * prevCamera;
            transform.localScale = (1 - t) * eventualScale + t * prevScale;
            RegionZoomTimer -= Time.deltaTime;
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
        if (RoundTimer <= 0f)
        {
            StartNextRound();
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
        if (World.IsFull<BasicBlight>())
        {
            LoseGame();
        }

        // scuffed old system inputs
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f) {
            if (scroll > 0f) {
                selection = (selection + 1) % unlocks;
            } else {
                selection = selection < 1 ? unlocks - 1 : selection - 1;
            }
            SetCursorMode(selection);
        }

        if (Input.GetMouseButton(1)) {
            regionIndex = -1;
            eventualCamera = cameraOrigin;
            eventualScale = scaleOrigin;
            prevCamera = CameraObject.transform.localPosition;
            prevScale = transform.localScale;
            RegionZoomTimer = RegionZoomDuration;
        }
        
        if (Input.GetKeyDown("escape")) {
            UI.GetComponent<Canvas>().enabled = !UI.GetComponent<Canvas>().enabled;
        }

        if (Input.GetMouseButton(2) && regionIndex != -1) {
            Vector3 perPixel =  ( Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)) -
                                Camera.main.ScreenToWorldPoint(new Vector3(64, 0, 0)) );
            CameraObject.transform.localPosition += Input.mousePositionDelta * perPixel.x / 32f;
        }
        // end scuffed old system inputs

        // this nonsense should only change upon setcursor requests really.
        Vector3 tangleDelta = selection == -1 ? new Vector3(500f, 0f, 0f) : new Vector3(0f, 14f, 0f);
        Vector3 tanglePos = Shop.transform.GetChild(selection + 1).transform.localPosition;
        GameObject tangle = Shop.transform.GetChild(0).gameObject;
        tangle.transform.localPosition = tangleDelta + tanglePos;

    }

    private void DisplayRound()
    {
        Message.text = "Round " + Round;
        RoundStartMessageTimer = RoundMessageDuration;
        Message.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void SpawnRound()
    {
        int EnemyCount = 1 + 2 * Round + Round * Round / 5;
        for (int i = 0; i < EnemyCount; i++) {
            GameObject enemy = Instantiate(BasicBlight);
            AddBlightToRandomCell(enemy);
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
        Message.text = "You Lose!";
        Message.color = new Color(5.0f, 0.0f, 0.0f, 1.0f);
    }

    public void HoverTile(WorldTile caller) {

        ringMenuBasis = World.WithinDuckRing(caller);

        GameObject suds = null;
        BasicBlight blight = null;

        for (int i = 0; i < caller.transform.childCount; i++) {
            blight = blight == null ? caller.transform.GetChild(i).GetComponent<BasicBlight>() : blight;
        }

        if (blight != null) {
            blight.enabled = true;
        }

        if ((suds != null && cursorMode == 0) || (Input.GetMouseButton(0) && cursorMode > 0) || cursorMode == 14) {
            ClickTile(caller);
        }
    }

    public void ExitTile(WorldTile caller) {
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
                target.GetComponent<BasicBlight>().Damage(1.0f);
                money += 1;
            }
        }
        // duck remover
        else if (cursorMode == 15)
        {
            for (int i = 0; i < caller.transform.childCount; i++) {
                BasicDuck child = caller.transform.GetChild(i).GetComponent<BasicDuck>();
                if (child != null) {
                    money += (int)(child.HP * child.power * 5f);
                    child.Kill();
                }
            }
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

    public void Upgrade(List<WorldTile> menuRing) {
        bool powerLevel = true;

        foreach (WorldTile iChild in menuRing) { // see if all are at unlocks power level
            BasicDuck child = World.GetObjectAtCell<BasicDuck>(iChild.tileCoord).GetComponent<BasicDuck>();
            powerLevel &= child.power == GetDuckForMode(unlocks - 1).GetComponent<BasicDuck>().power;
        }

        if (powerLevel) {
            foreach(WorldTile iChild in menuRing) { // delete
                World.GetObjectAtCell<BasicDuck>(iChild.tileCoord).GetComponent<BasicDuck>().Kill();
            }
            World.AddAtTile(Instantiate(GetDuckForMode(unlocks)), menuRing[0]);
            World.RemoveDuckRing(menuRing[0]);
            ringMenuBasis = null;
            for (int i = 0; i < Shop.transform.childCount; i++) {
                if (!Shop.transform.GetChild(i).GetComponent<Button>().interactable) {
                    Shop.transform.GetChild(i).GetComponent<Button>().interactable = true;
                    break;
                }
            }
            unlocks++;
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
        Cursor.SetCursor(GetCursorForMode(mode), Vector2.zero, CursorMode.Auto);
        cursorMode = mode % 20;
        if (cursorMode > 5) {
        selection = -1;
        }
    }

    private Texture2D GetCursorForMode(int mode)
    {
        switch (mode)
        {
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
        switch (cursorMode)
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

    public void MapFocus(GameObject caller) {

        for (regionIndex = 0; regionIndex < Regions.Length; regionIndex++) {
            if (Regions[regionIndex] == caller) {
                break;
            }
        }
        if (regionIndex == Regions.Length) {
            regionIndex = -1;
            return;
        }

        RegionZoomTimer = RegionZoomDuration;
        prevCamera = CameraObject.transform.localPosition;
        prevScale = transform.localScale;
        eventualCamera = cameraMove[regionIndex];
        eventualScale = controllerScale[regionIndex];
    }

    public void StartNextRound()
    {
        Round += 1;
        DisplayRound();
        SpawnRound();
        RoundTMP.text = "" + Round;
        RoundTimer = RoundDuration;
        RoundTime.transform.localPosition = Vector3.zero;
        SkipButton.interactable = false;
    }
}
