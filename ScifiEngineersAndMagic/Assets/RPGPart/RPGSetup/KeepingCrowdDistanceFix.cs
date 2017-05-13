using System;
using System.Collections.Generic;
using UnityEngine;

public class KeepingCrowdDistanceFix : VectorFixing{

    public Ai ai;
    public float keepRangeToAlliesMin;
    public float keepRangeToAlliesMax;
    public float keepRangeToAllies;

    public float keepRangeToEnemies;

    void Awake() {
        NextRange();
    }

    public override void ApplyFix(ref Vector3 dir, Vector3 pos) {

        // keeps distance to allies by taking nearest 2
        MovingCharacter[] participants = GlobalCrowds.Participants();
        // TODO: apply right or left vectors based on distance to crowd and their direction, prefering right
            Vector3 fix = Vector3.zero;
        MovingCharacter nearest = GlobalCrowds.ChoseNearest(transform);
        if (nearest) {

            Vector3 neardir = nearest.transform.position - transform.position;
            if (keepRangeToAllies == 0) Debug.Log("Range is 0.", this);
            if (Vector3.Distance(nearest.transform.position, transform.position) < keepRangeToAllies) {
                fix += -neardir;
                NextRange();
            }
            /*
            bool[] filter = new bool[GlobalCrowds.worldCrowd.Count];
            for (int i = 0; i < filter.Length; i++) {
                filter[i] = true;
            }
            GlobalCrowds.AddToFilter(ref filter, nearest, false);
            MovingCharacter secondNearest = GlobalCrowds.ChoseNearest(transform, filter);
            // find path to near and secnear, add negative vectors of them to fix and normalize
            // you get a vector pointing away from 2 nearest units
            if (secondNearest != null) {
                Vector3 secndir = secondNearest.transform.position - transform.position;
                fix += -secndir;
                Debug.Log("test2 "+fix);
            }*/
        }

        dir = (dir + fix).normalized;
        base.ApplyFix(ref dir, pos);
    }

    public void NextRange() {
        keepRangeToAllies = UnityEngine.Random.Range(keepRangeToAlliesMin, keepRangeToAlliesMax);
        ai.crowds.SetRange(Mathf.Max(keepRangeToAllies+0.1f, ai.crowds.arrivedRange));
    }
}
