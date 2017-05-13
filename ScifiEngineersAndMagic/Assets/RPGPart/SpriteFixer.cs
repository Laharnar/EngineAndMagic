using UnityEngine;
using System.Collections;

public class SpriteFixer : MonoBehaviour {
    public SpriteRenderer sprite;

    void Awake() {
        sprite  = sprite == null ? GetComponent<SpriteRenderer>() : sprite;
    }
    void LateUpdate() {
            int layer = (int)Camera.main.WorldToScreenPoint(sprite.bounds.min).y ;
            sprite.sortingOrder = layer;
    }
}
