using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// ObjectMaster is the master class all derivative objects inherit from.
/// The master object has some basic traits that all objects share which can be overriden by child instances of the class.
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CollisionUtility))]

[ExecuteInEditMode]
public class ObjectMaster : PatchModifier
{
    public override string GetNormalName()
    {
        return "";
    }
    public override string GetInvertedName()
    {
        return "";

    }

    protected bool pickupable; // Gives the player ability to pickup the object.
    protected bool bouncy = false;
    protected bool isAlive;
    protected bool lethal;
    protected bool mortal;
    protected bool heavy;
    protected bool gravity;

    [SerializeField]protected Rigidbody rb;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        RB.useGravity = false;
    }

    protected virtual void Start()
    {
        isInverted = false;
        pickupable = false;
        bouncy = false;
        isAlive = true;
        lethal = false;
        mortal = false;
    }



    public Rigidbody RB    { get { return rb; }         set { rb = value;         } } // accessor does pretty much nothing but tidy up look and make code slightly easier to read. 
    public bool Pickupable { get { return pickupable; } set { pickupable = value; } }
    public bool Bouncy     { get { return bouncy; }     set { bouncy = value;     } }
    public bool IsAlive    { get { return isAlive; }    set { isAlive = value;    } }
    public bool Lethal     { get { return lethal; }     set { lethal = value;     } }
    public bool Mortal     { get { return mortal; }     set { mortal = value;     } }
    public bool Heavy      { get { return heavy; }      set { heavy = value;      } }
    public bool  Gravity   { get { return gravity; }    set { gravity = value;    } }

}

