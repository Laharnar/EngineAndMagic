using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 16.2.2017
/// modified for another fast draw editor.
/// 
/// 29.1.2017
/// adds tiles of selected color.
/// tiles can be removed
/// also set their sprite layer
/// 
/// v2
/// added snap setting
/// </summary>
public class FastEditor : MonoBehaviour {

    public int spriteLayer = 50;
    public Color[] spriteColors;
    int _activeColor = -1;
    public int activeColor {
        get { return _activeColor; }
        set { _activeColor = Mathf.Clamp(value, 0, spriteColors.Length) % spriteColors.Length; }
    }

    public Transform pref;
    public Transform parentTo;

    // v2
    public float snap=0.01f;
    public float scaling =1;

    public Transform brush;
    public bool inited = false;

    // Use this for initialization
    public void Init () {
        // brush cant have collision to prevent removal
        brush = Instantiate(pref);
        DestroyImmediate(brush.GetComponent<BoxCollider2D>());
        //brush.GetComponent<BoxCollider2D>().enabled = false;
        brush.parent = parentTo;
        brush.name = "brush";
        inited = true;
    }

    void Start() {
        if (inited == false)
            Init();
    }

    // Update is called once per frame
    void Update () {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);
        if (hit) {
            Editing(hit.transform, hit.point, Input.GetMouseButton(0), Input.GetMouseButton(1));
        }
    }

    public void Editing(Transform hit, Vector3 point, bool mouse0, bool mouse1) {
        if (!hit) return;
        if (brush == null) { Debug.Log("brush is null"); return; }
        if (activeColor == -1) { Debug.Log("assign color"); return; }

        // brush
        if (hit) {
            Vector3 pos = point;
            pos.x = pos.x - pos.x % snap + scaling / 2;
            pos.y = pos.y - pos.y % snap + scaling / 2;
            pos.z = 0;
            brush.position = pos;
            brush.localScale = Vector3.one * scaling;

            float a = spriteColors[activeColor].a * 0.3f;
            brush.GetComponent<SpriteRenderer>().color = new Color(spriteColors[activeColor].r, spriteColors[activeColor].g, spriteColors[activeColor].b, a);
            brush.GetComponent<SpriteRenderer>().sortingOrder = spriteLayer;
        }

        if (mouse1) {
            // remove
            Delete(hit.transform);
        } else if (mouse0) {
            // add
            Draw(hit.transform, point);
        }
    }

    private void Delete(Transform hit) {
        if (hit.name.Contains("fastMapItem")) {
            DestroyImmediate(hit.gameObject);
        }
    }

    public void Draw(Transform hit, Vector3 point) {
        if (hit.name == "fastEditGround") {
            Transform obj = Instantiate(pref);
            Vector3 pos = point;
            pos.x = pos.x - pos.x % snap;
            pos.y = pos.y - pos.y % snap;
            pos.z = 0;
            obj.position = pos;


            obj.localScale *= scaling;
            obj.GetComponent<SpriteRenderer>().color = spriteColors[activeColor];
            obj.GetComponent<SpriteRenderer>().sortingOrder = spriteLayer;
            obj.parent = parentTo;
        }
    }
}
