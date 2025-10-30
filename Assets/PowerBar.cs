using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public Image fillBar;
    public float PowerLevel
    {
        get {
            return PowerLevel;
        }
        set {
            fillBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
