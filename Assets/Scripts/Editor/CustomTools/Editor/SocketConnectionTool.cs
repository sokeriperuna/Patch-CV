using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.EditorTools;
using Patch.SocketUtility;

[EditorTool("Socket Connection Tool")]
public class SocketConnectionTool : EditorTool
{
    // Serializing this to make it editable in the inspector
    [SerializeField]
    Texture2D toolIcon;

    GUIContent iconContent;

    bool raycastSuccess;
    RaycastHit hit;

    PatchSocket currentSocket;

    void OnEnable()
    {
        iconContent = new GUIContent()
        {
            image = toolIcon,
            text = "Patch Tool",
            tooltip = "Patch Tool"
        };
    }

    public override GUIContent toolbarIcon
    {
        get { return iconContent;  }
    }

    // Called for each window that the tool is active in. Functionality goes here.
    public override void OnToolGUI(EditorWindow window)
    {

        // Make sure the tool plays nice with other GUI
        int controlID = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);

        Camera sceneCam = SceneView.lastActiveSceneView.camera; // We get the scene most recently in focus

        if (sceneCam != null)
        {
            if (window.hasFocus)
            {
                Vector2 GUI_mPos = Event.current.mousePosition; // This is the rect position. Top-left is (0,0), bottom-right is (pixelWidth, pixelHeight).

                Ray camRay = HandleUtility.GUIPointToWorldRay(GUI_mPos);

                RaycastHit[] hits = Physics.RaycastAll(camRay);

                raycastSuccess = Physics.Raycast(camRay, out hit);

                GameObject[] sockets = GameObject.FindGameObjectsWithTag("Socket");

                // Draw highlighting visuals for all of the sockets
                Handles.color = Color.yellow;
                foreach (GameObject s in sockets)
                    Handles.DrawWireCube(s.transform.position, s.transform.lossyScale); 

                Event e = Event.current;
                if(e.isMouse)
                    switch(e.type)
                    {
                        case EventType.MouseDown:
                            ProcessMouseDown(e.button);
                            break;


                        case EventType.MouseDrag:
                            ProcessDrag(e.button);
                            //Debug.Log("Mouse drag");
                            break;

                        case EventType.MouseUp:
                            //Debug.Log("Mouse was released");
                            ProcessMouseUp(e.button);
                            break;

                        default:
                            break;
                    }

                window.Repaint(); // Draw everything
            }
        }
    }

    void ProcessMouseDown(int buttonIndex)
    {
        if (!raycastSuccess)
            return;
        else
        {
            // Begin dragging.
            switch (buttonIndex)
            {
                case 0: // Left-click
                    //Debug.Log("Left mouse button");
                    switch (hit.collider.tag)
                    {
                        case "Socket":
                            currentSocket = hit.collider.GetComponent<PatchSocket>();
                            SocketUtilityHandler.CreateNewConnection(currentSocket, hit.point); // Begin dragging new connection

                            break;

                        case "Cable":
                            hit.collider.GetComponent<PatchCable>().Disconnect();

                            break;
                        default:
                            break;
                    }
                    break;
                case 1: // Right-click
                    switch(hit.collider.tag)
                    {
                        case "Cable":
                            PatchCable cable = hit.collider.GetComponent<PatchCable>();
                            SocketUtilityHandler.UnpackPrefab(cable.gameObject); // Make sure changes are saved

                            if(cable.GetCableType() == PATCH_TYPE.NORMAL)
                                cable.SetCableType(PATCH_TYPE.STATIC);
                            else
                                cable.SetCableType(PATCH_TYPE.NORMAL);

                        break;

                        default:
                        break;
                    }
                    break;
                case 2: // Middle mouse button
                    break;
                default: // Unknown mouse input
                    break;
            }
        }
        
    }

    void ProcessDrag(int buttonIndex)
    {
        if (!raycastSuccess)
            return;
        else
        {
            // What button is being pressed while we drag?
            switch (buttonIndex)
            {
                case 0: // Left-click
                    if(hit.collider.CompareTag("Socket"))
                    
                    {
                        //Vector3 p = SocketUtilityHandler.GetBoardPosition(hit.collider.transform);
                        SocketUtilityHandler.DragConnection(hit.collider.transform.position);
                    }
                    else
                        SocketUtilityHandler.DragConnection(hit.point);
                    break; 
                case 1: // Right-click
                    break;
                case 2: // Middle mouse button
                    break;
                default: // Unknwon mouse input
                    break;
            }
        }
    }

    void ProcessMouseUp(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0: // Left-click
                if (raycastSuccess)
                {
                    if (hit.collider.CompareTag("Socket"))
                    {
                        
                        SocketUtilityHandler.ConnectPatches(currentSocket, hit.collider.GetComponent<PatchSocket>());
                        break; // Exit when we make the connection
                    }
                }

                //Debug.Log("Failure");
                SocketUtilityHandler.CleanFailedConnection();


                break;
            case 1: // Right-click
                break;
            case 2: // Middle mouse button
                break;
            default: // Unknwon mouse input
                break;
        }
    }
}
