using UnityEngine;
using System.Collections;

public class ScrollingCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {

        float d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f) {
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize - 0.5f, 1);
            // scroll in
        } else if (d < 0f) {
            // scroll out
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize + 0.5f, 40);
        }

    }
}
