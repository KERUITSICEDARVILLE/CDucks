using TMPro;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public TMP_Text Action;
    public bool which;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle() {
        which = !which;
        Action.text = which ? "-" : "+";
        gameObject.SetActive(which);
    }
}
