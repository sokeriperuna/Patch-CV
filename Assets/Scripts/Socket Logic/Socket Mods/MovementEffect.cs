using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//Some hardcoded air speed values at 112 and 114 or if moved can be found from calculate movement and else from if(if grounded)
public class MovementEffect : EffectMaster
{
    public override string GetNormalName()
    {
        return "Movement";
    }
    public override string GetInvertedName()
    {
        return "tnemevoM";

    }

    //Sorry for adding dependencies
    private ScriptThatHandlesInputs inputProvider;
    //movespeed doesn't represent actual speed. Actual horizontal speed is based on movement forces and opposing forces to them.
    [SerializeField] private float moveSpeed = 45; 
    [SerializeField] private float jumpForce = 5; 
    private Vector2 inputData;
    private Vector3 objPos = Vector3.zero;
    [SerializeField] private bool _jump; 
    private bool _interact;
    private bool _grounded = false;
    private Vector3 newVelocity;

    protected override void Start()
    {
        InitializeInputs();
        base.Start();        
    }

    public override void ApplyEffectOn(ObjectMaster obj)
    {
        base.ApplyEffectOn(obj); // Prints basic debug message
        inputData = inputProvider.getPlayerInputs.GetInputs().directonalInput;

        if (Inverted) // quick test of logic and inversion
        {
            //input data inverted
            inputData = -inputData.normalized;
        }
        else
        {
            inputData = inputData.normalized;
        }

        objPos = obj.transform.position;
        CheckIfGrounded(obj);

        if (obj.tag == "Player")
        {
            if(_interact)
            {
                if(obj.gameObject.GetComponent<BreakableJoint>().pickedUpObj == null)
                {
                    TryPicupObject(obj.gameObject);
                }
                else
                {
                    obj.gameObject.GetComponent<BreakableJoint>().Drop();
                }
                _interact = false;
            }
        }

        Vector3 dir = obj.transform.forward * inputData.y + obj.transform.right * inputData.x;
        CalculateMovement(dir, obj);
        
    }

    void TryPicupObject(GameObject playerObject)
    {
        Transform cameraTransform = playerObject.transform.GetChild(0).transform;
        //raycast from camera to check if object in range
        RaycastHit hit;
        Physics.Raycast(cameraTransform.position, cameraTransform.forward,out hit, playerObject.GetComponent<BreakableJoint>().reach); 
        //reach is in breakable joint and it's the max distance where object can be picked up form
        if(hit.transform != null && hit.collider.gameObject.transform.GetComponent<ObjectMaster>() != null)
        {
            if(hit.collider.gameObject.transform.GetComponent<ObjectMaster>().Pickupable)
            {
                playerObject.GetComponent<BreakableJoint>().Pickup(hit.collider.gameObject.transform);
            }
        }
        

    }
    void CalculateMovement(Vector3 directionInput, ObjectMaster obj)
    {
        // Calculates a new velocity for player based on current velocity, ground angle and player inputs.
        Vector3 newVelocity;
        Vector3 adjustedDirection = RaycastGroundAngle(directionInput);
        float multiplier = 0;
        if(!float.IsNaN((-obj.RB.velocity*(Mathf.Log10(obj.RB.velocity.magnitude)+10)).magnitude))
        {
            multiplier = (Mathf.Log10(obj.RB.velocity.magnitude)+10);
        }
        if (_grounded)
        {

            //stopping when no input applied
            if (adjustedDirection.magnitude == 0)
            {
                {
                    obj.RB.AddForce(-obj.RB.velocity*multiplier);
                }
            }
            else
            {
                obj.RB.AddForce(adjustedDirection*moveSpeed);
                Vector3 horizontalVelocity = new Vector3(obj.RB.velocity.x, 0, obj.RB.velocity.z);
                obj.RB.AddForce(-horizontalVelocity * multiplier);  
            }
        }
        else
        {
            //air movement
            obj.RB.AddForce(adjustedDirection*moveSpeed*0.2f);
            Vector3 horizontalVelocity = new Vector3(obj.RB.velocity.x, 0, obj.RB.velocity.z);
            if(horizontalVelocity.magnitude > 4.5f)
            {
                obj.RB.AddForce(-horizontalVelocity * multiplier);  
            }
        }
        if(_jump == true)
        {
            //jumping
            if(obj.RB.velocity.y < 0)
            {
                newVelocity = new Vector3(obj.RB.velocity.x, 0, obj.RB.velocity.z);
                obj.RB.velocity = newVelocity;
            }
            obj.RB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jump = false;
        }
    }

    Vector3 RaycastGroundAngle(Vector3 directionInput)
    {
        // Outputs angle of the ground according to directionInput vector.
        float raycastLength = 2;
        RaycastHit[] hits = TripleRaycast(raycastLength,directionInput);

        List<RaycastHit> confirmedHits = new List<RaycastHit>();
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                confirmedHits.Add(hit);
            }
        }

        Vector3 outputDirection = directionInput;
        // The outputDirection is adjusted according to ground plane angle.
        // We return the input direction if we're on flat ground.
        switch (confirmedHits.Count)
        {
            default:

            case 1:
                break;

            case 2:
                outputDirection = (confirmedHits[1].point - confirmedHits[0].point).normalized;

                Debug.DrawLine(confirmedHits[1].point, confirmedHits[0].point);
                break;

            case 3:
                Vector3 backToCenter = confirmedHits[1].point - confirmedHits[0].point;
                Vector3 centerToFront = confirmedHits[2].point - confirmedHits[1].point;
                outputDirection = ((backToCenter + centerToFront) * 0.5f).normalized;

                Debug.DrawLine(confirmedHits[0].point, confirmedHits[1].point);
                Debug.DrawLine(confirmedHits[1].point, confirmedHits[2].point);
                break;
        }
        Debug.DrawLine(objPos, objPos + outputDirection * 3, Color.yellow);
        return outputDirection;

    }
    RaycastHit[] TripleRaycast(float raycastLength, Vector3 directionInput)
    {
        int layerMask = 1 << 6;
        layerMask = ~layerMask;

        // Raycasts in the back of the player model, in the middle of the player model and in front of the player model.
        RaycastHit[] hits = new RaycastHit[3]; // 0 = back, 1 = center, 2 = front

        //Use 'Bounds' to make universal. Cal
        if (Physics.Raycast(objPos + (directionInput).normalized / 2, -transform.up, out hits[2], raycastLength,layerMask))
        {
            Debug.DrawRay(objPos + (directionInput).normalized / 2, Vector3.down * hits[2].distance, Color.green);
        }

        if (Physics.Raycast(objPos, -transform.up, out hits[1], raycastLength,layerMask))
        {
            Debug.DrawRay(objPos, Vector3.down * hits[1].distance, Color.red);
        }

        if (Physics.Raycast(objPos - (directionInput).normalized / 2, -transform.up, out hits[0], raycastLength,layerMask))
        {
            Debug.DrawRay(objPos - (directionInput).normalized / 2, Vector3.down * hits[0].distance, Color.blue);
        }
        return hits;
    }

    void CheckIfGrounded(ObjectMaster obj)
    {
        
        // Raycasts to check if the player is grounded
        
        float raycastJumpLength = obj.GetComponent<Collider>().bounds.size.y * 0.525f;
        RaycastHit[] hits = TripleRaycast(raycastJumpLength, Vector3.zero);

        List<RaycastHit> confirmedHits = new List<RaycastHit>();
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                confirmedHits.Add(hit);
            }
        }

        if (confirmedHits.Count > 0)
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }
    }
    
    void InitializeInputs()
    {
                /*#if UNITY_EDITOR
        try
        {
            inputProvider = GameObject.FindGameObjectsWithTag("Input")[0].GetComponent<ScriptThatHandlesInputs>();
        }
        catch
        {
            Debug.LogError("InputHandler object not found. Add input provider to scene from prefabs");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        #endif*/
        if(inputProvider == null)
        {
            inputProvider = GameObject.FindGameObjectsWithTag("Input")[0].GetComponent<ScriptThatHandlesInputs>();
        }
        inputProvider.getPlayerInputs.jumpEvent += Jump;
        inputProvider.getPlayerInputs.interaction += Interact;
    }

    void Jump()
    {
        if(_grounded)
        {
            _jump = true;
        }
    }
    void Interact()
    {
        _interact = true;
    }
}
