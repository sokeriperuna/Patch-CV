using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Helps with defining and regulating collision based behaviour in Objects
public class CollisionUtility : MonoBehaviour
{
    // Start is called before the first frame update
    ObjectMaster objectMaster;
    void Awake()
    {
        objectMaster = GetComponent<ObjectMaster>();
    }

    void OnCollisionEnter(Collision col)
    {
        if(objectMaster == null)
        {
            return; // Object master is missing so we just return
        }


        if(objectMaster.Bouncy) // Let's bounce
        {
            Debug.Log(col.collider.gameObject.name);

            objectMaster.RB.AddForce(col.contacts[0].normal * 10, ForceMode.VelocityChange);

            Debug.Log(col.collider.gameObject.name);
            if(col.collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                col.collider.gameObject.GetComponent<Rigidbody>().AddForce(-col.contacts[0].normal * 10, ForceMode.VelocityChange);
                Debug.Log(col.contacts[0].normal);
            }

        }

        if(objectMaster.IsAlive)
        {
            if(col.collider.gameObject.GetComponent<ObjectMaster>() != null)
            {
                if(objectMaster.Mortal && col.collider.gameObject.GetComponent<ObjectMaster>().Lethal)
                {
                    objectMaster.IsAlive = false;
                    Debug.Log(objectMaster.gameObject.name + " " + "is dead");
                    if(col.collider.gameObject.tag != "Player")
                    {
                        if(objectMaster is PlayerObject)
                            (objectMaster as PlayerObject).Die();
                        else
                            objectMaster.gameObject.SetActive(false);  
                    }
                }
            }
        }
    }
}
