using UnityEngine;
using System.Collections;

public abstract class TeleportTrigger: WorldInteraction {

    Vector3 targetPosition; // where to move

    public void SetLocation(Vector3 loc) {
        targetPosition = loc;
    }

    public void TriggerTeleport(Transform obj) {
        obj.position = targetPosition;
    }
    
}

