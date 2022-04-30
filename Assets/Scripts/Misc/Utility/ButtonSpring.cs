using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSpring : MonoBehaviour
{
    public Rigidbody pressurePlate;
    public Rigidbody buttonBase;

    public float springConstant = 1000f;
    public float dampening = 10f;
    public float buttonHeight = 0.2f;

    public bool pressed = false;

    // Start is called before the first frame update
     Vector3 pressurePlatePos;
     Vector3 upPosition;
     
    void Start()
    {
        pressurePlatePos = new Vector3(buttonBase.transform.position.x-0.5f, buttonBase.transform.position.y, buttonBase.transform.position.z+0.5f);
        upPosition = pressurePlatePos + buttonBase.transform.up * buttonHeight;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //defining the position where the pressureplate needs to be and the offset from the position.
        pressurePlatePos = new Vector3(buttonBase.transform.position.x, buttonBase.transform.position.y, buttonBase.transform.position.z);
        upPosition = pressurePlatePos + buttonBase.transform.up * buttonHeight;
        Vector3 d = pressurePlate.transform.position-upPosition;
        
        //Spring force
        if(buttonHeight - d.magnitude <0.1f)
        {
            pressurePlate.position = buttonBase.position + buttonBase.transform.up*0.1f;
        }
        if((pressurePlate.position-buttonBase.position).magnitude > buttonHeight)
        {
            pressurePlate.position = upPosition;
            pressurePlate.velocity = Vector3.zero;
        }
        pressurePlate.AddForce(-springConstant * d);
        
        pressurePlate.transform.rotation = buttonBase.transform.rotation;

        //friction force that opposes the spring force based on velocity
        pressurePlate.AddForce(-pressurePlate.velocity*dampening);   

        
        //check if the pressureplate is presed enough
        if(buttonHeight - d.magnitude < 0.15f)
        {
            pressed = true;
        } 
        else
        {
            pressed = false;
        }
        
    }

}
