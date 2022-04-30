using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyEffect : EffectMaster
{
    public override string GetNormalName()
    {
        return "Bouncy";
    }
    public override string GetInvertedName()
    {
        return "Not Bouncy";
    }

    public override void ApplyEffectOn(ObjectMaster obj)
    {
        base.ApplyEffectOn(obj);
        if (Inverted) // quick test of logic and inversion
            return;
        else
        {
            obj.Bouncy = true;
        }
    }

    public override void OnConnect(PatchModifier mod)
    {
        base.OnConnect(mod);
        if(mod is ObjectMaster)
        {
            if(Application.isPlaying)
            {
                Debug.Log("Hej");
            ObjectMaster obj = mod as ObjectMaster;
            Debug.Log(obj.gameObject.name);
            Debug.Log(obj.Bouncy);

            obj.RB.AddForce(Vector3.up*10, ForceMode.VelocityChange);
            }

        }
    }
    public override void OnDisconnect(PatchModifier mod)
    {
        base.OnDisconnect(mod);
        if(mod is ObjectMaster)
        {
            ObjectMaster obj = mod as ObjectMaster;
            obj.Bouncy = false;
        }
    }
}
