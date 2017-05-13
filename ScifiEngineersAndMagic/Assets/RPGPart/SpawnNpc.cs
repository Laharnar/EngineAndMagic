using UnityEngine;
using System.Collections;
using System;

public class SpawnNpc : MonoBehaviour {

    public Transform pref;
    public int spawnNumber;
    public float rate = 2f;
    float time;
	
    // Update is called once per frame
	void Update () {
        if (spawnNumber > 0 && Time.time > time) {
            Spawn();
            time = Time.time + rate;
            spawnNumber--;
        }
	}

    private void Spawn() {
        Transform t = Instantiate(pref);
        t.position = transform.position;
        t.name += spawnNumber;
    }
}
