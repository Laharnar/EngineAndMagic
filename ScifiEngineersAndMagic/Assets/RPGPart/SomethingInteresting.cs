using System.Collections.Generic;
using UnityEngine;

public class SomethingInteresting : MonoBehaviour {
    internal int numOfParticipants { get; private set; }
    List<Bypasser> participants = new List<Bypasser>();
    public float timeInvestment; // how much time will npc invest into this

    void Awake() {
        GlobalPOI.AddAsPoi(this);
    }

    public void NewParticipant(Bypasser c) {
        numOfParticipants++; // TODO: keep reference to those bypassers
        participants.Add(c);
    }

    public void LostInterest(Bypasser c) {
        int i = participants.IndexOf(c);
        numOfParticipants--;
        participants.RemoveAt(i);
        if (numOfParticipants < 0) {
            Debug.Log("negative participants, too many removals");
            numOfParticipants = 0;
        }
    }
}