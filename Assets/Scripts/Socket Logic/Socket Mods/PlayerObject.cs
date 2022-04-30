using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[ExecuteInEditMode]
public class PlayerObject : ObjectMaster
{
    public override string GetNormalName()
    {
        return "Player";
    }
    public override string GetInvertedName()
    {
        return "Player";

    }

    ObjectMaster playerObjectMaster;
    protected override void Awake()
    {
        playerObjectMaster = GetComponent<ObjectMaster>();
        base.Awake();
    }

    public static event Action OnPlayerDeath;
    
    public void Die()
    {
        playerObjectMaster.IsAlive = false;
            if(OnPlayerDeath != null)
                OnPlayerDeath();
    }

}
