using UnityEngine;
using System.Collections;

// collide with level by keeping everything in player aligned
public class KeepAligned : MonoBehaviour {

    public Transform target;

    public Transform[] fix;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (target.localPosition.x != 0) {
            transform.Translate(target.localPosition.x*Vector3.right*3f);

            for (int i = 0; i < fix.Length; i++) {
                fix[i].localPosition = new Vector3(0, fix[i].localPosition.y);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("check " +other.transform);
        Vector3 point = other.transform.position;
        transform.GetComponent<Rigidbody2D>().MovePosition((transform.position-point).normalized*2);
    }
}
