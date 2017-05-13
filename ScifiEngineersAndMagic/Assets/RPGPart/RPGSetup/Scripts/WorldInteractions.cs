using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allows player and npcs to interact with the world.
/// Acts as basic interface between commands and world.
/// 
/// Interactible items are detected with collision.
/// </summary>
public class WorldInteractions : MonoBehaviour {

    // make sure this tag exists in scene.
    public static string defaultTag = "interaction";

    // public WorldInteractionCommands[] commands; fix: connect strings and commands

    List<WorldInteraction> interactibleInRange = new List<WorldInteraction>();


    #region Functions that require customization
    // use this on buttons or npc's. extend it
    public enum WorldInteractionCommands {
        MoveIntoBuilding,
        Talk
    }

    /// <summary>
    /// Has some specific custom queries that explain what is interaction about.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public bool CanInteract(string query) {
        if (query == "move into building") {
            return -1 != IndexOf(WorldInteractionCommands.MoveIntoBuilding);
        }
        if (query == "talk to npc") {
            return -1 != IndexOf(WorldInteractionCommands.Talk);
        }
        return false;
    }

    // note: it can interract with only one thing per command
    public void InteractiWithWorld(WorldInteractionCommands command, int lookFor = 0) {
        int counted = 0;// allows you to skip first, or second correct type command.
        for (int i = 0; i < interactibleInRange.Count; i++) {
            if (interactibleInRange[i].interactionType != command) {
                continue;
            }
            if (counted < lookFor) {
                counted++;
                continue;
            }
            switch (interactibleInRange[i].interactionType) {
                case WorldInteractionCommands.MoveIntoBuilding:
                    ((MoveInside)interactibleInRange[i]).RequestMove(transform);
                    break;
                case WorldInteractionCommands.Talk:
                    Debug.Log("gibberish");
                    break;
                default:
                    break;
            }
        }

    } 
    #endregion

    void OnTriggerEnter2D (Collider2D worldItem) {
    
        // record all world items character cna interact with, except other character and combat items
        // it's assumed, that the item will have special tag for the world item
        if (worldItem.tag == defaultTag) {
            WorldInteraction wi = worldItem.GetComponent<WorldInteraction>();
            if (wi) {
                if (interactibleInRange.Contains(wi)) Debug.Log("Suggestion: You have multiple trigger colliders firing this message.", this);
                interactibleInRange.Add(wi);
                // todo: Add oinstant options, that triggers on trigger, here
            }
        }
	}

    void OnTriggerExit2D(Collider2D worldItem) {
        // record all world items character cna interact with, except other character and combat items
        // it's assumed, that the item will have special tag for the world item
        if (worldItem.tag == defaultTag) {
            WorldInteraction wi = worldItem.GetComponent<WorldInteraction>();
            if (wi) {
                if (!interactibleInRange.Contains(wi)) Debug.Log("Suggestion: You have multiple trigger colliders firing this message.", this);
                interactibleInRange.Remove(wi);
            }
        }
    }

   

    void InteractWithWorld(string customCommand) {
        throw new System.NotImplementedException();
    }

    int IndexOf(WorldInteractionCommands find, int start = 0) {
        for (int i = start; i < interactibleInRange.Count; i++) {
            if (interactibleInRange[i].interactionType == find) {
                return i;
            }
        }
        return -1;
    }

    
}


public class WorldInteraction : MonoBehaviour {
    public WorldInteractions.WorldInteractionCommands interactionType;

    protected void Awake() {
        if (tag != "interaction") {
            Debug.Log("Suggestion: You might be missing a tag for interaction item.");
        }
    }
}