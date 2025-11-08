using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class WorldTile : MonoBehaviour
{
    [Header("Identity Information")]
    public Vector2Int tileCoord;
    public Color color;
    public Color heighlight;

    [Header("Discovery and Relevant Metadata")]
    public Vector2Int discoveryParentCoord;
    public int lengthToOrigin;
    public bool isDiscovered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = heighlight;
    }

    public void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void OnMouseDown()
    {
        FindAnyObjectByType<GameController>().ClickTile(this);
    }
}
