using UnityEngine;
using UnityEngine.EventSystems;

public class tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string msg;
    public int id;
    private GameController Controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Controller = FindAnyObjectByType<GameController>().GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        Controller.Wisdom(id, msg);
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        Controller.Wisdom(id, "");
    }
}
