using System;
using UnityEngine;

// todo: put it in separate file
public class TimeDebug : MonoBehaviour {

    static TimeDebug m;

    public TaggedTime[] times;
    public int[] countedTimes;

    [Serializable]
    public class TaggedTime {
        public string key;
        public float value;

        public TaggedTime(string key, float value) {
            this.key = key;
            this.value = value;
        }
    }
    static System.Collections.Generic.Dictionary<string, float> recordings = new System.Collections.Generic.Dictionary<string, float>();
    static System.Collections.Generic.Dictionary<string, int> counted = new System.Collections.Generic.Dictionary<string, int>();

    void Awake() {
        m = this;
    }

    // todo: call record and stop from functions, every unity should use it's id for key
    public static void Record(string tag) {
        if (recordings.ContainsKey(tag))
            recordings[tag] = Time.realtimeSinceStartup;
        else recordings.Add(tag, Time.realtimeSinceStartup);

        if (counted.ContainsKey(tag))
            counted[tag] = counted[tag] + 1;
        else counted.Add(tag, 0);
        if (m == null) {
            m = new GameObject().AddComponent<TimeDebug>();
            m.name = "TimeDebugger";
        }
    }

    public static void Stop(string tag) {
        recordings[tag] = Time.realtimeSinceStartup - recordings[tag];
        if (m == null) {
            m = new GameObject().AddComponent<TimeDebug>();
        }
    }

    void Update() {
        // save time changes into array
        times = new TaggedTime[recordings.Count];
        int i = 0;
        foreach (System.Collections.Generic.KeyValuePair<string, float> key in recordings) {
            times[i++] = new TaggedTime(key.Key, key.Value);
        }
        i = 0;
        countedTimes = new int[times.Length];
        foreach (System.Collections.Generic.KeyValuePair<string, int> key in counted) {
            countedTimes[i++] = key.Value;
        }
    }
}