using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GauntletSuit : Combatant {
    TreeNode root2;
    FBSequence root;

    public Weapon weapon;

    int activeAttack;

    void OnDrawGizmos() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    // Use this for initialization
    void Awake() {
        // setup root node same way as for viking. just different animation, 
        // added push, and higher damage
        root = new FBSequence().Do<FBSequence>(
            (Func<bool>)FindNearestTarget,
            (Func<bool>)WaitUntilNearTarget,
            (Func<bool>)weapon.Attack
        ).Final<FBSequence>("Detect and attack nearest");
        return;
        root2 = new Sequence().SetChildren(
            // enemy find
            new DataTransferSequence().SetChildren(
                new AllInRange().Init(new Dictionary<string, object>() {
                    { "searchRange", detectionRange },
                    { "position", transform }
                }),
                new FindNearestTarget().Init(new Dictionary<string, object>() {
                        { "transform", transform },
                        { "alliance" , ai.alliance }
                    }),
                // move near found target
                new MoveNearDestination().SetChecks(new CheckIsInRange())
                .Init(new Dictionary<string, object>() {
                        { "range" , 1.0f },
                        { "ai" , ai }
                    })
            ),

            new InfiniteRepeater().SetChild(
                new DataTransferSequence().SetChildren(
                        // basic atacking
                        new WaitAction(weapon.NotAttacking),
                        new Timer().Init(new Dictionary<string, object>() {
                        { "ctime", Time.time }
                        }),
                        new SetTime().Init(new Dictionary<string, object>() {
                            { "rate", weapon.atkRate }
                        }),
                        new SetTriggerAnimation().Init(new Dictionary<string, object>() {
                            { "triggerChoice" , "Attack" },
                            { "animator", weapon.anim },
                            { "active", true }// todo: it will check if animation exists. it will allows to preset actions for animation, but have it turned off
                        }),
                        // switch to other attack
                        new ActionNode(IncrementAttack),
                        new ActionNode(SetAttackIdOnAnimator)
                    )
            )
        );
	}

    void IncrementAttack() {
        activeAttack = (++activeAttack) % 2;
    }

    void SetAttackIdOnAnimator() {
        weapon.anim.SetInteger("AttackId", activeAttack);
    }



    // Update is called once per frame
    void Update () {
        root.Tick();
	}
}