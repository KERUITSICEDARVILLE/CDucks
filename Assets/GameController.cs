using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int Round;

    public GameObject BasicBlight;

    public WorldGrid World;

    public float RoundMessageDuration;
    private float RoundStartMessageTimer;

    public TMP_Text Message;

    void Start()
    {
        Round = 0;
        RoundStartMessageTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (World.EntityCount() == 0)
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
        if (World.IsFull())
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
            World.AddToRandomEmptyCell(enemy);
        }
    }

    public void LoseGame()
    {
        Message.text = "You Lose!";
        Message.color = new Color(5.0f, 0.0f, 0.0f, 1.0f);
    }

    public void ClickTile(Vector2Int tile)
    {

    }
}
