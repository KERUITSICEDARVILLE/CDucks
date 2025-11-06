using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Camera mainCameraForPosition;

    [SerializeField]
    private float maxSpeed = 60f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCameraForPosition = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouseLag();
    }

    //Legacy function to snap mouse to position. This is flat and boring
    private void FollowMouseInstant()
    {
        transform.position = GetMousePosition();
    }

    private void FollowMouseLag()
    {
        transform.position = Vector2.MoveTowards(transform.position, GetMousePosition(),
            maxSpeed * Time.deltaTime);
    }
    
    private Vector2 GetMousePosition()
    {
        return mainCameraForPosition.ScreenToWorldPoint(Input.mousePosition);
    }
}
