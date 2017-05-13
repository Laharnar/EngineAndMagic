using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

// map that is getting edited in play mode
[CustomEditor(typeof(FastEditor))]
public class FastEditorEditor : Editor {

    bool customEditing = false;
    FastEditor source;

    internal static bool drawing;
    private bool ctrl; // ctrl+mouse: delete

    void OnSceneGUI() {

        source = (FastEditor)target;
        if (source.inited  == false) {
            source.Init();
        }
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        KeyCode currentCode = Event.current.keyCode;

        // keyboard
        ReservedKeys(currentCode); // handle editor reserved keys
        if (customEditing) {
            NumberKeys(currentCode); // set brushes

            BrushKeys(currentCode); // set brushes

            //mouse
            MouseClicks();
            if (Event.current.type == EventType.layout) {
                HandleUtility.AddDefaultControl(controlID);
            }
        }

    }

    private void NumberKeys(KeyCode currentCode) {
        // Set operations with number keys
        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.Alpha1) {
                Debug.Log("Does nothing");
            }
            if (Event.current.keyCode == KeyCode.Alpha2) {
                Debug.Log("Does nothing");
            }
        }
    }

    
    private void MouseClicks() {

        // start drawing
        if (Event.current.type == EventType.MouseDown) {
            drawing = true;
        }
        
        bool mouse0 = Event.current.button == 0;
        bool mouse1 = mouse0 && ctrl;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        //HandleUtility.
        //RaycastHit hit;
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (hit) {
            source.Editing(hit.transform, hit.point, 
                drawing && mouse0,
                drawing && mouse1);
        }
        /*// keep drawing, make sure items arent already drawn there in current stroke.
        if (Event.current.type == EventType.MouseDrag || drawing) {
            
        } */
        if (Event.current.type == EventType.MouseUp) {
            drawing = false;
        }
    }

    private void BrushKeys(KeyCode currentCode) {
        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.W) {
                source.activeColor++;
                Debug.Log("increment color : "+source.activeColor);
            } else if (Event.current.keyCode == KeyCode.S) {
                source.activeColor--;
                Debug.Log("decrement color : " + source.activeColor);
            }
        }
    }

    private void ReservedKeys(KeyCode currentCode) {
        if (!drawing) {
            if (Event.current.type == EventType.KeyDown) {
                if (currentCode == KeyCode.Tab) {
                    Debug.Log("tab");
                    customEditing = !customEditing;
                }
                // first press the special keys, then mouse
                if (currentCode == KeyCode.LeftControl)
                    ctrl = true;
            }
            if (Event.current.type == EventType.KeyUp) {
                if (currentCode == KeyCode.LeftControl)
                    ctrl = false;
            }
        }
    }
}
