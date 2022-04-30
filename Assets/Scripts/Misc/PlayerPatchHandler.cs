using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patch.SocketUtility;

public class PlayerPatchHandler : MonoBehaviour
{
    protected Camera playerCam;

    public bool isDraggingPatch;
    protected PatchSocket startSocket;
    public float socketSelectionRange = 1f;
    public float maxSocketConnectionRange = 5f;

    public delegate void PlayerPatchHandlerEvent(string id);
    public static event PlayerPatchHandlerEvent PatchInRange;
    public static event PlayerPatchHandlerEvent PatchOutOfRange;


    public delegate void PatchConnectionEvent(int count);
    public static event PatchConnectionEvent CountUpdate;

    protected int currentPatchCount = 0;

    protected void Awake()
    {
        playerCam = Camera.main; // The main camera is also the player camera. Fix/change if this is not the case in the future.
    }

    protected void Start()
    {
        isDraggingPatch = false;
        currentPatchCount = 0;
    }

    protected void Update()
    {
        Vector3 rayDir = playerCam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, rayDir, out hit,socketSelectionRange))// Has the camera hit anything within range?
        {
            Debug.DrawLine(playerCam.transform.position, hit.point, Color.red);


            // Left-click to disconnect cable
            if (hit.collider.CompareTag("Cable") && Input.GetButtonDown("Fire1"))
            {
                PatchCable cable = hit.collider.GetComponent<PatchCable>();
                Debug.Log("TYPE: " +cable.GetCableType().ToString());
                if(cable.GetCableType() == PATCH_TYPE.STATIC)
                {
                    return;
                }
                else
                {
                    hit.collider.GetComponent<PatchCable>().Disconnect();
                    if (CountUpdate != null)
                        CountUpdate(++currentPatchCount);
                }

                return;
            }

            if(hit.collider.CompareTag("Cable") && PatchInRange != null)
                PatchInRange("Cable");

            if (hit.collider.CompareTag("Socket"))
            {
                if (PatchInRange != null)
                    PatchInRange("Socket");
                if(isDraggingPatch && Input.GetButton("Fire1"))
                    SocketUtilityHandler.DragConnection(hit.collider.transform.position);
            } // Check if socket is in range


            if (!hit.collider.CompareTag("Socket") && isDraggingPatch && Input.GetButton("Fire1"))
            {
                SocketUtilityHandler.DragConnection(hit.point /*- rayDir*0.2f*/);
            }

            if (!isDraggingPatch && hit.collider.CompareTag("Socket") &&  currentPatchCount > 0 && (Input.GetButtonDown("Fire1") || Input.GetButton("Fire1"))) // We hit a socket and start dragging
            {
                isDraggingPatch = true;
                Debug.Log("Began dragging connection from " + hit.collider.name);

                startSocket = hit.collider.GetComponent<PatchSocket>();
                if (startSocket != null)
                {
                    Vector3 boardP = SocketUtilityHandler.GetBoardPosition(startSocket.transform);
                    SocketUtilityHandler.CreateNewConnection(startSocket, hit.point);
                }
            }

            if (isDraggingPatch && (Input.GetButtonUp("Fire1"))) // We stopped pressing down the mouse button.
            {
                isDraggingPatch = false;
                Debug.Log("Stopped dragging");

                Vector3 dif = startSocket.transform.position - hit.point;
                if (hit.collider.CompareTag("Socket") && dif.sqrMagnitude < Mathf.Pow(maxSocketConnectionRange, 2))
                {
                    PatchSocket endSocket = hit.collider.GetComponent<PatchSocket>();
                    if (startSocket != null && endSocket != null && currentPatchCount > 0)
                    {
                        bool success = SocketUtilityHandler.ConnectPatches(startSocket, endSocket);
                        if (success && CountUpdate != null)
                            CountUpdate(--currentPatchCount);
                    }
                }
                else
                    SocketUtilityHandler.CleanFailedConnection();

            }
        }

        if(hit.collider == null)
            return;

        if(!hit.collider.CompareTag("Cable") && !hit.collider.CompareTag("Socket"))
            if(PatchOutOfRange != null)
                PatchOutOfRange("none");
            
    }
}
