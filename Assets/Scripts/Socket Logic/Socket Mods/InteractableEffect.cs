using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEffect : EffectMaster
{
    public override string GetNormalName()
    {
        return "Pickupable";
    }
    public override string GetInvertedName()
    {
        return "Not Pickupable";

    }
    public override void ApplyEffectOn(ObjectMaster obj)
    {
        base.ApplyEffectOn(obj);
        obj.Pickupable = !Inverted;
    }

}
