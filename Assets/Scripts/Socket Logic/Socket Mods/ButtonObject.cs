using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ButtonLogic))]
[ExecuteInEditMode]


public class ButtonObject : ObjectMaster
{
    public override string GetNormalName()
    {
        return "Button (off)";
    }
    public override string GetInvertedName()
    {
        return "Button (on)";
    }

    // Implement mass requirement and other behaviour etc. here

    protected bool isPressed = false;
    ButtonSpring spring;
    protected override void Awake() 
    {
        base.Awake();
        spring = GetComponent<ButtonSpring>();
    }

    public override void OnDisconnect(PatchModifier mod)
    {
        base.OnDisconnect(mod);
        mod.Inverted = false;
    }

    void FixedUpdate()
    {
        isPressed = spring.pressed;
        Inverted = spring.pressed;
    }

    public bool Pressed { get { return isPressed; } } // Read only accessor for isPressed
}
