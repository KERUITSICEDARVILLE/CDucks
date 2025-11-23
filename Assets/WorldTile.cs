using UnityEngine;

[ExecuteInEditMode]
public class WorldTile : MonoBehaviour
{
    [Header("Identity Information")]
    public Vector2Int tileCoord;
    public Color color;
    public Color pressed;
    public Color heighlight;
    public bool isBeingPressed;

    [Header("Discovery and Relevant Metadata")]
    public Vector2Int discoveryParentCoord;
    public int lengthToOrigin;
    public bool isDiscovered;

    [Header("Waves")]
    public Vector3 initialTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialTransform = transform.localPosition;
        GetComponent<SpriteRenderer>().color = color;
    }

    // Update is called once per frame

    public void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = heighlight;
        FindAnyObjectByType<GameController>().HoverTile(this);
    }

    public void OnMouseExit()
    {
        isBeingPressed = false;
        GetComponent<SpriteRenderer>().color = color;
        FindAnyObjectByType<GameController>().ExitTile(this);
    }

    public void OnMouseDown()
    {
        isBeingPressed = true;
        //GetComponent<SpriteRenderer>().color = pressed;
        FindAnyObjectByType<GameController>().ClickTile(this);
    }

    public void OnMouseUp() {
        isBeingPressed = false;
        //GetComponent<SpriteRenderer>().color = color;
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            FindAnyObjectByType<GameController>().ClickTile(this);
        }
    }
}
