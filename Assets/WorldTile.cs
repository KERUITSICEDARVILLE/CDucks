using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class WorldTile : MonoBehaviour
{
    public Vector2Int tileCoord;
    public Color color;
    public Color heighlight;

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
        FindAnyObjectByType<GameController>().ClickTile(tileCoord);
    }
}
