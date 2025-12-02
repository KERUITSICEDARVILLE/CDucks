using TMPro;
using UnityEngine;
using System.Collections.Generic;

// TODO: give priority to blight with higher growth rates

public class BlightController : MonoBehaviour
{
    public int allowance;
    public int terrorAllowance;
    public WorldGrid World;
    public TMP_Text AlgaeCount;
    private HashSet<GameObject> Subset;
    private HashSet<GameObject> Mutations;
    private HashSet<int> idSet; 
    private float timer;
    const float timerMax = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        timer = timerMax;
        Subset = new HashSet<GameObject>();
        Mutations = new HashSet<GameObject>();
        idSet = new HashSet<int>();
    }

    void Update()
    {
        if (timer > 0) {
            timer -= Time.deltaTime;
            return;
        } else {
            timer = timerMax;
        }
        List<GameObject> live = new List<GameObject>();
        List<GameObject> dead = new List<GameObject>();

        foreach (GameObject iChild in Subset) {
            if (iChild.GetComponent<BasicBlight>().shouldWake) { // changed
            live.Add(iChild);
            } else {
            dead.Add(iChild);                
            }
        }

        foreach (GameObject iChild in live) {
            if (World.CountAdjacentCellsWithoutType<BasicBlight>(
                iChild.transform.parent.GetComponent<WorldTile>().tileCoord
                ) == 0) {
                iChild.GetComponent<BasicBlight>().Sleep();
            }
        } // kill within borders

        int needKill, needLive;
        int select;

        if (live.Count > allowance) {
            needKill = live.Count - allowance;
            while (needKill != 0) {
                select = Random.Range(0, live.Count);
                if (live[select].GetComponent<BasicBlight>().shouldWake) {
                    live[select].GetComponent<BasicBlight>().Sleep();
                    needKill--;
                }
            }
        } else {
            needLive = allowance - live.Count;
            while (needLive != 0 && dead.Count > 0) {
                select = Random.Range(0, dead.Count);
                dead[select].GetComponent<BasicBlight>().Wake();
                needLive--;
            }
        } // bring total alive to allowance

        List<GameObject> shouldLive = new List<GameObject>();

        bool shouldLiveAllActive = true;

        int maxLineage = 0;
        float maxGrowth = 0.5f;

        BasicBlight iterate;

        foreach (GameObject iChild in Subset) {
            iterate = iChild.GetComponent<BasicBlight>();
            if (iterate.shouldWake
                || World.CountAdjacentCellsWithoutType<BasicBlight>(iChild.transform.parent.GetComponent<WorldTile>().tileCoord) > 0) {
                maxGrowth = maxGrowth > iterate.GrowthRate ? maxGrowth : iterate.GrowthRate;
                maxLineage = maxGrowth > iterate.GrowthRate ? maxLineage : iterate.Lineage;
                shouldLiveAllActive &= iterate.shouldWake; // changed
                shouldLive.Add(iChild);
            }
        } // select all enabled or at border cells

        if (shouldLiveAllActive) {
            return;
        }

        if (shouldLive.Count > 0)
        {
            for (int i = 0; i < allowance; i++)
            {
                // take one random
                select = Random.Range(0, shouldLive.Count);
                while (!shouldLive[select].GetComponent<BasicBlight>().shouldWake)
                {
                    select = Random.Range(0, shouldLive.Count);
                }
                shouldLive[select].GetComponent<BasicBlight>().Sleep();
                // give one random
                select = Random.Range(0, shouldLive.Count);
                while (shouldLive[select].GetComponent<BasicBlight>().shouldWake)
                {
                    select = Random.Range(0, shouldLive.Count);
                }
                shouldLive[select].GetComponent<BasicBlight>().Wake();
            } // jumble all of the previously selected
        }

        int terror = terrorAllowance;

        foreach (GameObject iChild in shouldLive) {
            iterate = iChild.GetComponent<BasicBlight>();
            if (iterate.Lineage == maxLineage && !iterate.shouldWake && terror > 0) {
                iterate.Wake();
                terror--;
            }
        }

    }

    // need to lock down these functions to only be accessible from one blight mutation
    public void Register(GameObject caller) {
        Subset.Add(caller);
        AlgaeCount.text = "" + Subset.Count;
    }

    public int GiveMeUniqueID() {
        int selection = Random.Range(0, 256);
        while (idSet.Contains(selection)) {
            selection = Random.Range(0, 256);
        }
        idSet.Add(selection);
        return selection;
    }

    public void RegisterMutation(GameObject caller) {
        Mutations.Add(caller);
    }

    public void UnregisterMutation(GameObject caller) {
        Mutations.Remove(caller);
    }    

    public void Unregister(GameObject caller) {
        Subset.Remove(caller);
        AlgaeCount.text = "" + Subset.Count;
    }

    public GameObject GrabRandomBlight() {
        List<GameObject>flat = new List<GameObject>(Subset);
        return flat[Random.Range(0, flat.Count)]; 
    }

    public void GiveTarget(GameObject subject) {
        BlightMutation mut = subject.GetComponent<BlightMutation>();
        if (mut == null) {
            return;
        }
        WorldTile stop = GrabRandomBlight().transform.parent.GetComponent<WorldTile>();
        WorldTile endpt = World.BFSstopstart<BasicDuck>(stop, World.GetTile(mut.cell), true, 0);
        List<WorldTile> path = World.Gather(endpt);
        if (path == null) {
            Destroy(mut.gameObject);
            return;
        }
        path.Reverse();
        path.Add(stop);
        mut.Path = path;
        mut.TargetTile = endpt;
        World.ResetDiscoveryChannels();
    }

    public void RetargetNear(BlightMutation caller, WorldTile tile) {
        //
    }

    public void Nuke() {
        foreach (GameObject iChild in Subset) {
            Destroy(iChild);
        }
        Subset = new HashSet<GameObject>();
        foreach (GameObject iChild in Mutations) {
            Destroy(iChild);
        }
        Mutations = new HashSet<GameObject>();
    }

    public bool isFull() {
        return Subset.Count > 999;
    }

    public bool bossCriteria() {
        return Subset.Count < 500;
        // are there blight within range of end tiles?
    }

}
