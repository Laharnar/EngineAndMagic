using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generates grid for graph.
 * How to use: Assign graph, width and height
 * Disable it if you don't want pathfinding. Units will automaticaly work without it.
 * */
public class WaypointGrid : MonoBehaviour {

    public int width, length;
    public Graph graph;

    public Component[] componentTypes;
    public float step;

    public bool areNodesTrigger = true;
    public Sprite nodeSprite;


    // Use this for initialization
    void Start() {
        graph.width = width;
        graph.height = length;
        graph.worldStep = step;

        // get types of components that should be attached to grid items
        List<Type> compTypes = new List<Type>();
        for (int i = 0; i < componentTypes.Length; i++) {
            if (componentTypes[i] != null) {
                compTypes.Add(componentTypes[i].GetType());
            }
        }

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        // create grid and run mod on every item
        graph.grid = new Transform[width * length];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                graph.grid[j * width + i] = new GameObject("grid_" + j + "_" + i, compTypes.ToArray()).transform;
                // Note: if you will add option to change parent, make sure you also fix it at OnInitDone
                graph.grid[j * width + i].parent = transform;
                graph.grid[j * width + i].localPosition = new Vector3(i * step, j * step, 0);
                graph.grid[j * width + i].GetComponent<SpriteRenderer>().sprite = nodeSprite;
                graph.grid[j * width + i].GetComponent<Collider2D>().isTrigger = areNodesTrigger;
            }
        }
        graph.OnInitDone(width, length, transform.position);
        Debug.Log("graph ready");
    }
}



