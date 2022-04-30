using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityEffect : EffectMaster
{
    public override string GetNormalName()
    {
        return "Gravity";
    }
    public override string GetInvertedName()
    {
        return "Anti-Gravity";
    }

    [SerializeField] float gravityStrength = 9.81f;
    private Vector3 gravityVector;
    public override void ApplyEffectOn(ObjectMaster obj)
    {
        base.ApplyEffectOn(obj);
        if (Inverted) // quick test of logic and inversion
            gravityVector = Vector3.up * gravityStrength;
        else
        {
            gravityVector = Vector3.down * gravityStrength;
        }
        obj.RB.AddForce(gravityVector,ForceMode.Acceleration);
    }


    public override void OnDisconnect(PatchModifier mod)
    {
        base.OnDisconnect(mod);
        if(mod is ObjectMaster)
        {
            ObjectMaster obj = mod as ObjectMaster;
            obj.Gravity = false;
        }
    }
    public override void  OnConnect(PatchModifier mod)
    {
        base.OnConnect(mod);
        if(mod is ObjectMaster)
        {
            ObjectMaster obj = mod as ObjectMaster;
            obj.Gravity = true;
        }
    }
}
