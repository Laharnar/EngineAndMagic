using System;
using System.Collections;
using System.Collections.Generic;

public class Path<Node> : IEnumerable<Node> {

    public Node LastStep { get; private set; } // this node
    public Path<Node> PreviousSteps { get; private set; }
    public double TotalCost { get; private set; }
    public int PathLength { get; private set; } // 1 less than the number of nodes in path

    private Path(Node lastStep, Path<Node> previousSteps, double totalCost, int pathLength) {
        LastStep = lastStep;
        PreviousSteps = previousSteps;
        TotalCost = totalCost;
        PathLength = pathLength;
    }

    public Path(Node start) : this(start, null, 0, 0) { }
    public Path<Node> AddStep(Node step, double stepCost) {
        return new Path<Node>(step, this, TotalCost + stepCost, PathLength+1);
    }

    public IEnumerator<Node> GetEnumerator() {
        for (Path<Node> p = this; p != null; p = p.PreviousSteps)
            yield return p.LastStep;
    }

    public List<Node> ToList() {
        List<Node> nodes = new List<Node>();
        for (Path<Node> p = this; p != null; p = p.PreviousSteps) {
            nodes.Add(p.LastStep);
        }
        // at the moment, last node is first one, so we reverse it
        nodes.Reverse();
        return nodes;
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
    
}