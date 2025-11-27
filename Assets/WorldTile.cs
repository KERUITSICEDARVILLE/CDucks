using UnityEngine;

[ExecuteInEditMode]
public class WorldTile : MonoBehaviour
{
    [Header("Identity Information")]
    public Vector2Int tileCoord;
    public Color color;
    public Color heighlight;
    public SpriteRenderer render;
    public GameController Controller;

    [Header("Discovery and Relevant Metadata")]
    public Vector2Int discoveryParentCoord;
    public int lengthToOrigin;
    public bool isDiscovered;

    [Header("Waves")]
    public Vector3 initialTransform;

    [Header("Makes things nicer to look at")]
    public float colorDuration;
    public float colorTimer;
    public Color prevColor;
    public Color eventualColor;
    public Color TileColor {
        set {
            prevColor = render.color;
            if (value == new Color(0f, 0f, 0f, 0f)) {
                eventualColor = color;
            } else {
                eventualColor = value;
            }
            colorTimer = colorDuration;
            this.enabled = true;
        }
        get {
            return render.color;
        }
    }

    void Awake()
    {
        initialTransform = transform.localPosition;
        Controller = FindAnyObjectByType<GameController>();
        render = GetComponent<SpriteRenderer>();
        render.color = color;
        colorTimer = 0f;
    }

    void Update() { // fear not for this is (almost) never touched
        if (!Application.isPlaying) {
            return;
        }
        //Debug.Log("first time?");
        //Debug.Break();
        float t;
        float decay = 0f;
        if (colorTimer > 0f) {
            if (colorTimer / colorDuration < 0.75f) {
                decay = (Input.mousePositionDelta).magnitude * Time.deltaTime;
            }
            t = colorTimer / colorDuration;
            render.color = (1 - t) * eventualColor + t * prevColor;
            colorTimer -= Time.deltaTime + decay;
        } else {
            render.color = eventualColor;
            this.enabled = false;
        }
    }

    public void OnMouseEnter()
    {
        render.color = heighlight;
        Controller.HoverTile(this);
    }

    public void OnMouseExit()
    {
        render.color = color;
        Controller.ExitTile(this);
    }

    public void OnMouseDown()
    {
        Controller.ClickTile(this);
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButton(0)) {
            Controller.ClickTile(this);
        }
    }

}
