using UnityEngine;

public class ControllerComm : MonoBehaviour
{

    public int region;

    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
    }

    public void OnMouseEnter() {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 1f);
    }


    public void OnMouseExit() {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
    }

    public void OnMouseDown() {
        FindAnyObjectByType<GameController>().MapFocus(region);
    }
}
