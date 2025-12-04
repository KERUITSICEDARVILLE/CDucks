using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Next_Scene_Start_Button : MonoBehaviour
{
    // This method is called when the user clicks on the collider attached to this GameObject
IEnumerator WaitAndDoSomething(float secondsToWait)
        {
            yield return new WaitForSeconds(secondsToWait); // This line pauses execution

            SceneManager.LoadScene("GameScreen");
        }


    public void OnButtonClick()
    {
        StartCoroutine(WaitAndDoSomething(0.5f));
    }

}
