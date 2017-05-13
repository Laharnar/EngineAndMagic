using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// 1) find player and move to him
// 2) execute attack with the weapon
// 2.1) charge - animation
// 2.2) attack
public class Viking : Combatant {


    public AxeWeapon weapon;

    //public TreeNode root;
    public BNode root;
    // Use this for initialization
    void Awake () {
        NullCheck(ai, "Missing ai on "+name, this);
        NullCheck(weapon, "Missing weapon on "+name, this);
        /*root = new Selector().SetChildren(
        new Sequence().SetChildren(
            BasicBehaviours.BasicEnemyFinder(ai, transform),
            BasicBehaviours.BasicAttackRepeater(weapon)
        ));
        */
        root = new FBSelector().Do<FBSelector>(
            DestroyAllEnemiesOnMap()
        ).Final<FBSelector>("attack nearest logic");
    }

    FBSequence DestroyAllEnemiesOnMap() {
        detectionRange = GlobalAi.maxDetection;

        return new FBSequence().Do<FBSequence>(
            (Func<bool>)FindNearestTarget,
            (Func<bool>)WaitUntilNearTarget,
            (Func<bool>)weapon.Attack
        ).Final<FBSequence>("find and attack nearest");
    }
    

    private void NullCheck(object ai, string v, MonoBehaviour transform) {
        if (ai == null) {
            Debug.Log(v, transform);
        }
    }

    // Update is called once per frame
    void Update () {
        ai.priorityDangerLevel = 1 - Mathf.Exp(hp.hp / hp.maxHp) / Mathf.Exp(hp.maxHp);

        root.Tick();
    }

    void OnDrawGizmos() {
        if (true)
            Gizmos.DrawCube(transform.position+Vector3.down*1.5F, Vector3.one/2);
    }
}
