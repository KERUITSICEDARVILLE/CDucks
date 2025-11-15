using TMPro;
using UnityEngine;

using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Game State")]
    public int Round;
    private int moneyAmount;
    public int money {
        set {
            moneyAmount = value;
            MoneyDisplay.text = "$" + moneyAmount;
        }
        get {
            return moneyAmount;
        }
    }

    public List<WorldTile> ringMenuBasis;

    public bool borderCleanse;
    public bool haveSwipePower;
    public int cursorMode;

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
    public GameObject Shop;
    public GameObject ShavedTangle;
    public GameObject RingMenu;
    private GameObject Menu;
    public WorldGrid World;
    public int selection;
    public int unlocks;
    public float uniTime;
    public float RoundMessageDuration;
    private float RoundStartMessageTimer;
    public TMP_Text Message;
    public TMP_Text MoneyDisplay;

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
    public Texture2D specialPowerCursor;

    void Start()
    {
        uniTime = 0f;
        Menu = null;
        unlocks = 2;
        selection = -1;
        Round = 0;
        moneyAmount = 0;
        ringMenuBasis = null;
        borderCleanse = false;
        haveSwipePower = false;
        RoundStartMessageTimer = 0;
        Cursor.SetCursor(GetCursorForMode(0), Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (ringMenuBasis != null) {
            HeighlightRing();
            HandleRingMenu();
            if (Random.Range(0f, 50f) < 29f) {
                return;
            }
        }
        if (ringMenuBasis == null && Menu != null) {
            MenuToggle eventScript = Menu.transform.GetComponent<MenuToggle>();
            if (eventScript.readyDestroy) {
                Destroy(Menu);
                Menu = null;
            }
        }
        if (World.EntityCount<BasicBlight>() == 0)
        {
            Round += 1;
            DisplayRound();
            SpawnRound();
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

        // live colorfully
        //GameObject.Find("PowerButton1").transform.GetChild(0).GetComponent<Image>().color = new Vector4(0f, 0f, 255f, 255f);
        // enforce selection is proper with visual

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f) {
            if (scroll > 0f) {
                selection = (selection + 1) % unlocks;
            } else {
                selection = selection < 1 ? unlocks - 1 : selection - 1;
            }
            SetCursorMode(selection);
        }

        
        for (int i = 0; i < Shop.transform.childCount; i++) {
            if (Shop.transform.GetChild(i).transform.childCount == 3 && i != selection) {
                Destroy(Shop.transform.GetChild(i).transform.GetChild(2).gameObject);
            }
            if (Shop.transform.GetChild(i).transform.childCount == 2 && i == selection) {
                GameObject tangle = Instantiate(ShavedTangle);
                tangle.transform.SetParent(Shop.transform.GetChild(i).transform);
                tangle.transform.localPosition = new Vector3(0f, 7.2f, 0f);
                tangle.transform.localScale = new Vector3(112f, 99.4f, 1f);
            }
        }

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
            Destroy(duck);
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
        selection = -1;

        ringMenuBasis = World.WithinDuckRing(caller);

        Power suds = null;

        for (int i = 0; i < caller.transform.childCount; i++) {
            suds = suds == null ? caller.transform.GetChild(i).GetComponent<Power>() : suds;
        }

        if ((suds != null && cursorMode == 0) || (Input.GetMouseButton(0) && cursorMode > 0)) {
            ClickTile(caller);
        }
    }

    public void ExitTile(WorldTile caller) {
        ringMenuBasis = null;
    }

    public void ClickTile(WorldTile caller)
    {   
        selection = -1;

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
        else if (cursorMode > 10 && cursorMode < 20)
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
        
    }

    public void HeighlightRing() {
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
        Menu.transform.SetParent(transform);
        Menu.transform.GetComponent<MenuToggle>().Own(ringMenuBasis);
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
        // 14 = use power 4
        Cursor.SetCursor(GetCursorForMode(mode), Vector2.zero, CursorMode.Auto);
        cursorMode = mode % 20;
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
            default:
                return cleanerCursor;
        }
    }

    public void ForceCursor() {
        Cursor.SetCursor(GetCursorForMode(cursorMode), Vector2.zero, CursorMode.Auto);
    }

    public void UnsetCursor() {
        return;
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
        switch (cursorMode)
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
        // cost for both turning on/off
        if (money >= 200) {
            money -= 200;
            borderCleanse = !borderCleanse;
        }
    }

}
