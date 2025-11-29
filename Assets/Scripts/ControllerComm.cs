using UnityEngine;

public class ControllerComm : MonoBehaviour
{
    public GameController Controller;
    public int region;

    void Awake()
    {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
    }

    void Update() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && Controller.zoomPercent + scroll > 1f) {
            Controller.MapFocus(region);            
        }
    }

    public void OnMouseEnter() {
        if (Controller.Pause) {
            return;
        }
        this.enabled = true;
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 1f);
    }


    public void OnMouseExit() {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
        if (Controller.Pause) {
            return;
        }
        this.enabled = false;
    }

    public void OnMouseDown() {
        if (Controller.Pause) {
            return;
        }
        Controller.MapFocus(region);
    }
}
