using UnityEngine;
using System.Collections.Generic;

public class DuckController : MonoBehaviour
{
    public int allowance;
    public WorldGrid World;
    private HashSet<GameObject> Subset;
    private float timer;
    const float timerMax = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = timerMax;
        allowance = 40;
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
            if (iChild.GetComponent<BasicDuck>().enabled) {
            live.Add(iChild);
            } else {
            dead.Add(iChild);                
            }
        }

        foreach (GameObject iChild in live) {
            if (World.CountAdjacentCellsWithoutType<BasicDuck>(
                iChild.transform.parent.GetComponent<WorldTile>().tileCoord
                ) == 0) {
                iChild.GetComponent<BasicDuck>().enabled = false;
            }
        } // kill within borders

        int needKill, needLive;
        int select;

        if (live.Count > allowance) {
            needKill = live.Count - allowance;
            while (needKill != 0) {
                select = Random.Range(0, live.Count);
                if (live[select].GetComponent<BasicDuck>().enabled) {
                    live[select].GetComponent<BasicDuck>().enabled = false;
                    needKill--;
                }
            }
        } else {
            needLive = allowance - live.Count;
            while (needLive != 0 && dead.Count > 0) {
                select = Random.Range(0, dead.Count);
                dead[select].GetComponent<BasicDuck>().enabled = true;
                needLive--;
            }
        } // bring total alive to allowance

        List<GameObject> shouldLive = new List<GameObject>();

        foreach (GameObject iChild in Subset) {
            if (iChild.GetComponent<BasicDuck>().enabled
                || World.CountAdjacentCellsWithoutType<BasicDuck>(iChild.transform.parent.GetComponent<WorldTile>().tileCoord) > 0) {
                shouldLive.Add(iChild);
            }
        } // select all enabled or at border cells

        for (int i = 0; i < allowance; i++) {
            // take one random
            select = Random.Range(0, shouldLive.Count);
            while (!shouldLive[select].GetComponent<BasicDuck>().enabled) {
                select = Random.Range(0, shouldLive.Count);
            }
            shouldLive[select].GetComponent<BasicDuck>().enabled = false;
            // give one random
            select = Random.Range(0, shouldLive.Count);
            while (shouldLive[select].GetComponent<BasicDuck>().enabled) {
                select = Random.Range(0, shouldLive.Count);
            }
            shouldLive[select].GetComponent<BasicDuck>().enabled = true;
        } // jumble all of the previously selected

    }

    public void Register(GameObject caller) {
        Subset.Add(caller);
    }

    public void Unregister(GameObject caller) {
        Subset.Remove(caller);
    }
}
