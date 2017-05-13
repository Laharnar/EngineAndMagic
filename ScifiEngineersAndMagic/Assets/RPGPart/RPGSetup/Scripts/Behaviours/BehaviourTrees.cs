using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class AllInRange: DataNode {
    public override NodeStatus Tick() {
        // marks which ai's are in range from some object
        // test it
        List<Ai> all = (List<Ai>)GlobalAi.allAi;
        Transform compared = (Transform)data["position"];
        float range = (float)data["searchRange"];

        bool[] isIn = new bool[all.Count];
        bool changes = false;
        for (int i = 0; i < all.Count; i++) {
            if (all[i] == null) continue;
            if (SqDistance(all[i].transform.position, compared.position) < Mathf.Pow(range, 2)) {
                isIn[i] = true;
                changes = true;
            }else
                isIn[i] = false;
        }   
        data["filtered"] = isIn;
        if (!changes)
            return NodeStatus.FAILURE;
        return NodeStatus.IGNORE;
    }

    float SqDistance(Vector3 v1, Vector3 v2) {
        return (Mathf.Pow(v2.x - v1.x, 2) + Mathf.Pow(v2.y - v1.y, 2));
    }
}

internal class LookAtTarget : DataNode {
    public override NodeStatus Tick() {
        ((Transform)data["transform"]).transform.right = (((Vector3)data["targetPos"])) - (((Transform)data["transform"])).position;;

        return NodeStatus.IGNORE;
    }
}

internal class LookAtRelativePosition : DataNode {
    public override NodeStatus Tick() {
        ((Transform)data["transform"]).transform.right = (((Vector3)data["targetPos"])) + (((Transform)data["transform"])).localPosition;

        return NodeStatus.IGNORE;
    }
}

internal class ActionNode : ActionLeafNode {
    public ActionNode(Action action) : base(action) {
    }

    public override NodeStatus Tick() {
        action();
        return NodeStatus.IGNORE;
    }
}

internal class WaitAction : FuncLeafNode {
    public WaitAction(Func<bool> action) : base(action) {
    }

    public override NodeStatus Tick() {
        if (action()) {
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }
}

internal class Inverter : Decorator {
    public override NodeStatus Tick() {
        NodeStatus ns = child.Tick();
        if (ns == NodeStatus.RUNNING) return ns;
        return (NodeStatus)(((int)ns + 1) % 2); // success<>fail, 
    }
}

internal class CheckIsInRange : DataNode {
    public override NodeStatus Tick() {
        if (Vector3.Distance(((Ai)data["ai"]).transform.position, ((Vector3)data["targetPos"]))
            < ((float)data["range"])) {
            
            return NodeStatus.SUCCESS;
        }
        //((DataNode)child).Append(data);
        return child.Tick();
    }
}

internal class Failure : DataNode {
    public override NodeStatus Tick() {
        return NodeStatus.FAILURE;
    }
}

internal class DebugMsg : DataNode {
    string msg;
    public DebugMsg(string msg) {
        this.msg = msg;
    }

    public override NodeStatus Tick() {
        Debug.Log(msg);
        if (child == null) return NodeStatus.IGNORE;

        NodeStatus status = child.Tick();
        return status;
    }
}

internal class SetTriggerAnimation : DataNode {
    public override NodeStatus Tick() {
        ((Animator)data["animator"]).SetTrigger(((string)data["triggerChoice"]));
        return NodeStatus.SUCCESS;
    }
}

internal class InfiniteRepeater : DataNode {
    public override NodeStatus Tick() {
        child.Tick();// will always run, but always ignroe it in the tree
        return NodeStatus.IGNORE;
    }
}

internal class Repeater : DataNode {
    public override NodeStatus Tick() {
        if ((int)data["counter"] > 0) {
            data["counter"] = (int)data["counter"] - 1;
            child.Tick();
            return NodeStatus.RUNNING;
        }
        return NodeStatus.FAILURE;
    }
}

internal class SetTime : DataNode {
    public override NodeStatus Tick() {
        data["ctime"] = Time.time + (float)data["rate"];
        return NodeStatus.SUCCESS;
    }
}

internal class Timer : DataNode{
    public override NodeStatus Tick() {
        if ((float)data["ctime"] > Time.time) {
            return NodeStatus.RUNNING;
        }
        return NodeStatus.SUCCESS;
    }
}

internal class WaitForAnimationDone : DataNode {
    public override NodeStatus Tick() {
        if (!((Animator)data["animator"]).GetCurrentAnimatorStateInfo(0)
            .IsName((string)data["animationName"])) {
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }
}

internal class MoveNearDestination : DataNode {
    public override NodeStatus Tick() {
        ((Ai)data["ai"]).WalkNear(((Vector3)data["targetPos"]), ((float)data["range"]), null);
        return ((Ai)data["ai"]).crowds.busy ? NodeStatus.RUNNING : NodeStatus.SUCCESS;
    }
}

internal class FindNearestTarget : DataNode {

    public FindNearestTarget() {
        if (data == null) {
            data = new Dictionary<string, object>();
        }
        data["target"] = null;
        data["targetPos"] = null;
    }

    public override NodeStatus Tick() {
        
        //if (!data.ContainsKey("target") || (data.ContainsKey("target") && (Transform)data["target"] == null)) {
            Ai objective = GlobalAi.ChoseNearest((Transform)data["transform"], (string)data["alliance"], "!", data.ContainsKey("filtered") == false ? null : (bool[])data["filtered"]);//GlobalAi.ChoseTargetByPriorityImportance((string)data["alliance"], "!", data.ContainsKey("filtered") == false ? null : (bool[])data["filtered"]);
            data["target"] = null;
            if (objective != null)
                data["target"] = objective.transform;
        //}

        if (((Transform)data["target"]) != null) {
            data["targetPos"] = ((Transform)data["target"]).position;
        }

        return data["target"] == null ? NodeStatus.FAILURE : NodeStatus.SUCCESS;
    }
}

public abstract class DataNode : Decorator{
    public Dictionary<string, object> data;

    public DataNode() {
        data = new Dictionary<string, object>();
        type = "DataNode";
    }

    public override TreeNode Init(Dictionary<string, object> data) {
        this.data = data;
        if (data == null) {
            data = new Dictionary<string, object>();
        }
        base.Init(data);
        return this;
    }

    public void Append(Dictionary<string, object> ndata) {
        foreach (string key in ndata.Keys.ToList()) {
            if (!data.ContainsKey(key))
                data.Add(key, ndata[key]);
            else 
                data[key] = ndata[key];
        }
    }
}

class DataTransferSequence : Sequence {
    public Dictionary<string, object> data;

    public DataTransferSequence() {
        data = new Dictionary<string, object>();
    }

    public override TreeNode Init(Dictionary<string, object> data) {
        this.data = new Dictionary<string, object>();
        return base.Init(data);
    }

    public override NodeStatus Tick() {
        // keep transfering data front starting to end node
        for (int i = 0; i < children.Length; i++) {
            if (children[i].type == "DataNode") {
                ((DataNode)children[i]).Append(data);
            }

            NodeStatus status = children[i].Tick();
            if (children[i].type == "DataNode") {

                foreach (string key in ((DataNode)children[i]).data.Keys.ToList()) {
                    data[key] = ((DataNode)children[i]).data[key];
                }
            }

            if (status != NodeStatus.SUCCESS 
                && status != NodeStatus.IGNORE) // skip succeses
                return status;
        }
        return NodeStatus.SUCCESS;
    }
}

internal class RepeatUntilFail : Decorator {
    public override NodeStatus Tick() {
        NodeStatus status = child.Tick();
        if (status == NodeStatus.FAILURE)
            return NodeStatus.SUCCESS;
        return status;
    }
}


public abstract class TreeNode {
    public string type = "TreeNode";
    
    public abstract TreeNode Init(Dictionary<string, object> data);
    public abstract NodeStatus Tick();

    
}

public enum NodeStatus {
    SUCCESS,
    FAILURE,
    RUNNING,
    IGNORE
}

public abstract class Decorator : TreeNode {
    public TreeNode child;

    public TreeNode SetChild(TreeNode child) {
        this.child = child;
        return this;
    }

    internal TreeNode SetChecks(Decorator condition) {
        condition.child = this; // reversed set child
        return condition;
    }

    public override TreeNode Init(Dictionary<string, object> data) {
        if (child != null) child.Init(data);
        return this;
    }
}

public abstract class Compositor : TreeNode {
    public TreeNode[] children;

    public override TreeNode Init(Dictionary<string, object> data) {
        foreach (TreeNode child in children) {
            child.Init(data);
        }
        return this;
    }

    public TreeNode SetChildren(params TreeNode[] children) {
        this.children = children;
        return this;
    }
}

public class Sequence : Compositor {

    public override NodeStatus Tick() {
        for (int i = 0; i < children.Length; i++) {
            NodeStatus status = children[i].Tick();
            if (status != NodeStatus.SUCCESS && status != NodeStatus.IGNORE) // skip succeses
                return status;
        }
        return NodeStatus.FAILURE;
    }
}

public class Selector : Compositor {
    public override NodeStatus Tick() {
        for (int i = 0; i < children.Length; i++) {
            NodeStatus status = children[i].Tick();
            if (status != NodeStatus.FAILURE && status != NodeStatus.IGNORE) // skip fails
                return status;
        }
        return NodeStatus.FAILURE;
    }
}
internal abstract class FuncLeafNode : TreeNode {

    public Func<bool> action;

    public FuncLeafNode(Func<bool> action) {
        this.action = action;
        if (action == null) {
            Debug.LogError("ActionLeafNode: action is null");
        }
    }

    public override TreeNode Init(Dictionary<string, object> data) {
        return this;
    }

}

internal abstract class ActionLeafNode : TreeNode {
    public Action action;

    public ActionLeafNode(Action action) {
        this.action = action;
        if (action == null) {
            Debug.LogError("ActionLeafNode: action is null");
        }
    }

    public override TreeNode Init(Dictionary<string, object> data) {
        return this;
    }
}