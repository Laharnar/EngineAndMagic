using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Behaviour:
/// shoot at closest enemy.
/// if enemy is close try to keep distance, but keep shooting. stop to shoot
/// reload if low on bullets and enemy isnt near.
/// if ran out of bullets, and enemy is near, move away to safe distance
/// 
/// optional: accuracy angle is 80%, meaning 90-90 degrees*0.8=+-18 degree error
/// bullets filter by enemy type
/// </summary>
public class Police : Combatant {

    public ProjectileWeapon gun;

    //Sequence root;
    FBSequence root;
	// Use this for initialization
	void Awake () {
        root = new FBSequence().Do<FBSequence>(
            new RunTimes().Do<RunTimes>(
                    new Encapsulator().Final<Encapsulator>("init range", Init)
                ).Final<RunTimes>("init"),

            (Func<bool>)FindNearestTarget,

            new FBParalel().Do<FBParalel>(
                new FBSequence().Do<FBSequence>(
                    (Func<bool>)AimAtTarget,
                    (Func<bool>)gun.Attack
                ),
                (Func<bool>)KeepDistance
            )
        );
    }

    bool Init() {
        gun.atkRate = 1;
        return true;
    }

    // Update is called once per frame
    void Update () {
        root.Tick();
	}

    bool AimAtTarget() {
        gun.Aim(target.position);
        return true; // TODO: Slow aim + Return true only if aim is within angle error.
    }
}

