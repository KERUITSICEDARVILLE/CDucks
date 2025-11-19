using UnityEngine;

public class ControllerComm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // start transparent
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
    }

    public void OnMouseEnter() {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 1f);
    }


    public void OnMouseExit() {
        GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, 0f);
    }

    // Update is called once per frame
    public void OnMouseDown() {
//        transform.localPosition += new Vector3(0f, 0f, 5f);
        FindAnyObjectByType<GameController>().MapFocus(gameObject);
    }
}
