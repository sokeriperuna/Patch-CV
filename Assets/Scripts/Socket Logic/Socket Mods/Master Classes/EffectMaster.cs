using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMaster : PatchModifier // This class is simply a set of instructions that the PatchSocket class uses to affect an object appriately.
{
    public override string GetNormalName()
    {
        return "";
    }
    public override string GetInvertedName()
    {
        return "";

    }

    protected virtual void Start()
    {

    }

    /// <summary>
    /// Describes changes/behaviour exerted on object. Remember to describe inverted and non inverted behaviors in child classes!!!
    /// </summary>
    /// <param name="obj"></param>
    public virtual void ApplyEffectOn(ObjectMaster obj) // This method is overriden in childclasses to do appropriate things.
    {
        // Do stuff here.
    }

}
