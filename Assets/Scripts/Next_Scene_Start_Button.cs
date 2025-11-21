using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Next_Scene_Start_Button : MonoBehaviour
{
    // This method is called when the user clicks on the collider attached to this GameObject
    private void OnMouseDown()
    {
        SceneManager.LoadScene("GameScreen");
    }

    private void Update()
    {
 
    } 

    private void OnMouseOver()
        {
            transform.localPosition = new Vector3(0f, 0f, -(float)((Math.Sin(Time.timeAsDouble) + 1) * 3.0));
        }

    private void OnMouseEnter() 
    {
        // Change the cursor to a hand when hovering over the button
        //transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2.0f, 2.0f, 2.0f), Time.deltaTime * 100);
        //transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

     private void OnMouseExit()
    {
        // Reset the scale back to normal when the mouse is no longer over the button
        //transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * 100);

    //transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
    }

}
