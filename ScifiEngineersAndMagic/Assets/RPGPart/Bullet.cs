using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float speed = 4;

    public string alliance;

    public int damage = 1;

    // not used anywhere
    public void Init(string alliance, int damage) {
        this.alliance = alliance;
        this.damage = damage;
    }

	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.right*Time.deltaTime * speed);
	}

    void OnTriggerEnter2D(Collider2D other) {
        Ai ai = other.GetComponent<Ai>();
        if (ai && ai.alliance != alliance) {
            ai.GetComponent<Hp>().Damage(damage);
            Destroy(gameObject);
        }
    }
}
