using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CrowdCharacter : MonoBehaviour {

    public static List<CrowdCharacter> crowd = new List<CrowdCharacter>();

    [SerializeField] Vector3 _wp;
    [System.Obsolete("not used")]
    public List<Vector3> wps = new List<Vector3>();
    Action arriveCallback;

    Rigidbody2D rig;
    public int speed = 1;

    internal bool busy;
    public bool walking { get; private set; }
    float dist;
    public float arrivedRange = 0.5f;
    private float nextWpRange = 2f;

    /// <summary>
    /// squared distance how far should characters attept stay away from each other
    /// 
    /// </summary>
    public float sqCrowdKeepingDistance = 1;

    SpriteRenderer[] rend;  //allows to adjust for sorting within the unit layer, for visuals
    private int worldLayerOffset = 1000; // how far down below 0,0 coordinates can sprite go without dissapearing

    public AdjustmentsQue moveFixing;

    bool isTemporaryWp;
    public PathfindAgent pathfinder;

    internal PathScheduler pathMaker;

    Transform t;

    // Use this for initialization
    void Awake () {
        _wp = transform.position;
        rig = GetComponent<Rigidbody2D>();

        rend = transform.GetComponentsInChildren<SpriteRenderer>();

        t = transform;

        crowd.Add(this);
	}

    void LateUpdate() {
        // fix order of layer, units below are drawn on top
        // Note: there is a limit with world layer offset -2000 means you can do 2000 units down
        int layer = worldLayerOffset - (int)Camera.main.WorldToScreenPoint(GetComponentInChildren<SpriteRenderer>().bounds.min).y;
        for (int i = 0; i < rend.Length; i++) {
            rend[i].sortingOrder = layer;
        }
        
    }

    internal void ForcePathfindWalk(Graph grid, CrowdCharacter[] chars, Vector3 position, float customArrivedRange, Action callback) {
        Debug.Log("pathfind force walk");
        // todo: remove this function, and only keep ForceWalk
        // todo: assign pathmaker from pathscheduler before callind force walk
        ForceWalk(position, customArrivedRange, callback);

        // if pathfinder exists, use it to calculate path
        if (pathfinder) {
            Path<GNode> nodes = grid.FindPath(t.position, position);
            
            Debug.Log("path, length:"+nodes.PathLength);
            _wp = pathfinder.SetupPath(position, nodes.ToList());
            Debug.Log("add pathfinder " + _wp);
            // todo: replace "wp" data with pathfinders, that has correct path
            //isTemporaryWp = pathfinder.FixWp();
            // every time wp gets updated path will be recalculated. if that's too much strain, do it only when target position drasticaly changes/changes enough.
            // pathfinder supplements the wp with his own internal path wp's, until arriving to goal wp.
        }
    }

    /// <summary>
    /// ignore multivector path and just walk directly there
    /// 
    /// useful for enemy attacking.
    /// </summary>
    /// <param name="position1"></param>
    /// <param name="customArrivedRange"></param>
    /// <param name="callback"></param>
    internal void ForceWalk(Vector3 position1, float customArrivedRange, Action callback) {
        if (name == "WanderingNpc") {
            Debug.Log("force walk to?" + _wp);
        }
        arrivedRange = customArrivedRange;
        _wp = position1;
        arriveCallback = callback;
        busy = walking= true;


    }

    internal void SetRange(float v) {
        arrivedRange = v;
    }

    // Update is called once per frame
    void Update () {
        //WalkToWp(); use bt
    }

    internal bool WalkTo(Vector3 position) {
        if (name == "WanderingNpc") {
            Debug.Log("walk to?");
        }
        _wp = position;
        return WalkToWp();// return is moving
    }


    public bool WalkToWp() {
        
        TimeDebug.Record("walktowp_" + name + "_" + GetInstanceID());
        if (pathfinder) {
            pathfinder.UpdatePathfinder(ref _wp);
        }
        TimeDebug.Stop("walktowp_" + name + "_" + GetInstanceID());

        TimeDebug.Record("checkdist_" + name + "_" + GetInstanceID());
        CheckDistances();
        TimeDebug.Stop("checkdist_" + name + "_" + GetInstanceID());

        if (!walking) return false;// false: not moving; true: moving
        TimeDebug.Record("move_" + name + "_" + GetInstanceID());

        // get direction fixes that will prevent getting stuck on other npc's
        Vector3 fix = Vector3.zero; // broken when chasing enemies GetFixedDistanceFromCrowd();
        //Vector3 fix2 = GetFixedDistanceFromWalls();
        
        Vector3 dir = (_wp - t.position).normalized;
        fix = dir;
        if (moveFixing)
            moveFixing.ApplyFix(ref fix, t.position);

        //dir = (dir + fix).normalized;
        dir = fix.normalized;
        rig.MovePosition(t.position + dir * Time.deltaTime * speed);
        //Debug.DrawLine(t.position, _wp, Color.yellow);
        TimeDebug.Stop("move_" + name + "_" + GetInstanceID());

        return true;
    }

    private void CheckDistances() {
        //dist = Vector3.Distance(wp, transform.position);
        
        
        dist = Vector3.Distance(_wp, transform.position);

        // too close range, no wps, stop
        if (dist - transform.position.z < arrivedRange) { // 0.5f
            walking = false;
            if (arriveCallback != null) arriveCallback();

        } else {
            // still not arrived
            walking = true;
        }
    }

    private Vector3 GetFixedDistanceFromCrowd() {
        Vector3 fix = Vector3.zero;
        for (int i = 0; i < crowd.Count; i++) {
            // if agent is near another agent, apply small movement away from it.
            if (crowd[i] == null || crowd[i].transform.position == _wp) continue;
            if (sqCrowdKeepingDistance > Vector2.SqrMagnitude(crowd[i].transform.position - t.position)) {
                fix += -(crowd[i].t.position - t.position).normalized;
            }
        }
        return fix;
    }

    
}
