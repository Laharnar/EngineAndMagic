using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkCollision : WorldInteraction {

    List<Transform> talkWith = new List<Transform>();

    public CharacterChat lines;

    // Use this for initialization
    void Start () {
        if (GameObject.FindGameObjectWithTag("Player") == null) {
            Debug.Log("No player in scene.");
        }
        if (!GetComponent<Collider2D>().isTrigger) {
            Debug.Log("Script is using trigger colliders. Set collider to trigger.");
        }
    }
	
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" || other.tag == "Npc") {
            talkWith.Add(other.transform);
            // TODO: begin talking?
            Debug.Log("Npc can talk "+ name);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player" || other.tag == "Npc") {
            talkWith.Remove(other.transform);
        }
    }
}
