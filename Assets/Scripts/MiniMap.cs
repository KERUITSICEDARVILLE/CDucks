using TMPro;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public TMP_Text Action;
    public bool which;

    public void Toggle() {
        which = !which;
        Action.text = which ? "-" : "+";
        gameObject.SetActive(which);
    }
}
