using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool readyDestroy;

    const float openTime = 0.3f;
    private float openTimer;
    private Vector3 eventual;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(0f, 0f, 1f);
        openTimer = 0f;
        readyDestroy = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (openTimer > openTime) {
            return;
        } else {
            openTimer += Time.deltaTime;
            transform.localScale = eventual * openTimer / openTime;
        }
    }

    public void Consolidate() {
        readyDestroy = true;
        return;
    }

    public void MoveRing() {
        readyDestroy = true;
        return;
    }

    public void OnMouseExit() {
        if (openTimer > openTime) {
        readyDestroy = true;
        }
    }

    public void Resize(int ringSize) {
        eventual = new Vector3(ringSize * 0.04f, ringSize * 0.04f, 1f);
    }
}
