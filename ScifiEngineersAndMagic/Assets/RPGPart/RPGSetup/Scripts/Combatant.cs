using System;
using UnityEngine;

public abstract class Combatant : MovingCharacter {

    public Transform target;

    public float detectionRange = 20f;

    protected override bool WaitUntilNearTarget() {
        position = target.position;
        return base.WaitUntilNearTarget(); // true: arrived, false: moving
    }

    protected bool AnyAiInRange(Vector3 position, float range, ref bool[] isInRange) {
        Ai[] a = GlobalAi.allAi.ToArray();
        bool anyUnitsFound = false;

        if (isInRange.Length != a.Length)
            isInRange = new bool[a.Length];

        for (int i = 0; i < a.Length; i++) {
            if (a[i] == null) {
                isInRange[i] = false;
                continue;
            }
            if (SqDistance(a[i].transform.position, position) < Mathf.Pow(range, 2)) {
                isInRange[i] = true;
                anyUnitsFound = true;
            } else
                isInRange[i] = false;
        }
        return anyUnitsFound;
    }


    protected virtual bool FindNearestTarget() {
        bool[] aiInRange = new bool[0];// TODO: save this value on unit. and only run updates every few frames
        if (AnyAiInRange(transform.position, detectionRange, ref aiInRange)) {
            Ai targetAi = GlobalAi.ChoseNearest(transform, ai.alliance, "!", aiInRange);
            if (targetAi) {
                this.target = targetAi.transform;
                return true;
            }
        }
        return false;
    }

    
}