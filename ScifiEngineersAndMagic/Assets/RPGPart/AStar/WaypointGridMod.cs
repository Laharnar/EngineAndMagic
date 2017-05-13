
using UnityEngine;

class WaypointGridMod : MonoBehaviour, IInstantiateMod {
    // note: data should be reference type if it will be changed

    void IInstantiateMod.OnBegin(params object[] data) {
        Transform[] grid;
        int width;
        int length;
    }

    void IInstantiateMod.OnDone(params object[] data) {
        Transform[] grid;
        int width;
        int length;
    }
    
    void IInstantiateMod.OnInstantiate(params object[] data) {
        Transform instantiated = (Transform)data[0];
        int j = (int)data[1];
        int i = (int)data[2];
        Transform[] grid = (Transform[])data[3];

    }
}