using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Next_Scene_Start_Button : MonoBehaviour
{
    // This method is called when the user clicks on the collider attached to this GameObject
    public void OnButtonClick()
    {
        SceneManager.LoadScene("GameScreen");
    }

}
