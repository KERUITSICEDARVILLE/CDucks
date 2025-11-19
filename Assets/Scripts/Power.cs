using UnityEngine;

public class Power : MonoBehaviour
{
    const float survivalMax = 10f;
    private float survivalTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        survivalTimer = survivalMax;
    }

    // Update is called once per frame
    void Update()
    {

        if (survivalTimer > 0f) {
            survivalTimer -= Time.deltaTime;
            transform.localScale =
                new Vector3(0.5f + 0.5f * survivalTimer / survivalMax,
                            0.5f + 0.5f * survivalTimer / survivalMax, 1);
        } else {
            Destroy(gameObject);
        }
    }
}
