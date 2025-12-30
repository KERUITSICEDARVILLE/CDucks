using UnityEngine;

public class ControllerComm : MonoBehaviour
{
    public RegionController rController;
    public GameController Controller;
    public regionInfo regionData;
    public int region;

    public Vector2Int origin;
    public int radius;

    void Awake()
    {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
        regionData.origin = origin;
        regionData.radius = radius;
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
        // trigger pulse in Controller
        // this will look like updating each tiles random green color value
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
