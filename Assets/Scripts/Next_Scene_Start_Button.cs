using TMPro;
using UnityEngine.UI;

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;

public class Next_Scene_Start_Button : MonoBehaviour
{
    public Image Block;
    public float TimerMax;
    private float Timer;

    void Start() {
        Timer = 0;
    }
    // This method is called when the user clicks on the collider attached to this GameObject
    IEnumerator WaitAndDoSomething(float secondsToWait)
    {
        if (Timer < TimerMax) {
        Timer += Time.deltaTime;
        Block.color = new Color(1f, 1f, 1f, Timer/TimerMax);
        }
        yield return new WaitForSeconds(secondsToWait); // This line pauses execution
        SceneManager.LoadScene("GameScreen");
    }


    public void OnButtonClick()
    {
        StartCoroutine(WaitAndDoSomething(TimerMax));
    }

}
