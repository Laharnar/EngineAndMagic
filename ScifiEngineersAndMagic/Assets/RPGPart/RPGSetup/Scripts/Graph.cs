using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Graph : MonoBehaviour {

    // path calculation
    [SerializeField]
    Vector3 offsetMatrix;

    // graph data
    public Transform[] grid;
    [SerializeField]
    internal int width;
    [SerializeField]
    internal int height;
    [SerializeField]
    internal float worldStep; // assigned from grid, step used to generate the grid


    // generated nodes
    public GNode[] nodes;
    public Vector3 gridStart;

    internal void OnInitDone(int width, int height, Vector3 gridStart) {
        this.gridStart = gridStart;
        // generates nodes after data was fully received(from grid/other)
        // Note: graph also requires other data, like step of nodes, to properly calculate offset matrix
        // you can work witout matrix by removing it and calculating distance to target
        nodes = new GNode[grid.Length];
        for (int i = 0; i < grid.Length; i++) {
            nodes[i] = new GNode(grid[i], i, width, height);
        }
        // finds neighbours
        for (int i = 0; i < nodes.Length; i++) {
            // Note: costs should be close to real costs
            nodes[i].AddNeighbours(1, 1.4, nodes);
        }
    }

    /// <summary>
    /// Constructs a path to goal. 
    /// - includes goal node
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public Path<GNode> FindPath(Vector3 start, Vector3 goal) {
        // this ensures path will be found as long as there are any nodes
        if (nodes.Length == 0) { 
            Debug.LogError("Can't calculate path without nodes. (Err: 0 nodes)");
            return null;
        }

        // find where should manhattan dist be applied from - first node + target pos(snapped)
        offsetMatrix = gridStart + goal - new Vector3(goal.x%1, goal.y % 1, goal.z % 1);

      //  Debug.Log("n: "+start+" "+goal);
        // Do A*, applying offset to costs every time
        GNode startNode = NearestNode(start, nodes);
        GNode endNode = NearestNode(goal, nodes);
//        Debug.Log("nodes = start:"+startNode.transform.position+" end:"+ endNode.transform.position);
        Debug.DrawLine(startNode.transform.position, start, Color.red, 3);
// doesn't need drawing        Debug.DrawLine(endNode.transform.position, goal, Color.red, 5);
        return AStarPathfind(startNode, endNode, offsetMatrix, nodes,
            Distance, estimate);
        
    }

    internal List<GNode> FindRandomPath(Vector3 start) {
        return (List<GNode>)FindPath(start, transform.position + new Vector3(UnityEngine.Random.Range(0, width * worldStep), UnityEngine.Random.Range(0, height* worldStep))).GetEnumerator();
    }

    private static GNode NearestNode(Vector3 compare, GNode[] nodes) {
        int closest = -1;
        float smallestDist = 0;
        for (int i = 0; i < nodes.Length; i++) {
            if (nodes[i].transform == null) continue;
            if (closest == -1) {
                closest = i;
                smallestDist = (nodes[closest].transform.position - compare).sqrMagnitude;
                continue;
            }
            float dist = (nodes[i].transform.position - compare).sqrMagnitude;
            if (dist < smallestDist) {
                closest = i;
                smallestDist = dist;
            }
        }
        return closest == -1 ? null : nodes[closest];
    }

    private Path<GNode> AStarPathfind(
            GNode start, 
            GNode goal, 
            Vector3 offsetCost, 
            GNode[] nodes,
            Func<GNode, GNode, double> distance,
            Func<GNode, double> estimate) {

        var closed = new HashSet<GNode>();
        var queue = new PriorityQueue<double, Path<GNode>>();
        queue.Enqueue(0, new Path<GNode>(start));
        while (!queue.IsEmpty) {
            var path = queue.Dequeue();
            if (closed.Contains(path.LastStep))
                continue;
            if (path.LastStep.Equals(goal))
                return path;
            closed.Add(path.LastStep);
            // note: would make more sense if you estimated everytime, instead of only on init of neighbours
            foreach (GNode.Neighbour n in path.LastStep.neighbours) {
                if (n!= null && n.node.transform != null) {
                    double d = distance(path.LastStep, n.node);
                    var newPath = path.AddStep(n.node, d);
                    queue.Enqueue(newPath.TotalCost + estimate(n.node), newPath);
                }
            }
        }
        return null;
    }

    private double estimate(GNode n) {
        return n.manhattanDistance;
    }

    private double Distance(GNode lastStep, GNode n) {
        if (lastStep.transform == null || n.transform == null) {
            Debug.Log("null " + (lastStep.transform == null) +" "+ (n.transform == null));
        }
        return Vector3.Distance(lastStep.transform.position, n.transform.position);
    }

    private List<GNode> ReconstructPath(int[] cameFrom, int current, List<GNode> openSet) {
        List<GNode> totalPath = new List<GNode>();
        totalPath.Add(openSet[current]);
        foreach (int cur in cameFrom) {
            current = cameFrom[cur];
            totalPath.Add(openSet[current]);
        }
        return totalPath;
    }
    

}

[Serializable]
public class GNode { // graph node

    // important changing path data

    public double manhattanDistance;
        // manhattan dist to lower left corner in graph
        // apply offset matrix to get correct distances.

    // node data
    
    [SerializeField] internal int i;
    [SerializeField] internal Transform transform;
    [SerializeField] internal List<Neighbour> backTracking;
    [SerializeField] internal List<Neighbour> neighbours;

    // reference to grid size
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    

    public class Neighbour {
        [SerializeField] internal GNode node;
        [SerializeField] double cost;
        [SerializeField] private int x, y;
        [SerializeField] internal Transform transform;
        [SerializeField] private int i;

        public Neighbour(GNode node, Transform transform, int x, int y, int i, double cost) {
            this.node = node;
            this.transform = transform;
            this.x = x;
            this.y = y;
            this.i = i;
            this.cost = cost;
        }
    }

    public GNode(Transform transform, int i, int gridWidth, int gridHeight) {
        this.transform = transform;
        this.i = i;

        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;

        // get manhattan dist to lower-left corner in graph
        int x = i % gridWidth;
        int y = (i - x) / gridWidth;
        manhattanDistance = x + y;
    }

    /// <summary>
    /// Marks all 8 neighbour nodes around this node
    /// </summary>
    /// <param name="crossCost"></param>
    /// <param name="diagonalCost"></param>
    /// <param name="nodes"></param>
    internal void AddNeighbours(double crossCost, double diagonalCost, GNode[] nodes) {
        neighbours = new List<Neighbour>();
        int x = i % gridWidth;
        int y = (i-x) / gridWidth;

        // upperleft, clockwise diagonal, then top, clockwise cross 
        AddNodeIfValid((y + 1) * gridWidth + x - 1, x - 1, y + 1, diagonalCost, nodes);
        AddNodeIfValid((y + 1) * gridWidth + x + 1, x + 1, y + 1, diagonalCost, nodes);
        AddNodeIfValid((y - 1) * gridWidth + x + 1, x + 1, y - 1, diagonalCost, nodes);
        AddNodeIfValid((y - 1) * gridWidth + x - 1, x - 1, y - 1, diagonalCost, nodes);

        AddNodeIfValid((y + 1) * gridWidth + x, x, y + 1, crossCost, nodes);
        AddNodeIfValid(y * gridWidth + x + 1, x + 1, y, crossCost, nodes);
        AddNodeIfValid((y - 1) * gridWidth + x, x, y - 1, crossCost, nodes);
        AddNodeIfValid(y * gridWidth + x - 1, x - 1, y, crossCost, nodes);

    }

    /// <summary>
    /// adds nodes that are out of range as neighbours, and gives them cost.
    /// </summary>
    /// <param name="ii"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="cost"></param>
    /// <param name="nodes"></param>
    private void AddNodeIfValid(int ii, int x, int y, double cost, GNode[] nodes) {
        GNode n; 
        if (NodeInGridSizeBounds(out n, ii, gridWidth, nodes)) {
            neighbours.Add(new Neighbour(n, n.transform, x, y, ii, cost));
        }
    }

    private bool NodeInGridSizeBounds(out GNode n, int i, int gridWidth, GNode[] nodes) {
        n = (i < 0 || i >= nodes.Length) ? null : nodes[i];
        return n != null;
    }
}