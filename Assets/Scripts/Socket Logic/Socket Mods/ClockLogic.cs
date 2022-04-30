using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ButtonObject))]
public class ClockLogic : LogicMaster
{
    protected ButtonObject buttonObj;

    protected void Awake()
    {
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void UpdateOutputState()
    {
        base.UpdateOutputState();
        outputState = buttonObj.Pressed;
    }
}
