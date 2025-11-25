using UnityEngine;

[ExecuteInEditMode]
public class WorldTile : MonoBehaviour
{
    [Header("Identity Information")]
    public Vector2Int tileCoord;
    public Color color;
    public Color pressed;
    public Color heighlight;
    public SpriteRenderer render;
    public Color TileColor {
        set {
            if (value == new Color(0f, 0f, 0f, 0f)) {
                render.color = color;
            } else {
                render.color = value;
            }
        }
        get {
            return render.color;
        }
    }
    public GameController Controller;

    [Header("Discovery and Relevant Metadata")]
    public Vector2Int discoveryParentCoord;
    public int lengthToOrigin;
    public bool isDiscovered;

    [Header("Waves")]
    public Vector3 initialTransform;

    void Start()
    {
        initialTransform = transform.localPosition;
        Controller = FindAnyObjectByType<GameController>();
        render = GetComponent<SpriteRenderer>();
        render.color = color;
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
