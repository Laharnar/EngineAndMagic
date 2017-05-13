using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allows registered crowd characters to use pathfinding.
/// </summary>
public class PathScheduler : MonoBehaviour {

    public static PathScheduler m;

    public Graph grid;
    public CrowdCharacter[] chars;

    public Transform wp1, wp2;
    bool activeWp;
    float time;
	// Use this for initialization
	void Awake () {
        m = this;
    }
	
	// Update is called once per frame
	void Update () {
        // toggle walk between 2 wps
        if (Time.time > time) {
            time = Time.time + 5;
            for (int i = 0; i < chars.Length; i++) {
                // todo: make sure path is found and added, instead of wps directly
                if (chars[i].busy == false && chars[i].pathfinder) {
                    Debug.Log(chars[i].name+"| active wp: "+(activeWp ? "1" : "0") + " "+ (activeWp ? wp1.position : wp2.position));
                    //chars[i].pathfinder.AddWp(activeWp ? wp1.position : wp2.position);
                    activeWp = !activeWp;
                    //List<GNode> nodes = grid.FindPath(chars[i].transform.position, activeWp ? wp1.position : wp2.position);
                    //chars[i].WalkAlongPath(grid.FindRandomPath(chars[i].transform.position));
                    //chars[i].ForceWalk(Vector3.zero, 0.1f, null);
                    chars[i].ForcePathfindWalk(grid, chars, activeWp ? wp1.position : wp2.position, 0.1f, null);
                }
            }
        }
        for (int i = 0; i < chars.Length; i++) {
            chars[i].WalkToWp();
        }
    }
}
