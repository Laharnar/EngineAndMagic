using UnityEngine;
using System.Collections;

public class CharacterAnimControl : MonoBehaviour {

    public Animator anim;

	// Use this for initialization
	void Awake () {
        anim = anim == null ? anim = GetComponent<Animator>() : anim;
	}
	
    public void SetTrigger(string triggerName) {
        anim.SetTrigger(triggerName);
    }
}
