using UnityEngine;

public class BasicDuck : MonoBehaviour
{
    public int duckMode;

    public DuckController Controller;
    public WorldGrid World;

    private bool eventKill;
    public GameObject zzz;
    public GameObject HPbar;
    public int attackRange;
    public float MaxHealth;
    private float healthPool;
    public float HP {
        set {
            healthPool = value;
            HPbar.transform.localScale = new Vector3(
                HPbar.transform.localScale.x,
                healthPool / MaxHealth * 0.5f,
                HPbar.transform.localScale.z);
        }
        get {
            return healthPool;
        }
    }
    public float power;
    public float speed;
    private float cooldown;

    public Vector2Int cell
    {
        get { return transform.parent.GetComponent<WorldTile>().tileCoord; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackRange = 3;
        transform.localScale = new Vector3(3.2f, 3.2f, 3.2f);
        HP = MaxHealth;
        if (transform.parent != null) {
            if (transform.parent.parent != null) {
                World = transform.parent.parent.GetComponent<WorldGrid>();
            }
        }
        Controller = GameObject.FindAnyObjectByType<DuckController>().GetComponent<DuckController>();
        Controller.Register(gameObject);
        eventKill = false;
        cooldown = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        BasicBlight victim;

        if (eventKill) {
            Controller.Unregister(gameObject);
            Destroy(gameObject);
        }
        if (cooldown > 0f) {
            cooldown -= speed * Time.deltaTime;
        }

        if (cooldown < 0f && World.CountAdjacentCellRangeWithType<BasicBlight>(cell, attackRange) > 0)
        {
            WorldTile target = World.GetRandomAdjacentTileRangeWithType<BasicBlight>(cell, attackRange);
            victim = World.GetObjectAtCell<BasicBlight>(target.tileCoord).GetComponent<BasicBlight>();
            victim.enabled = true;
            victim.Damage(power);
            Damage(0.25f * (MaxHealth + healthPool * Random.Range(0f, 0.1f)) / power);
            FindAnyObjectByType<GameController>().money += (int) (power * 1.5);
            cooldown = 1.0f;
        }
        if (HP < MaxHealth && World.CountAdjacentCellsWithType<BasicBlight>(cell) == 0) {
            HP += 0.00625f * power * Time.deltaTime;
        }
    }

    public bool Damage(float amount) {
        this.enabled = true; // force response
        HP -= amount;
        if (HP < 0f) {
            Kill();
        }
        return HP < 0f;
    }
 
    public void Kill() {
        this.enabled = true;
        eventKill = true;
    }

    public void Sleep() {
        this.enabled = false;
        zzz.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Wake() {
        this.enabled = true;
        zzz.GetComponent<SpriteRenderer>().enabled = false;
    }
}
