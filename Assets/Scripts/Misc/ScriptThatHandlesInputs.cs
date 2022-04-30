using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
Check MovementEffect on how to implement inputs.

To use interface:
1. add action even to interface eg. jump
2. add an function that triggers the event
3. have the input being checked in GetKeyPresses

Use previous code as a reference.
*/
public class ScriptThatHandlesInputs : MonoBehaviour
{
    // Start is called before the first frame update

    public GetPlayerInputs getPlayerInputs = new GetPlayerInputs();

    private InputState inputState;
    // Start is called before the first frame update
    void Start()
    {
        inputState = new InputState();
        //DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        ContinousInputs();
        getPlayerInputs.assignInputs(inputState);
        GetKeyPresses();
    }

    private void ContinousInputs()
    {
        //add continous variables here
        inputState.directonalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputState.mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));  
    }

    private void GetKeyPresses()
    {
        //add checks for key presses here
        if(Input.GetKeyDown(KeyCode.Space))
        {
            getPlayerInputs.Jump();
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            getPlayerInputs.Interact();
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            getPlayerInputs.LockUnlock();
        }
    }
}


//______________________________________________
public struct InputState
{
    //continious input variables
    public Vector2 directonalInput;
    public Vector2 mouseInput;
}
//____________________________________________________
public interface IInputProvider
{
    public InputState GetInputs();
    public event Action jumpEvent;
    public event Action interaction;
    public event Action escapeEvent;

}

public class GetPlayerInputs: IInputProvider
{
    InputState inputs;
    public event Action jumpEvent;
    public event Action interaction;
    public event Action escapeEvent;


    public void Jump()
    {
        if(jumpEvent != null)
        {
            jumpEvent();
        }
    }
    public void Interact()
    {
        if(interaction != null)
        {
            interaction();
        }
    }
    public void LockUnlock()
    {
        if(escapeEvent != null)
        {
            escapeEvent();
        }
    }
    
    public void assignInputs(InputState ins)
    {
        inputs = ins;
    }
    public InputState GetInputs()
    {       
        return inputs;
    }
}
