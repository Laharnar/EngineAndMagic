using System;
using System.Collections.Generic;
using UnityEngine;

internal static class GlobalPOI {
    

    // POI: Point Of Interest
    public static List<MonoBehaviour> poi = new List<MonoBehaviour>();

    public static readonly float poiDefaultRange = 10000;

    public static void AddAsPoi(MonoBehaviour point) {
        poi.Add(point);
    }

    internal static MonoBehaviour FindNearestPOI(Vector3 position) {
        bool[] inRange = new bool[0];

        if (AnyInRange(position, poiDefaultRange, poi.GetVectors(), ref inRange)) {
            int target = ChoseNearest(position, poi.GetVectors(), inRange);
            return poi[target];
        }
        return null;
    }

    private static int ChoseNearest(Vector3 position, Vector3[] vector3, bool[] inRange) {
        int best = -1;
        for (int i = 0; i < vector3.Length; i++) {
            if (inRange[i] == false) continue;
            if (best == -1) { best = i; continue; }
            if (Vector3.SqrMagnitude(vector3[i] - position) < Vector3.SqrMagnitude(vector3[best] - position))
                best = i;
        }
        return best;
    }


    public static bool AnyInRange(Vector3 position, float range, Vector3[] search, ref bool[] isInRange) {
        Vector3[] a = search;//GlobalPOI.poi.ToArray();
        bool anyPointsFound = false;

        if (isInRange.Length != a.Length)
            isInRange = new bool[a.Length];

        for (int i = 0; i < a.Length; i++) {
            if (a[i] == null) {
                isInRange[i] = false;
                continue;
            }
            if (MovingCharacter.SqDistance(position, a[i]) < Mathf.Pow(range, 2)) {
                isInRange[i] = true;
                anyPointsFound = true;
            } else
                isInRange[i] = false;
        }
        return anyPointsFound;
    }

    internal static MonoBehaviour RandomPOI(Vector3 position) {
        bool[] inRange = new bool[0];

        if (AnyInRange(position, poiDefaultRange, poi.GetVectors(), ref inRange)) {
            int target = ChoseRandom(position, poi.GetVectors(), inRange);
            return poi[target];
        }
        return null;
    }

    private static int ChoseRandom(Vector3 position, Vector3[] vector3, bool[] inRange) {
        // get valid options into array of indexes
        int[] validOptions = new int[vector3.Length];
        int last = -1;
        for (int i = 0; i < vector3.Length; i++) {
            if (inRange[i] == false) continue;
            validOptions[++last] = i+1; 
        }
        if (last == -1) return -1; // shouldn't happen if you first check if there are any

        // get index of random valid vector
        return validOptions[UnityEngine.Random.Range(0, (int)last)];
    }

    public static Vector3[] GetVectors(this List<MonoBehaviour> monos) {
        Vector3[] vectors = new Vector3[monos.Count];
        for (int i = 0; i < vectors.Length; i++) {
            if (monos[i] != null) {
                vectors[i] = monos[i].transform.position;
            }
        }
        return vectors;
    }

}