using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeEffect : EffectMaster
{
    public override string GetNormalName()
    {
        return "Freeze";
    }
    public override string GetInvertedName()
    {
        return "Unfreeze";

    }
    public override void ApplyEffectOn(ObjectMaster obj)
    {

        base.ApplyEffectOn(obj);
        if (Inverted) // quick test of logic and inversion
            obj.transform.GetComponent<Rigidbody>().isKinematic = false;
        else
        {
            obj.transform.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public override void OnDisconnect(PatchModifier mod)
    {
        if(mod is ObjectMaster)
        {
            ObjectMaster obj = mod as ObjectMaster;
            obj.transform.GetComponent<Rigidbody>().isKinematic = false;
        }
        
    }
}
