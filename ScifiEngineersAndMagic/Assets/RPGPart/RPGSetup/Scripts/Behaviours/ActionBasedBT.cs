using System;
using System.Collections.Generic;
using UnityEngine;


public class RunTimes : FBDecorator {
    int countLeft;

    public override T Final<T>(string name) {
        countLeft = 1;
        return base.Final<T>(name);
    }

    public T Final<T>(string name, int count) where T:RunTimes {
        countLeft = count;
        return base.Final<T>(name);
    }

    public override void Tick() {
        status = FBStatus.Failure;
        if (init) {
            if (countLeft > 0) {
                countLeft--;
                child.Tick();
                status = child.status;
            }
            status = FBStatus.Success;
        }
    }
}

public class Encapsulator : BNode {
    Func<bool> f;
    private FBStatus statusIfFalse;
    private FBStatus statusIfTrue;

    public T Final<T>(string name, Func<bool> f, FBStatus statusIfTrue = FBStatus.Success, FBStatus statusIfFalse = FBStatus.Failure) where T: Encapsulator {
        this.f = f;
        this.statusIfTrue = statusIfTrue;
        this.statusIfFalse = statusIfFalse;
        init = true;
        return (T)this;
    }

    public override void Tick() {
        if (!init) { Debug.Log("Encapsulator not init."); return; }
        if (f()) status = statusIfTrue;
        else status = statusIfFalse;
    }
}

public class Checker : FBDecorator {
    // runs checks before running children

    List<Func<FBStatus>> checks;
    bool checksPass;

    public Checker() {
        checks = new List<Func<FBStatus>>();
    }

    public T AddCheck<T>(Func<FBStatus> f) where T : Checker {
        if (f != null)
            this.checks.Add(f);
        return (T)this;
    }

    public override void Tick() {
        // if checks pass, continue with child
        status = FBStatus.Failure;
        checksPass = false;
        if (!init) return;

        if (!Pass()) return;

        checksPass = true;
        child.Tick();
        status = child.status;

    }

    bool Pass() {
        for (int i = 0; i < checks.Count; i++) {
            status = checks[i]();
            if (status != FBStatus.Success) {
                return false;
            }
        }
        return true;
    }
}

public class FBTokenRing : FBCompositor {

    int i;

    public T Final<T>(string name, int startIndex) where T: FBTokenRing {
        i = startIndex;
        base.Final<T>(name);
        return (T)this;
    }

    public override void Tick() {
        // run a single child that has the token, until it succeeds
        if (children.Length > 0) {
            status = FBStatus.Running;
            children[i].Tick();

            if (children[i].status == FBStatus.Success) {
                if ((i + 1) % children.Length == 0) { status = FBStatus.Success; }
                i = (i+1) % children.Length;
            }
            return;
        }
        status = FBStatus.Failure;
    }
}

public class FBParalel : FBCompositor {
    public override void Tick() {
        // run everything at the same time, regardless of results
        for (int i = 0; i < children.Length; i++) {
            children[i].Tick();
        }
        status = FBStatus.Success;
    }
}

public class FBSequence : FBCompositor {
    public override void Tick() {
        bool allPassed = true;
        // act as AND
        for (int i = 0; i < children.Length; i++) {
            children[i].Tick();
            if (children[i].status == FBStatus.Success)
                continue;
            if (children[i].status == FBStatus.Running) {
                status = FBStatus.Running;
                allPassed = false;
                return;
            }
            if (children[i].status == FBStatus.Failure) {
                status = FBStatus.Failure;
                allPassed = false;
                return;
            }
        }
        if (allPassed)
            status = FBStatus.Success;
        else status = FBStatus.Running;
    }
}

public class FBSelector : FBCompositor {
    public override void Tick() {
        // act as OR
        for (int i = 0; i < children.Length; i++) {
            children[i].Tick();
            if (children[i].status == FBStatus.Failure) 
                continue;
            if (children[i].status == FBStatus.Running) {
                status = FBStatus.Running;
                return;
            }
            if (children[i].status == FBStatus.Success) {
                status = FBStatus.Success;
                return;
            }
        }
        status = FBStatus.Failure;
    }

    internal object Do<T>(object findPointOfInterest, object walkToPointOfInterest) {
        throw new NotImplementedException();
    }
}


public abstract class FBDecorator : BNode {
    // allows a child
    public BNode child;

    internal T Do<T>(object f) where T : FBDecorator {
        switch (Classificator.Classificate(f)) {
            case Classificator.NodeValue.BNode:
                child = (BNode)f;
                break;
            case Classificator.NodeValue.FuncStatus:
                child = new FBNode().SetAction<FBNode>((Func<FBStatus>)f);
                break;
            default:
                Debug.Log("undefined classification");
                break;
        }
        return (T)this;
    }

}

public abstract class FBCompositor : BNode {
    // allows multiple children
    public BNode[] children;

    internal T Do<T>(params object[] f) where T : FBCompositor {
        this.children = new BNode[f.Length];
        for (int i = 0; i < f.Length; i++) {
            switch (Classificator.Classificate(f[i])) {
                case Classificator.NodeValue.BNode:
                    children[i] = (BNode)f[i];
                    break;
                case Classificator.NodeValue.FuncStatus:
                    children[i] = new FBNode().SetAction<FBNode>((Func<FBStatus>)f[i]).Final(name+"_fnstatus");
                    break;
                case Classificator.NodeValue.FuncBool:
                    children[i] = new Encapsulator().Final<Encapsulator>(name+"_fn", (Func<bool>)f[i]);
                    break;
                default:
                    Debug.Log("undefined classification");
                    break;
            }
        }
        return (T)this;
    }
    
}

public static class Classificator {
    public enum NodeValue {
        BNode,// default
        FuncStatus,
        FuncBool
    }

    public static NodeValue Classificate(object value) {
        if (value.GetType() == typeof(Func<FBStatus>))
            return NodeValue.FuncStatus;
        if (value.GetType() == typeof(Func<bool>))
            return NodeValue.FuncBool;
        else return NodeValue.BNode;
    }
}

public class FBNode : BNode {
    // node with function based support

    Func<FBStatus> f;

    public override T Final<T>(string name) {
        if (f == null) {
            Debug.Log("Function isn't assigned on node" + name + ".");
        }
        return (T)base.Final<T>(name);
    }

    public virtual T SetAction<T>(Func<FBStatus> f) where T : FBNode {
        this.f = f;
        return (T)this;
    }

    public override void Tick() {
        status = FBStatus.Failure;
        if (!init) { Debug.Log("FBNode not init"); return; }
        status = f();
    }
}

public abstract class BNode {
    // base of all nodes

    public FBStatus status { get; protected set; }

    public string name;
    public bool init = false;
    //public abstract Type type { get { return typeof(BNode); } }

    public virtual T Final<T>(string name) where T : BNode {
        this.name = name;
        init = true;

        return (T)this;
    }

    public virtual BNode Final(string name){
        this.name = name;
        init = true;

        return this;
    }

    public abstract void Tick();
}



public enum FBStatus {
    Success,
    Failure,
    Running
}
