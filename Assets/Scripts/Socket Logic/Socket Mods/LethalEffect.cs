using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LethalEffect : EffectMaster
{
    public override string GetNormalName()
    {
        return "Lethal";
    }
    public override string GetInvertedName()
    {
        return "Non-Lethal";

    }
    public override void ApplyEffectOn(ObjectMaster obj)
    {
        base.ApplyEffectOn(obj);
        if (Inverted) // quick test of logic and inversion
            return;
        else
        {
            obj.Lethal = true;
        }
    }


    public override void OnDisconnect(PatchModifier mod)
    {
        base.OnDisconnect(mod);
        if(mod is ObjectMaster)
        {
            ObjectMaster obj = mod as ObjectMaster;
            obj.Lethal = false;
        }
    }
}
