using TMPro;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UI;

using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Game State")]
    public int Round;
    private int moneyAmount;
    public int money
    {
        set
        {
            moneyAmount = value;
            MoneyDisplay.text = "$" + moneyAmount;
        }
        get
        {
            return moneyAmount;
        }
    }
    private int cursorMode;

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
    public WorldGrid World;
    public float RoundMessageDuration;
    private float RoundStartMessageTimer;
    public TMP_Text Message;
    public TMP_Text MoneyDisplay;
    public Texture2D[] cursorGlyphs = new Texture2D[20];

    void Start()
    {
        Round = 0;
        RoundStartMessageTimer = 0;
        Cursor.SetCursor(cursorGlyphs[0], Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
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

        // If it has a duck KILL IT
        GameObject duck = World.GetObjectAtCell<BasicDuck>(location.tileCoord);
        if (duck != null)
        {
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

    public void ClickTile(WorldTile caller)
    {   
        Vector2Int tile = caller.tileCoord;
        // Cursor mode is placing a duck
        if (cursorMode > 0 && cursorMode < 10)
        {
            if (World.GetObjectAtCell<GameObject>(tile) == null)
            {
                if (money >= GetCost(cursorMode))
                {
                    money -= GetCost(cursorMode);
                    World.AddAtCell(Instantiate(GetDuckForMode(cursorMode)), tile);
                    World.CheckDuckRing(caller);
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
                World.CheckDuckRing(caller);
                World.ResetDiscoveryChannels();
            }
        }
        // Cursor mode is cleaning
        else if (cursorMode == 0)
        {
            GameObject target = World.GetObjectAtCell<BasicBlight>(tile);
            if (target != null)
            {
                target.GetComponent<BasicBlight>().Damage(1.0f);
            }
        }
        
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
        Cursor.SetCursor(cursorGlyphs[mode % 20], Vector2.zero, CursorMode.Auto);
        cursorMode = mode;
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


}
