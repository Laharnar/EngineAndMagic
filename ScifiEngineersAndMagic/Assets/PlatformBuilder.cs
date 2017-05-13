using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlatformBuilder : MonoBehaviour {

    public static PlatformBuilder m { get; private set; }

    internal float platformSize = 8;
    /// 1000/8 = +-120 platform width
    PlatformGrid grid;

    public Transform platformShadowPref;
    Transform createdShadowPlatform;



    void Awake() {
        m = this;
        grid = new PlatformGrid();
    }

    void Update() {
        // test building with e
        if (Input.GetKeyDown(KeyCode.E) && createdShadowPlatform == null) {
            Vector2 snapped = BackgroundRaycast.m.hit.point;
            snapped.x = snapped.x - snapped.x % (platformSize);
            snapped.y = snapped.y - snapped.y % (platformSize);
            createdShadowPlatform = (Transform)Instantiate(platformShadowPref,
                snapped, new Quaternion(),
                PlatformBuilder.m.transform);
        }
    }

    internal bool IsSlotFree(int x, int y) {
        return grid.IsSlotFree(x, y);
    }

    internal bool IsOnTopOfPlatform(Vector2 position) {
        for (int i = 0; i < grid.takenSlots.Count; i++) {
            int x = grid.takenSlots[i][0];
            int y = grid.takenSlots[i][1];
            Vector2 platformPos = grid.platforms[x, y].transform.position;
            Vector2 diff = ((Vector2)position - platformPos);
            if (-4 <= diff.x && diff.x < 4 &&
                -4 <= diff.y && diff.y < 4) {
                return true;
            }
        }
        return false;
    }

    internal bool IsOnCenterOfPlatform(Vector2 position) {
        for (int i = 0; i < grid.takenSlots.Count; i++) {
            int x = grid.takenSlots[i][0];
            int y = grid.takenSlots[i][1];
            Vector2 platformPos = grid.platforms[x, y].transform.position;
            Vector2 diff = ((Vector2)position - platformPos);
            if (diff == Vector2.zero) {
                return true;
            }
        }
        return false;
    }

    internal bool IsOnSideOfPlatforms(Vector2 position) {
        for (int i = 0; i < grid.takenSlots.Count; i++) {
            int x = grid.takenSlots[i][0];
            int y = grid.takenSlots[i][1];
            if (grid.platforms[x, y] != null) {
                Vector2 platformPos = grid.platforms[x, y].transform.position;
                Vector2 dirx = new Vector2(8, 0);
                Vector2 diry = new Vector2(0, 8);
                if (position + dirx == platformPos ||
                    position - dirx == platformPos ||
                    position + diry == platformPos ||
                    position - diry == platformPos) {
                    return true;
                }
            }
        }
        return false;
    }
    


    /// <summary>
    /// Supports up to +-1000 size of battlefield
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    internal void InsertIntoGrid(Platform platform, int x, int y) {
        grid.InsertIntoGrid(platform, x, y);
    }

    public class PlatformGrid {
        internal int size = 2000;

        // array for 2d content
        public Platform[,] platforms;
        public List<int[]> takenSlots;// x and y index of taken slots

        public PlatformGrid() {
            platforms = new Platform[size, size];
            takenSlots = new List<int[]>();
        }

        /// <summary>
        /// TODO: instead of directly inserting, create new grid twice the old size and begin
        /// inserting into it, while copying from old
        /// 
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void InsertIntoGrid(Platform platform, int x, int y) {
            if (x + size / 2 <= 0 || x + size / 2 >= size || y + size / 2 <= 0 || y + size / 2 >= size) {
                Debug.LogError("Item is out of range.");
                return;
            }
            platforms[y + size / 2, x + size / 2] = platform;
            takenSlots.Add(new int[2] { y + size / 2, x + size / 2 });
        }

        internal bool IsSlotFree(int x, int y) {
            return platforms[y + size / 2, x + size / 2] == null;
        }
    }
}

[Obsolete]
public class StackQue<T> {
    // allows addition and begging and end, while allowing indexed search


    int front = 0;
    int last = 0;
    int length = 0;
    public T[] items;

    public StackQue() {
        length = 0;
    }

    public void AddBack(T item) {
        if (last == length || front == 0) {
            Resize();
        }

        items[last] = item;
        last++;
    }

    public void AddFront(T item) {
        if (last == length || front == 0) {
            Resize();
        }

        front--;
        items[front] = item;
    }

    private void Resize() {
        // resizes my keeping new data in the middle
        T[] na = new T[items.Length * 2 + 1];
        int offset = na.Length / 2;
        for (int i = 0; i < length; i++) {
            na[i + offset] = items[i];
        }
        front += offset;
        last += offset;
    }



}