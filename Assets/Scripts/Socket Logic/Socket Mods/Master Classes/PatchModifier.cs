using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextGenerator))]
public abstract class PatchModifier : MonoBehaviour
{

    protected bool isInverted = false; // Basic property about the state of a mod. Logic sockets do not use this, usually.

    // This is nothing more than a class to categorize sub-classes

    /// <summary>
    /// Override this when you need to do stuff when disconnecting a patch modifier. NOTE: it is the patch modifiers responsibility to make sure proper initialization happnes and the object is reset
    /// </summary>
    public virtual void OnDisconnect(PatchModifier mod)
    {
        /// Insert deinitialization stuff here when overriding.
    }

    public virtual void OnConnect(PatchModifier mod)
    {
        /// Insert initialization stuff here when overriding.
    }

    public bool Inverted { get { return isInverted; } set { isInverted = value; } }

    public abstract string GetNormalName();
    public abstract string GetInvertedName();

}
