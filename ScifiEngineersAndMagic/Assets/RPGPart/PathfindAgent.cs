using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//https://blogs.msdn.microsoft.com/ericlippert/tag/AStar/
// allows some unit to move between wp's before going to goal
public class PathfindAgent : MonoBehaviour {

    public float wpNearRange = 1;
    List<Vector3> wps = new List<Vector3>();
    Vector3 wp;
    Vector3 goal;
    internal bool busy = false;// is using pathfind, false: going to goal

    void Awake() {
        // TODO: remove awake function
        // test run
    
        //AddWp(new Vector3(-1, 2));
        //AddWp(new Vector3(-1, 4));

    }

    internal void AddWp(Vector3 position1) {
        this.wps.Add(position1);
    }

    /// <summary>
    /// Converts nodes into path.
    /// Note: goal node is duplicated, if it's contained in nodes. Remove it yourself before calling function if design requires it.
    /// </summary>
    /// <param name="goal"></param>
    /// <param name="nodes"></param>
    /// <returns></returns>
    internal Vector3 SetupPath(Vector3 goal, List<GNode> nodes) {
        SetWalkAlongPath(nodes);
        SetGoal(goal);
        return NextWp();
    }

    /// <summary>
    /// Changes to next wp when near current one.
    /// Call from movement function.
    /// </summary>
    /// <param name="_wp"></param>
    internal void UpdatePathfinder(ref Vector3 _wp) {
        if (NearWp()) {
            _wp = NextWp();
        }
    }

    void SetGoal(Vector3 goal) {
        this.goal = goal;
    }

    void SetWalkAlongPath(List<GNode> wps) {
        for (int i = 0; i < wps.Count; i++) {
            AddWp(wps[i].transform.position);
        }
    }

    Vector3 NextWp() {
        if (wps.Count == 0) {
            // Make sure wp is set to goal only once
            if (busy == true) {
                Debug.Log("no wps");
                // swap back to goal
                busy = false;
                wp = goal;
            }
        } else {
            //            Debug.Log("new wp");
            wp = wps[0];
            wps.RemoveAt(0);
        }
        return wp;
    }

    bool NearWp() {
        float dist = Vector3.Distance(wp, transform.position);
        return dist - transform.position.z < wpNearRange; // 0.5f
    }

}
