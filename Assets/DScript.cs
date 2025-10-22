using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DScript : MonoBehaviour
{
    /*public float[] lerp(float x1, float y1, float x2, float y2, float t) {
    float []out = new float[2];
    out[0] = (1 - t) * x1 + t * x2;
    out[1] = (1 - t) * y1 + t * y2;
    return out;
    }

    public float[] bezier(float[] x, float[] y, int order, float t) {
      float[] basex = new float[order];
      float[] basey = new float[order];
      for (int i = 0; i < order; i++) {
      basex[i] = x[i];
      basey[i] = y[i];
      }
      for (int i = order; i; i--) {
        for (int j = i; j; j--) {
        
        }
      }
    }*/

    private int curOrder;
    private float[] cxPoints;
    private float[] cyPoints;
    private float t = 0f;

    // Start is called before the first frame update
    void Start()
    {
        curOrder = (int)Random.Range(0f, 15f);
        cxPoints = new float[curOrder];
        cyPoints = new float[curOrder];
          for (int i = 0; i < curOrder; i++) {
          cxPoints[i] = Random.Range(-12f, 12f);
          cyPoints[i] = Random.Range(-5f, 5f);
          }
    }

    // Update is called once per frame
    void Update()
    {
        if (t < 1f) {
        t += 0.01f;
        } else {
        t = 0f;
        }
        if (Random.Range(0f, 1f) < 0.001) {
        curOrder = (int)Random.Range(0f, 15f);
        cxPoints = new float[curOrder];
        cyPoints = new float[curOrder];
          for (int i = 0; i < curOrder; i++) {
          cxPoints[i] = Random.Range(-12f, 12f);
          cyPoints[i] = Random.Range(-5f, 5f);
          }
        }

        

        transform.eulerAngles += new Vector3(0f, 0f, 0.1f);

    }
}
