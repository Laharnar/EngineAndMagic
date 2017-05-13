using UnityEngine;
using System.Collections;

public class Hp : MonoBehaviour {

    public float maxHp = 1;
    public float armor;// damage reduction 0-1, 0: no reduction
    float _hp;
    internal float hp { get { return _hp; } set { _hp = value; } }

    public float maxShields = 0;
    public float shieldRegenRate = 1;// regen in seconds
    public float shieldRegenAmount = 1; // flat amount

    float shieldsTime;
    float _shields;
    internal float shields { get { return _shields; } set { _shields = value; } }

    public GameObject destroyed;

    void Awake() {
        hp = maxHp;
        shields = maxShields;
        shieldsTime = Time.time+shieldRegenRate;
        if (destroyed == null) {
            destroyed = gameObject;

        }
    }

    public void Damage(float dmg) {
        shields -= dmg;
        float dmgOverShields = shields;
        shields = Mathf.Max(shields, 0);

        if (dmgOverShields < 0) {
            dmg = dmgOverShields;
            hp -= dmg * (1 - armor);
        }

        if (hp <= 0) {
            Destroy(destroyed);
        }
    }

    void Update() {
        if (maxShields > 0) {
            if (Time.time > shieldsTime) {
                shields = Mathf.Min(shields + shieldRegenAmount, maxShields) ;
                shieldsTime += shieldRegenRate;
            }
        }
    }
}
