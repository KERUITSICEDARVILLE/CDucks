using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject[] SubMenus;
    public GameObject Garland;
    public GameObject Back;
    public Vector2 eventualSize;
    public Vector2 startSize;
    public Vector3 startPosition;

    public float upDuration;
    public float upTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SubMenuActivate(-1);
        // turn off other components
        Back.GetComponent<RectTransform>().sizeDelta = startSize;
        Back.GetComponent<RectTransform>().localPosition = startPosition;
        upTimer = upDuration;
    }

    public void ToggleMenu(bool which) {
        SubMenuActivate(-1);
        // turn off other components
        Back.GetComponent<RectTransform>().sizeDelta = startSize;
        Back.GetComponent<RectTransform>().localPosition = startPosition;
        Garland.SetActive(false);
        if (which) {
            upTimer = upDuration;
            this.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float t = upTimer / upDuration;
        if (upTimer > 0f) {
            upTimer -= Time.deltaTime;
            Back.GetComponent<RectTransform>().sizeDelta = t * startSize + (1f - t) * eventualSize;
            Back.GetComponent<RectTransform>().localPosition = t * startPosition;
        } else {
            Garland.SetActive(true);
            SubMenuActivate(0);
            Back.GetComponent<RectTransform>().sizeDelta = eventualSize;
            Back.GetComponent<RectTransform>().localPosition = Vector3.zero;
            this.enabled = false;
        }
    }

    public void SubMenuActivate(int index) {
        foreach (GameObject SubMenu in SubMenus) {
            SubMenu.SetActive(false);
        }
        if (index == -1 || index >= SubMenus.Length) {
            return;
        }
        SubMenus[index].SetActive(true);
    }

}
