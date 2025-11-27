using UnityEngine;

public class ControllerComm : MonoBehaviour
{

    public int region;

    void Awake()
    {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
    }

    void Update() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            FindAnyObjectByType<GameController>().MapFocus(region);
        }
    }

    public void OnMouseEnter() {
        this.enabled = true;
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 1f);
    }


    public void OnMouseExit() {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
        this.enabled = false;
    }

    public void OnMouseDown() {
        FindAnyObjectByType<GameController>().MapFocus(region);
    }
}
