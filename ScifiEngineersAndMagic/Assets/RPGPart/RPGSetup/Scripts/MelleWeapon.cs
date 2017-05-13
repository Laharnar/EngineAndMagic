using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class MelleWeapon : Weapon {
    public int dmg;

    protected List<Transform> dmgStack = new List<Transform>();

    void OnTriggerEnter2D(Collider2D other) {
        dmgStack.Add(other.transform);
    }

    void OnTriggerExit2D(Collider2D other) {
        dmgStack.Remove(other.transform);
    }

    public override void OnAttackDone(string info) {
        for (int i = 0; i < dmgStack.Count; i++) {
            if (dmgStack[i] == null) { dmgStack.RemoveAt(i--); continue; }
            if (dmgStack[i].GetComponent<Hp>()) dmgStack[i].GetComponent<Hp>().Damage(dmg);
        }
    }
}

public class PoleWeapon : MelleWeapon {


}