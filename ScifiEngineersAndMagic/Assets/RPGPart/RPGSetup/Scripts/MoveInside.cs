using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
/// <summary>
/// Uses teleporting for entering buildings via trigger collider.
/// Requires a call on npc/player who wants to enter
/// </summary>
public class MoveInside : TeleportTrigger {

    List<Transform> validToEnter = new List<Transform>();

    /// <summary>
    /// where to move
    /// 
    /// Note: if you use ui menu with multple choice of locations, set the target pos from there
    /// Or 2., add multiple choice of transforms here and set active target. but its better to set them from buttons
    /// </summary>
    public Transform targetPos;

	// Use this for initialization
	void Start () {
        if (GameObject.FindGameObjectWithTag("Player") == null) {
            Debug.Log("No player in scene.");
        }
        if (!GetComponent<Collider2D>().isTrigger) {
            Debug.Log("Script is using trigger colliders. Set collider to trigger.");
        }
        SetLocation(targetPos.position);
	}
	
	void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "Player" || other.tag == "Npc") {
            validToEnter.Add(other.transform);
        }
	}

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player" || other.tag == "Npc") {
            validToEnter.Remove(other.transform);
        }
    }

    public void RequestMove(Transform character) {
        if (character == null) {
            return;
        }
        for (int i = 0; i < validToEnter.Count; i++) {
            // check if this character is allowed to enter. some npc's might enter areas that player cant
            if (validToEnter[i]==character) {
                // TODO: run fade inout coroutine together with teleport
                TriggerTeleport(character);
            }
        }
    }
}



