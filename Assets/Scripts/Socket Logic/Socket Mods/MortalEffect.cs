using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortalEffect : EffectMaster
{
    public override string GetNormalName()
    {
        return "Mortal";
    }
    public override string GetInvertedName()
    {
        return "Immortal";

    }
    public override void ApplyEffectOn(ObjectMaster obj)
    {
        base.ApplyEffectOn(obj);
        if (Inverted) // quick test of logic and inversion
        {
            obj.Mortal = false;
        }
        else
        {
            obj.Mortal = true;
        }
    }


    public override void OnDisconnect(PatchModifier mod)
    {
        base.OnDisconnect(mod);
        if(mod is ObjectMaster)
        {
            ObjectMaster obj = mod as ObjectMaster;
            obj.Mortal = false;
        }
    }
}
