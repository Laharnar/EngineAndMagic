using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// AI Movement script
/// </summary>
public class Ai : MonoBehaviour {

    public string alliance = "unknown";
    public CrowdCharacter crowds;
    

    // separate ai and priority MonoBehavour>Priority>Ai. doors can have priority
    public double priorityDangerLevel = 1; // how high is this danger level for this unit. change self behaviour based on this.

    // how important is this object for other objects. Others can decide to chose this as target if it's importance outweights the danger level.
    public double priorityImportance = 1;

    public Action behaviour;

    void Awake () {
        GlobalAi.allAi.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
        if (behaviour != null) {
            behaviour();
        }

	}

    internal void WalkNear(Vector3 position, float gotNearRange, Action onDone) {
        crowds.ForceWalk(position, gotNearRange, onDone);
    }
}

static class GlobalAi {
    public static List<Ai> allAi = new List<Ai>();
    internal static readonly float maxDetection = 100000;

    public static void NullClear() {
        int removed = 0;
        for (int i = 0; i < allAi.Count; i++) {
            if (allAi[i] == null) {
                allAi.RemoveAt(i);
                i--;
                removed++;
            }
        }
    }

    public static List<T> NullCleared<T>(this List<T> a) {
        for (int i = 0; i < a.Count; i++) {
            if ((T)a[i] == null) {
                a.RemoveAt(i);
                i--;
            }
        }
        return a;
    }

    /// <summary>
    /// Choses highest priority ai to make actions around, be it ally or enemy.
    /// </summary>
    /// <param name="alliance">focus only on enemies. (*) means anyone (!) means any other alliance </param>
    /// <returns></returns>
    internal static Ai ChoseTargetByPriorityDanger(string alliance = "*") {
        int max = -1;
        for (int i = 0; i < allAi.Count; i++) {
            bool allyPass = true;
            if (alliance == "=")
                allyPass = alliance == allAi[i].alliance;  // filter by alliances
            else if (alliance == "!")
                allyPass = alliance != allAi[i].alliance;  // filter by alliances


            if (allyPass) {
                if (max == -1) { max = i; continue; }
                double p1 = allAi[i].priorityDangerLevel;
                double pmax = allAi[max].priorityDangerLevel;
                if (p1 > pmax)
                    max = i;
            }
        }
        return max == -1 ? null : allAi[max];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="alliance"></param>
    /// <param name="operation">! skip allied, = only allied</param>
    /// <param name="filteredAi"></param>
    /// <returns></returns>
    internal static Ai ChoseNearest(Transform transform, string alliance, string operation = "*", bool[] filteredAi = null) {
        int best = -1;
        GlobalAi.NullClear();
        for (int i = 0; i < allAi.Count; i++) {
            if (filteredAi != null && filteredAi[i] == false) continue;
            bool passFilter = true;
            if (operation == "!")
                passFilter = alliance != allAi[i].alliance;  // filter by alliances
            else if (operation == "=")
                passFilter = alliance == allAi[i].alliance;  // filter by alliances

            if (passFilter) {
                try {
                //    if (allAi[i] == null) { allAi = allAi.NullCleared(); }
                    if (best == -1) { best = i; continue; }
                    if (Vector3.SqrMagnitude(allAi[i].transform.position - transform.position) < Vector3.SqrMagnitude(allAi[best].transform.position - transform.position))
                        best = i;
                } catch (MissingReferenceException) {
                    Debug.LogError(allAi[i] + " "+allAi[best] + " "+ transform);
                    throw;
                }
            }
        }
        return best == -1 ? null : allAi[best];
    }

    internal static Ai ChoseTargetByPriorityImportance(string alliance, string operation = "*", bool[] filteredAi = null) {
        int max = -1;
        for (int i = 0; i < allAi.Count; i++) {
            if (filteredAi != null && filteredAi[i] == false) {
                continue;
            }

            bool allyPass = true;
            if (operation == "!")
                allyPass = alliance != allAi[i].alliance;  // filter by alliances

            if (allyPass) {
                if (max == -1) { max = i; continue; }
                double p1 = allAi[i].priorityImportance;
                double pmax = allAi[max].priorityImportance;
                if (p1 > pmax)
                    max = i;
            }
        }
        return max == -1 ? null : allAi[max];
    }

    internal static Ai ChoseTargetByPriorityAverage(string alliance = "*") {
        int max = -1;
        for (int i = 0; i < allAi.Count; i++) {
            bool allyPass = true;
            if (alliance == "*") 
                allyPass = alliance != allAi[i].alliance;  // filter by alliances

            if (allyPass) {
                if (max == -1) { max = i; continue; }
                double p1 = (allAi[i].priorityImportance+ allAi[i].priorityImportance)/2;
                double pmax = (allAi[max].priorityImportance + allAi[max].priorityDangerLevel)/2;
                if (p1 > pmax)
                    max = i;
            }
        }
        return max == -1 ? null : allAi[max];
    }

    /// <summary>
    /// finds ai in list and marks it
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="add"></param>
    internal static void AddToFilter(ref bool[] filter, Ai add, bool pass) {
        for (int i = 0; i < allAi.Count; i++) {
            if (allAi[i] == add) {
                filter[i] = pass;
                break;
            }
        }
    }
}

interface IAiBehaviour {
    void AiLoop();
}