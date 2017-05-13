using System;
using UnityEngine;

public class AdjustmentsQue : VectorFixing {

    public VectorFixing[] adjustments;

    public override void ApplyFix(ref Vector3 dir, Vector3 pos) {
        for (int i = 0; i < adjustments.Length; i++) {
            adjustments[i].ApplyFix(ref dir, pos);
        }
    }
}

public class VectorFixing :MonoBehaviour{
    public virtual void ApplyFix(ref Vector3 dir, Vector3 pos) {
    }
}