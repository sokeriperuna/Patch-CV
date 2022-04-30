using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ButtonObject))]
public class ButtonLogic : LogicMaster
{
    public override string GetNormalName()
    {
        return "Button (off)";
    }
    public override string GetInvertedName()
    {
        return "Button (on)";

    }

    protected ButtonObject buttonObj;

    protected void Awake()
    {
        buttonObj = GetComponent<ButtonObject>();
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
        base.UpdateOutputState(); // base.UpdateOutputState records if wasInverted has changed
        outputState = buttonObj.Pressed;
    }
}
