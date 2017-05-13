using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

    public float detection;
    public float turnSpeed;
    public float turnTime;

    public Transform rotationTarget;
    public Vector3 lookAt;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        LookAt(lookAt);
    }

    void FollowTarget(Vector3 target) {
        lookAt = target;
    }

    void LookAt(Vector3 point) {
        lookAt = point;
        rotationTarget.Rotate(Vector3.forward, RotationFunction(true));
    }

    float RotationFunction(bool reset) {
        if (reset)
            turnTime = Time.time;
        return Vector2.Angle(rotationTarget.right, lookAt - rotationTarget.position);
    }
}
