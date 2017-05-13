using UnityEngine;
using System.Collections;

public class BackgroundRaycast : MonoBehaviour {

    internal static BackgroundRaycast m { get; private set; }

    public bool raycastOnlyOnMouse = false;

    internal RaycastHit2D hit;

    /** 
     * Vector2 snapped = hit.point;
        snapped.x = snapped.x - snapped.x % (platformSize);
        snapped.y = snapped.y - snapped.y % (platformSize);
     * */

    // Use this for initialization
    void Awake () {
        m = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (!raycastOnlyOnMouse || 
            (raycastOnlyOnMouse && Input.GetKeyDown(KeyCode.Mouse0))) {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);

            if (!hit || hit.transform.name == "InvisBackgroundCollider") {
                Debug.Log("off platform test ");
            }
        }
    }
}
