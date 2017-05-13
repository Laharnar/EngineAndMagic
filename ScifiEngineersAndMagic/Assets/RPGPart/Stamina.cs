using UnityEngine;
using System.Collections;
using System;

public class Stamina : MonoBehaviour {

    public float maxStamina;
    float _stamina;
    public float stamina { get { return _stamina; } set { _stamina = Mathf.Clamp(value, 0, maxStamina); } }
    public float regenPerSec = 1;

	// Use this for initialization
	void Awake () {
        stamina = maxStamina;
	}
	
	// Update is called once per frame
	void Update () {
        stamina = stamina + regenPerSec * Time.deltaTime;
	}

    internal void Consume(float consume) {
        stamina -= consume;
    }
}
