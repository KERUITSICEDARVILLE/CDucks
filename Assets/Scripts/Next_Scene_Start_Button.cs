using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class Next_Scene_Start_Button : MonoBehaviour
{

    public GameObject MaskingBlocker;
    public float timeMax;
    private float timer;

    public bool isClicked = false;

    // This method is called when the user clicks on the collider attached to this GameObject
    /*IEnumerator WaitAndDoSomething(float secondsToWait)
            {
                yield return new WaitForSeconds(secondsToWait); // This line pauses execution

                SceneManager.LoadScene("GameScreen");
            }

    */

    void Update()
    {
        if (!isClicked) 
        {
            return;
        }

        if (timer < timeMax)
        {
            timer += Time.deltaTime;
            
            MaskingBlocker.GetComponent<Image>().color = new Color(1f, 1f, 1f, timer / timeMax);
            return;
        }

        SceneManager.LoadScene("GameScreen");
    }
    public void OnButtonClick()
    {
        //StartCoroutine(WaitAndDoSomething(0.5f));

        isClicked = true;

        timer = 0f;
        MaskingBlocker.SetActive(true);
    }

}
