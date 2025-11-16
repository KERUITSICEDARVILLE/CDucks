using UnityEngine;
using System.Collections.Generic;

public class MenuToggle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool readyDestroy;

    const float openTime = 0.3f;
    private float openTimer;
    private float deathTimer;
    private Vector3 eventual;
    public List<WorldTile> ownRing;
    public bool lockPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(0f, 0f, 1f);
        openTimer = 0f;
        deathTimer = openTime;
        readyDestroy = false;
        lockPosition = false;
    }

    // Update is called once per frame
    void Update()
    {
        PositionMenu();
        if (ownRing != FindAnyObjectByType<GameController>().ringMenuBasis && !lockPosition) {
            deathTimer -= Time.deltaTime;
            if (deathTimer < 0f) {
                readyDestroy = true;
            }
        } else {
            deathTimer = openTimer;
        }
        if (openTimer > openTime) {
            return;
        } else {
            openTimer += Time.deltaTime;
            transform.localScale = eventual * openTimer / openTime;
        }
    }

    public void Consolidate() {
        FindAnyObjectByType<GameController>().Upgrade(ownRing);
        readyDestroy = true;
        return;
    }

    public void MoveRing() {
        readyDestroy = true;
        return;
    }

    public void OnMouseEnter() {
        lockPosition = true;
        FindAnyObjectByType<GameController>().UnsetCursor();
    }

    public void OnMouseExit() {
        lockPosition = false;
        FindAnyObjectByType<GameController>().ForceCursor();
        if (openTimer > openTime) {
        readyDestroy = true;
        }
    }

    public void Own(List<WorldTile> ring) {
        ownRing = ring;
        eventual = new Vector3(ring.Count * 0.04f, ring.Count * 0.04f, 1f);
    }

    public void PositionMenu() {
        if (lockPosition) {
            return;
        }
        Vector3 midpoint = Vector3.zero;
        foreach (WorldTile iChild in ownRing) {
            midpoint += iChild.transform.localPosition;
        }
        midpoint /= ownRing.Count;

        Vector3 cursorDeltaPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(-9.85f, -6.15f, 0f);
        cursorDeltaPosition.z = 0f;
        midpoint.z = 0f;

        transform.localPosition = midpoint + 0.75f * (midpoint - cursorDeltaPosition) + new Vector3(-9.85f, -6.15f, -9f);
    }
}
