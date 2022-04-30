using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]

[RequireComponent(typeof(SocketLabelController))]
public class PatchSocket : MonoBehaviour
{
    
    /// Socket behaviour is a group of public variables which input scripts that define what a socket does and how it does it.
    /// Eg. button passes on logic input. Movement effect moves object along its local axis based on keyboard input. An object component defines an object that is affeted by other sockets connecting to this socket.

    public PatchModifier[] modifiers;

    public PatchSocket[] children;

    [HideInInspector]
    public SocketLabelController labelController;

    bool[] wasInverted;

    protected virtual void Awake()
    {
        if(labelController == null) // stupid simple workaround to make sure component has a labelcontroller
            if(GetComponent<SocketLabelController>() != null)
                labelController = GetComponent<SocketLabelController>();

        wasInverted = new bool[modifiers.Length];

        for(int i=0; i<modifiers.Length; i++)
            wasInverted[i]= modifiers[i].Inverted;
    }

    protected virtual void FixedUpdate() // Leaving room for inheritance if needed
    {

        foreach (PatchSocket child in children)
        {
            foreach(PatchModifier parentMod in modifiers)
            {
                foreach (PatchModifier childMod in child.modifiers)
                {
                    if (parentMod is EffectMaster && childMod is ObjectMaster)
                       (parentMod as EffectMaster).ApplyEffectOn(childMod as ObjectMaster);


                    if (parentMod is LogicMaster)
                    {
                        LogicMaster logicParent = parentMod as LogicMaster;

                        if (childMod is EffectMaster)
                           (childMod as EffectMaster).Inverted = logicParent.Output;

                        if (childMod is ObjectMaster)
                           (childMod as ObjectMaster).Inverted = logicParent.Output;
                
                    }
                }

            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < modifiers.Length; i++)
            if(wasInverted[i] != modifiers[i].Inverted)
            {
                wasInverted[i] = !wasInverted[i];
                SignalStateChange();
            }

    }

    public void AddSocket(PatchSocket newChild)
    {
        // Extend the current array to acommodate new connection;
        PatchSocket[] temp = new PatchSocket[this.children.Length + 1];

        this.children.CopyTo(temp, 0); // Copy existing elements to the temp list
        temp[this.children.Length] = newChild; // Add child to the end of the list
        this.children = temp;

        foreach (PatchModifier mod in modifiers)
            foreach (PatchModifier childMod in newChild.modifiers)
            {
                mod.OnConnect(childMod);
            }

        Debug.Log("Connected " + this.ToString() + " to " + newChild.ToString() + '.');
        SignalStateChange();
    }

    public void RemoveSocket(PatchSocket oldChild)
    {
        int i = 0;
        bool foundChild = false;
        while (i < this.children.Length)
        {
            if (this.children[i] == oldChild)
            {
                foundChild = true;
                break;
            }
            ++i;
        }

        if (foundChild)
        {
            // Shorten array to remove old child
            PatchSocket[] temp = new PatchSocket[this.children.Length - 1];
            for (int j = 0; j < i; j++)
                temp[j] = this.children[j];

            for (int k = i + 1; k < this.children.Length; k++)
                temp[k - 1] = this.children[k];

            foreach (PatchModifier mod in modifiers)
                foreach (PatchModifier childMod in oldChild.modifiers)
                {
                    mod.OnDisconnect(childMod);
                }
                    

            this.children = temp;
            Debug.Log(oldChild.ToString() + " + removed from children.");

            SignalStateChange();
        }
        else
            Debug.LogError("Failure to remove child: child does not exist.");

    }        

    public void SignalStateChange()
    {
        //Debug.Log("Signalled state change.");
        if(labelController != null)
            labelController.UpdateLabelAll();
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }*/
}
