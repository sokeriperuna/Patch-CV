using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Patch.SocketUtility;

[ExecuteInEditMode]
[RequireComponent(typeof(PatchSocket))]
public class SocketLabelController : MonoBehaviour
{
    private PatchSocket socket;
    private Bounds socketBounds;

    public GameObject textObject;
    public TextMeshPro TMProText;



    private const float textBoundsToWorld = 0.001f;
    private const float lowerTextSpace = 0.05f;//0.15f;

    Vector3 labelOffesetPosition;

    public void Awake()
    {
        /*
        if (textObject != null)
            UpdateLabelPosition();*/
    }
    public void Start()
    {
        if (socket == null || textObject == null || TMProText == null)
            InitializeLabel();

        UpdateLabelAll();
    }

    void InitializeLabel()
    {
        socket = GetComponent<PatchSocket>(); // Make sure we have a reference to this
        socketBounds = socket.GetComponent<Collider>().bounds;

        GameObject textObjectPrefab = Resources.Load<GameObject>("Prefabs/Socket Label Text");

        foreach (Transform child in this.transform)
        {
            if (TryGetComponent<TextMeshPro>(out TMProText))
                textObject = child.gameObject;
        }

        if(textObject == null)
        {
            textObject = Instantiate(textObjectPrefab, socket.transform); // Parent 
        }

        TMProText = textObject.GetComponent<TextMeshPro>();

        SocketUtilityHandler.UnpackPrefab((UnityEngine.Object)textObject);
    }

    public void UpdateLabelAll()
    {
        UpdateLabelText();
        UpdateLabelPosition();
    }
    void UpdateLabelPosition()
    {
        Vector3 bef = textObject.transform.position;
        textObject.transform.rotation = this.transform.rotation; // Make sure we are aligned
        
        Vector3 offset = new Vector3(0f, TMProText.GetPreferredValues().y*textBoundsToWorld*0.5f + lowerTextSpace + socketBounds.size.y*0.5f, 0f);
        textObject.transform.position = this.transform.position+offset;

    }

    void UpdateLabelText()
    {
        string newText = "Empty";
        if (socket.modifiers[0] != null)
        {
            PatchModifier mod = socket.modifiers[0];
            newText = mod.Inverted ? mod.GetInvertedName() : mod.GetNormalName();
        }

        TMProText.text = newText;
    }
}
