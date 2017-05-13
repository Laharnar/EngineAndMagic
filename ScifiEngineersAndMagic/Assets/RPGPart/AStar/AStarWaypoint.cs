using UnityEngine;
using System.Collections;

public class AStarWaypoint : MonoBehaviour {

    public Collider2D col;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (col && col.enabled == false) return;
        if (other.tag == "Level") {
            Destroy(gameObject);
        }

    }
}
