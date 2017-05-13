using UnityEngine;
using System.Collections;

// build item script
public class SnapToPlatformEdge : MonoBehaviour {

    public Transform builtItem;

	// Update is called once per frame
	void Update () {
        RaycastHit2D hit = BackgroundRaycast.m.hit;
        float platformSize = PlatformBuilder.m.platformSize;
        Vector2 snapped = hit.point;
        snapped.x = snapped.x - snapped.x % (platformSize);
        snapped.y = snapped.y - snapped.y % (platformSize);
        // allows building platforms near each other
        if (PlatformBuilder.m.IsOnSideOfPlatforms(snapped) ) {
            snapped = hit.point;
            snapped.x = snapped.x - snapped.x % (platformSize);
            snapped.y = snapped.y - snapped.y % (platformSize);
            transform.position = snapped;

            if (Input.GetMouseButtonDown(0)) {
                // prevents duplicates
                if (PlatformBuilder.m.IsSlotFree((int)snapped.x,(int)snapped.y)
                    == false)
                    return;

                Transform created = (Transform)Instantiate(builtItem, snapped, new Quaternion(), 
                PlatformBuilder.m.transform);
                if (created.GetComponent<Platform>()) {
                    PlatformBuilder.m.InsertIntoGrid(
                        created.GetComponent<Platform>(),
                        Mathf.FloorToInt(snapped.x),
                        Mathf.FloorToInt(snapped.y)
                        );
                }
                Destroy(gameObject);
            }
        }

    }
}
