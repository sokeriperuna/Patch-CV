using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    ScriptThatHandlesInputs inputProvider;
    
    [SerializeField] private float mouseSensitivity = 5;

    Vector2 mouseInputs;

    GameObject PlayerCamera;

    float mouseY = 0;
    Vector3 mouseYeuler;
    
    void Start()
    {
        if(inputProvider == null)
        {
            inputProvider = GameObject.FindGameObjectsWithTag("Input")[0].GetComponent<ScriptThatHandlesInputs>();
        }

    }

    void Update()
    {
        mouseInputs = inputProvider.getPlayerInputs.GetInputs().mouseInput  * mouseSensitivity;

        mouseY -= mouseInputs.y;
        mouseY = Mathf.Clamp(mouseY, -90, 90);
        transform.GetChild(0).transform.localRotation = Quaternion.Euler(mouseY, 0f, 0f);
        transform.Rotate(Vector3.up * mouseInputs.x);
    }
}
