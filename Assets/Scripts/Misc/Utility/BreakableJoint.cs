using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableJoint : MonoBehaviour
{
    [SerializeField]
    private float maxBreakVelocity = 5;
    public Transform pickedUpObj;
    public bool dropped = false;
    public float breakLimit;
    public float reach = 1;
    private Rigidbody rb;
    Vector3 handPos;
    private int originalLayer;

    // Update is called once per frame

    public void Pickup(Transform objTransform)
    {
        pickedUpObj = objTransform.transform;
        originalLayer = pickedUpObj.gameObject.layer;
        pickedUpObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        rb = pickedUpObj.GetComponent<Rigidbody>();
        pickedUpObj.gameObject.layer = 6;

        
    }
    void FixedUpdate()
    {
        handPos = transform.GetChild(0).transform.position + transform.GetChild(0).transform.forward * reach; //spot where the object is being held
        if (pickedUpObj != null)
        {
            dropped = false;
            Physics.IgnoreCollision(transform.GetComponent<Collider>(), pickedUpObj.GetComponent<Collider>(), true);
            float distance = (handPos - pickedUpObj.position).magnitude;

            if (distance > breakLimit)
            {
                dropped = true;
            }
            else
            {
                Vector3 old = pickedUpObj.rotation.eulerAngles; // set rotation for the object so it is neat
                old.x = 0;
                old.z = 0;
                old.y = transform.rotation.eulerAngles.y;
                Quaternion newRotation = Quaternion.identity;
                newRotation.eulerAngles = old;
                pickedUpObj.rotation = newRotation;
                Vector3 direction = (handPos - pickedUpObj.position); // apply force that keeps the object in hand
                if(pickedUpObj.GetComponent<ObjectMaster>().Heavy)
                {
                    direction = direction * 0.4f;
                    direction.y = direction.y * 0.3f;

                }
                rb.AddForce(direction * 200);
                if(rb.velocity.magnitude < 0.5f)
                {
                    rb.AddForce(-rb.velocity * 50);
                }
                else
                {
                    rb.AddForce(-rb.velocity * 20);
                }
            }
            if(dropped)
            {
                Drop();
            }

        }
    }
    public void Drop()
    {
        //resets object before unasigning it.
            Vector3 unitVelocity = rb.velocity.normalized;
            if(rb.velocity.magnitude > 5) //limits max speed when "throwing object"
            {
                rb.velocity=unitVelocity*maxBreakVelocity;
            }
            Physics.IgnoreCollision(transform.GetComponent<Collider>(), pickedUpObj.GetComponent<Collider>(), false);
            pickedUpObj.gameObject.layer = originalLayer;
            pickedUpObj = null;
    }
    
}
