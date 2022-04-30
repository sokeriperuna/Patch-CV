using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class DoorObject : ObjectMaster
{

    public float openHeight = 2.5f;

    private Vector3 currentPos;

    public override string GetNormalName()
    {
        return "Door (closed)";
    }
    public override string GetInvertedName()
    {
        return "Door (open)";

    }

    protected override void Awake() 
    {
        //
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        currentPos = transform.position;
    }
    void Update()
    {
        #if UNITY_EDITOR 
        if (Application.isPlaying)
        #endif
            if(Inverted)
            {
                transform.position = currentPos+Vector3.up*openHeight;
            }
            else
            {
                transform.position = currentPos;
            }
    }



}
