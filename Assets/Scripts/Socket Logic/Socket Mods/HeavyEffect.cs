using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyEffect : EffectMaster
{
    public override string GetNormalName()
    {
        return "Heavy";
    }
    public override string GetInvertedName()
    {
        return "Light";

    }

    public override void ApplyEffectOn(ObjectMaster obj)
    {
        base.ApplyEffectOn(obj);
        if (Inverted) // quick test of logic and inversion
            obj.RB.mass = 0.1f;
        else
        {
            obj.Heavy = true;
            obj.RB.mass = 10;
        }
    }


    public override void OnDisconnect(PatchModifier mod)
    {
        base.OnDisconnect(mod);
        if(mod is ObjectMaster)
        {
            ObjectMaster obj = mod as ObjectMaster;
            if(obj == null)
            Debug.Log("nbull obj");
                                    if(obj.RB == null)
            Debug.Log("RB null");
            obj.Heavy = false;
        }
    }
}
