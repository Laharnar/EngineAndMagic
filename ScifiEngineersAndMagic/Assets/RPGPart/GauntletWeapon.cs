using UnityEngine;
using System.Collections;

public class GauntletWeapon : MelleWeapon {

    int activeAttack;

    int numberOfUniqueAttacks = 2;

    public override bool Attack() {
        if (base.Attack()) {
            IncrementAttack(); // use different attack next
            return true;
        }
        return false;
    }

    void IncrementAttack() {
        activeAttack = (++activeAttack) % numberOfUniqueAttacks;
        SetAttackIdOnAnimator();
    }

    void SetAttackIdOnAnimator() {
        anim.SetInteger("AttackId", activeAttack);
    }
}
