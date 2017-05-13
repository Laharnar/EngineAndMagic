using UnityEngine;
using System.Collections;

// selecting the playtform will offer build or upgrade menu
public class Platform : MonoBehaviour {

    public Transform platformMenu;

    public Transform builtItem;

    // Use this for initialization
    void Start () {
        PlatformBuilder.m.InsertIntoGrid(this, 
            Mathf.RoundToInt(transform.position.x), 
            Mathf.RoundToInt(transform.position.y));
	}
	
    void Selected() {
        // no item built -> show build menu
        if (builtItem == null) {
            platformMenu.gameObject.SetActive(true);
        }
        // item built -> show structure's upgrades
        else {
            builtItem.SendMessage("ShowUpgrades");
        }
    }
}
