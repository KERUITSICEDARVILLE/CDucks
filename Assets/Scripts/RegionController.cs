using UnityEngine;
using System.Collections.Generic;

public struct regionInfo {
    public Vector2Int origin;
    public int unlockLevel;
    public int radius;
}

public class RegionController : MonoBehaviour
{
    public WorldGrid World;
    public ControllerComm[] Comms;
    public int regionTotal;

    public int regionState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        regionState = 0;

        int i0, i1;

        List<int>levels = new List<int>();

        for (int i = 0; i < regionTotal; i++) {
            levels.Add(i);
        }

        for (int i = 0; i < regionTotal; i++) {
            i0 = Random.Range(0, regionTotal);
            i1 = Random.Range(0, regionTotal);
            if (i0 != i1) {
                levels[i0] ^= levels[i1];
                levels[i1] ^= levels[i0];
                levels[i0] ^= levels[i1];
            }
        }

        for (int i = 0; i < regionTotal; i++) {
            Comms[i].regionData.unlockLevel = levels[i];
        }

        regionState = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
