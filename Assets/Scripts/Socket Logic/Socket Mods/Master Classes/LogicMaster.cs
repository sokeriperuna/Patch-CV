using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicMaster: PatchModifier
{
    public override string GetNormalName()
    {
        return "LogicMaster (N)";
    }
    public override string GetInvertedName()
    {
        return "LogicMaster (I)";

    }
    protected bool outputState;

    protected virtual void Start()
    {
        outputState = false;
    }

    protected virtual void Update() // We update the outputstate every frame
    {
        UpdateOutputState();
    }

    protected virtual void UpdateOutputState()
    {
        // Update outputstate
    }

    public override void OnDisconnect(PatchModifier mod)
    {
        base.OnDisconnect(mod);
        outputState = false;
    }

    public bool Output { get { return outputState; } } // Read only accessor for isPressed
}
