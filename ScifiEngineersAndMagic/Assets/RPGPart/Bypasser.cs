using UnityEngine;
using System.Collections;
using System;

// simple npc witout combat, only moves around looking at interesting things (poi)
public class Bypasser : MovingCharacter {

    BNode root;
    SomethingInteresting poiTarget; // movement objective

    float attentionLevel = 1; // % how much interest it shows in attractions

    // waiting for a while
    bool waiting = false;
    float time;

	// Use this for initialization
	protected new void Awake () {
        base.Awake();
        // can observe interesting locations for some time, - watch the talk, or stalls
        // can run and walk at different speeds


        // run from danger - when attack begins
        // can localy avoid others while moving - move in on the talk, or around the talk area
        // get stuck to others while running - hit run into others while run away, pushing them down
        // allows player to push him away - player moves through the crowd

        // local guidance by pushing others away while moving towards target
        // local avoidance while idle, get pushed
        // running with tripping over when hitting into others
        // pushed down when someone runs
        // find safe location to run to
        // chose point of interest to move to
        // look in direction
        root = new FBSelector().Do<FBSelector>(
            // repeating
            new FBTokenRing().Do<FBTokenRing>(
                // invert failure from sequences or positive it, to make sure ring continue
                new FBTokenRing().Do<FBTokenRing>(
                    (Func<bool>)FindPointOfInterest,
                    (Func<bool>)WaitUntilNearPointOfInterest
                ).Final<FBTokenRing>("walk to new point of interest", 0),
                (Func<FBStatus>)WaitAtPointOfInterest
            ).Final<FBTokenRing>("watch interesting things", 0)

        ).Final<FBSelector>("useless bystanders on festival");
    }
    

    // Update is called once per frame
    void Update () {
        root.Tick();
	}

    bool FindPointOfInterest() {
        TimeDebug.Record("find_point_"+name+"_" + GetInstanceID());
        poiTarget = (SomethingInteresting)GlobalPOI.RandomPOI(transform.position);
        TimeDebug.Stop("find_point_" + name + "_" + GetInstanceID());
        if (poiTarget) {
            return true;
        }
        return false;
    }

    bool WaitUntilNearPointOfInterest() {
        // more participants there are, more space should be kept, but more can approach

        ai.crowds.arrivedRange = Mathf.Max(0.1f, poiTarget.numOfParticipants);
        bool moving = ai.crowds.WalkTo(poiTarget.transform.position); // True:moving ->negate
        return moving == false;
    }

    FBStatus WaitAtPointOfInterest() {
        TimeDebug.Record("waiting_" + name + "_" + GetInstanceID());
        if (waiting == false) {

            waiting = true;
            time = Time.time + poiTarget.timeInvestment * attentionLevel;
            poiTarget.NewParticipant(this);
        }
        if (waiting) {
            if (Time.time < time )
                return FBStatus.Running;
            poiTarget.LostInterest(this);
            waiting = false;
            TimeDebug.Stop("waiting_" + name + "_" + GetInstanceID());
            return FBStatus.Success;
        }
        TimeDebug.Stop("waiting_" + name + "_" + GetInstanceID());
        return FBStatus.Failure;

    }
}
