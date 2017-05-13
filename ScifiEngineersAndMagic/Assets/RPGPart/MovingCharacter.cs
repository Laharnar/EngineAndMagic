using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class MovingCharacter : MonoBehaviour {

    public Hp hp;
    public Ai ai;

    protected Vector3 position;

    protected void Awake() {
        GlobalCrowds.AddParticipant(this);
    }

    protected virtual bool WaitUntilNearTarget() {
        bool moving = ai.crowds.WalkTo(position);
        return !moving; // true: arrived, false: moving
    }

    public static float SqDistance(Vector3 v1, Vector3 v2) {
        return (Mathf.Pow(v2.x - v1.x, 2) + Mathf.Pow(v2.y - v1.y, 2));
    }


    protected virtual bool KeepDistance() {
        // t: keeping dist, f: too close, moving away
        float dist = 5f;
        if (Vector3.Distance(transform.position, position) < dist) {
            // not moving means, keep dist
            return !ai.crowds.WalkTo(transform.position + (transform.position - position).normalized * dist);
        }
        return true;
    }
}

public static class GlobalCrowds {

    public static List<MovingCharacter> worldCrowd = new List<MovingCharacter>();

    public static MovingCharacter[] Participants() {
        return worldCrowd.ToArray();
    }

    internal static void AddToFilter(ref bool[] filter, MovingCharacter add, bool pass) {
        for (int i = 0; i < worldCrowd.Count; i++) {
            if (worldCrowd[i] == add) {
                filter[i] = pass;
                break;
            }
        }
    }

    internal static void AddParticipant(MovingCharacter movingCharacter) {
        worldCrowd.Add(movingCharacter);
    }

    internal static MovingCharacter ChoseNearest(Transform transform, bool[] filtered = null) {
        int best = -1;
        //GlobalAi.NullClear();
        for (int i = 0; i < worldCrowd.Count; i++) {
            if (filtered != null && filtered[i] == false) continue;
            bool passFilter = true;
            if (passFilter) {
                try {
                    if (worldCrowd[i].transform == transform) continue;
                    //    if (allAi[i] == null) { allAi = allAi.NullCleared(); }
                    if (best == -1) { best = i; continue; }
                    if (Vector3.SqrMagnitude(worldCrowd[i].transform.position - transform.position) < Vector3.SqrMagnitude(worldCrowd[best].transform.position - transform.position))
                        best = i;
                } catch (MissingReferenceException) {
                    Debug.LogError(worldCrowd[i] + " " + worldCrowd[best] + " " + transform);
                    throw;
                }
            }
        }

        return best == -1 ? null : worldCrowd[best];
    }
}