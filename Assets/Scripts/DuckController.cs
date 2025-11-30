using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DuckController : MonoBehaviour
{
    public int allowance;
    public WorldGrid World;
    public TMP_Text DuckCount;
    private HashSet<GameObject> Subset;
    private float timer;
    const float timerMax = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        timer = timerMax;
        Subset = new HashSet<GameObject>();
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
            if (iChild.GetComponent<BasicDuck>().shouldWake) {
            live.Add(iChild);
            } else {
            dead.Add(iChild);                
            }
        }

        foreach (GameObject iChild in live) {
            if (World.CountAdjacentCellsWithoutType<BasicDuck>(
                iChild.transform.parent.GetComponent<WorldTile>().tileCoord
                ) == 0) {
                iChild.GetComponent<BasicDuck>().Sleep();
            }
        } // kill within borders

        int needKill, needLive;
        int select;

        if (live.Count > allowance) {
            needKill = live.Count - allowance;
            while (needKill != 0) {
                select = Random.Range(0, live.Count);
                if (live[select].GetComponent<BasicDuck>().shouldWake) {
                    live[select].GetComponent<BasicDuck>().Sleep();
                    needKill--;
                }
            }
        } else {
            needLive = allowance - live.Count;
            while (needLive != 0 && dead.Count > 0) {
                select = Random.Range(0, dead.Count);
                dead[select].GetComponent<BasicDuck>().Wake();
                needLive--;
            }
        } // bring total alive to allowance

        List<GameObject> shouldLive = new List<GameObject>();

        bool shouldLiveAllActive = true;

        // potentially prioritize ducks with low health even higher
        foreach (GameObject iChild in Subset) {
            if (iChild.GetComponent<BasicDuck>().shouldWake
                || World.CountAdjacentCellsWithType<BasicBlight>(iChild.transform.parent.GetComponent<WorldTile>().tileCoord) > 0) {
                shouldLiveAllActive &= iChild.GetComponent<BasicDuck>().shouldWake;
                shouldLive.Add(iChild);
            }
        } // select all enabled or at border cells

        if (shouldLive.Count == 0 || shouldLiveAllActive) {
            return;
        }

        // While the below code exists in exactly the same way, and does exactly the
        // same thing in BlightController, it here brings the entire game to its knees
        // I have spent hours trying to figure out exactly why.

        /*for (int i = 0; i < allowance; i++) {
            // take one random
            select = Random.Range(0, shouldLive.Count);
            while (!shouldLive[select].GetComponent<BasicDuck>().shouldWake) {
                select = Random.Range(0, shouldLive.Count);
            }
            shouldLive[select].GetComponent<BasicDuck>().Sleep();
            // give one random
            select = Random.Range(0, shouldLive.Count);
            while (shouldLive[select].GetComponent<BasicDuck>().shouldWake) {
                select = Random.Range(0, shouldLive.Count);
            }
            shouldLive[select].GetComponent<BasicDuck>().Wake();
        } // jumble all of the previously selected*/

    }

    public void Register(GameObject caller) {
        // do duck ring check and whatnot
        Subset.Add(caller);
        DuckCount.text = "" + Subset.Count;
    }

    public void Unregister(GameObject caller) {
        Subset.Remove(caller);
        DuckCount.text = "" + Subset.Count;
    }

    public void Nuke() {
        foreach (GameObject iChild in Subset) {
            Destroy(iChild);
        }
        Subset = new HashSet<GameObject>();
    }
    
}
