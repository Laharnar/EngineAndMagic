using UnityEngine;
using System.Collections;
using System;

public class Weapon : MonoBehaviour {

    public Animator anim;
    
    public Transform weaponUser;

    public float atkRate = 1;
    protected float time;
    protected float error = 0.1f;
    bool attacking = false;

    public string activeAttackTrigger = "Attack";

    public AttackType[] attackLibrary;

    public class AttackType {
        public string trigger; // "Attack" "Heavy"
        public string code;// "normal" "heavy"
        public float consumesStamina;
    }

    public bool NotAttacking() {
        return !attacking;
    }

    /// <summary>
    /// Default behaviour sets animator's trigger to "Attack"
    /// </summary>
    public virtual bool Attack(string attackTrigger) {
        if (time > Time.time) return false;
        time = Time.time + atkRate+UnityEngine.Random.Range(-error, error);

        anim.SetTrigger(attackTrigger);
        return true;// false: waiting reload, true: attack triggered
    }

    public virtual bool Attack() {
        if (time > Time.time) return false;
        time = Time.time + atkRate + UnityEngine.Random.Range(-error, error);

        anim.SetTrigger(activeAttackTrigger);
        if (attackLibrary != null)
        for (int i = 0; i < attackLibrary.Length; i++) {    
            if (attackLibrary[i].trigger == activeAttackTrigger) {
                weaponUser.GetComponent<Stamina>().Consume(attackLibrary[i].consumesStamina);
                break;
            }
        }
        // TODO: different attacks, with different stamina
        return true;
    }

    // call it from animation
    public virtual void OnAttackDone(string info) {
        attacking = false;
    }
}
