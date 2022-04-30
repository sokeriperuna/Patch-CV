using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Patch.SocketUtility
{
    public static class SocketUtilityHandler
    {
        static GameObject cablePrefab = Resources.Load<GameObject>("Prefabs/Patch Cable"); // Just load the asset straight into the static class because why not?

        private static PatchSocket currentStartSocket;
        private static PatchCable currentCable;

        public static Vector3 GetBoardPosition(Transform parentT)
        {
            Vector3 boardPosition = parentT.position + parentT.forward * parentT.GetComponent<Collider>().bounds.size.z * 0.5f;
            return boardPosition;

        }

        public static void CreateNewConnection(PatchSocket startingSocket, Vector3 selectionPosition) // Every new connection should be started by this function
        {
            currentStartSocket  = startingSocket;

            // Instantiate patch cable. Starting cable socket is the parent.
            GameObject cableObject = GameObject.Instantiate(cablePrefab, startingSocket.transform.position, Quaternion.identity, startingSocket.transform);
            currentCable = cableObject.GetComponent<PatchCable>();

            Transform startSocketT = currentStartSocket.transform;
            Collider socketColl = startSocketT.GetComponent<Collider>();

            Vector3 boardPosition = GetBoardPosition(startSocketT);
            currentCable.ConfigurePatchCable(boardPosition, selectionPosition);
        }

        public static void DragConnection(Vector3 newSelectionPosition) // Should be called through every frame of dragging
        {
            //Debug.Log("Drag connection called");
            if(currentCable != null)
            {
                Vector3 boardPos = GetBoardPosition(currentStartSocket.transform);
                currentCable.ConfigurePatchCable(boardPos, newSelectionPosition);
            }
                
        }

        /// <summary>
        /// Submit patches in order of dragging.
        /// </summary>
        /// <param name="socket1"></param>
        /// <param name="socket2"></param>
        /// <returns></returns>
        public static bool ConnectPatches(PatchSocket socket1, PatchSocket socket2) // Order does not matter as it gets resolved. Socket 1 wins ties though. Bool tells if connection was succesful or not.
        {
            if(socket1 == null || socket2 == null) // Check for errors just to be safe before we commit to running this long function
            {
                Debug.LogError("Trying to connect sockets with a null reference!");
                return false;
            }

            if (socket1 == socket2)
            {
                Debug.Log("Cannot connect " + socket1.ToString() + "to self.");
                CleanFailedConnection();
                return false;
            }

            if (CheckForExistingConnection(socket1, socket2) || CheckForExistingConnection(socket2, socket1))
            {
                CleanFailedConnection();
                Debug.Log("Cancelled duplicate between " + socket1.ToString() + " and " + socket2.ToString() + '.');
                return false;
            }

            Debug.Log("S1: " + socket1.name + " | S2:" + socket2.name);

            /// |      [SOCKET CONNECTIVITY RULES]          |
            /// | Logic can connect to Effects and Objects. |
            /// | Modifiers can connect to Objects.         |
            /// | Objects alone cannot connect to anything. |
              
            // We resolve the order and priority of the patches

            // x = object, y = effect, z = logic
            Vector3Int modCount1 = GetSocketModCount(socket1); 
            Vector3Int modCount2 = GetSocketModCount(socket2);


            /// Check for all illegal connections this string of if statements is a bit bad but it works...

            // A connection between two pure object sockets without any other sockets is illegal.
            if (modCount1.x > 0 && modCount2.x > 0 && (modCount1.y + modCount2.y + modCount1.z + modCount2.z) == 0) 
            {
                LogIllegalConnection(modCount1, modCount2);
                CleanFailedConnection();
                return false;
            }

            // A connection between two pure modifier sockets without any other sockets is illegal.
            if (modCount1.y > 0 && modCount2.y > 0 && (modCount1.x + modCount2.x + modCount1.z + modCount2.z) == 0) 
            {
                LogIllegalConnection(modCount1, modCount2);
                CleanFailedConnection();
                return false;
            }

            // A connection between two pure logic sockets without any other sockets is illegal.
            if (modCount1.z > 0 && modCount2.z > 0 && (modCount1.x + modCount2.x + modCount1.y + modCount2.y) == 0) 
            {
                LogIllegalConnection(modCount1, modCount2);
                CleanFailedConnection();
                return false;
            }


            // Now that the forbidden illegal cases have been resolved, we can go sort out the how the order goes for the legal cases

            PatchSocket parent = null;
            PatchSocket child  = null;

            int sum1 = modCount1.x + modCount1.y + modCount1.z;
            int sum2 = modCount2.x + modCount2.y + modCount2.z;


            /// NOTE ABOUT HYBRID CONFLIC RESOLUTION: 
            /// Currently there are only Object-Logic hybrids in existence.
            /// The rejection of impossible cases in the beginning makes sure no impossible and wonky stuff happens.
            /// Othewise the order is simply decided upon what order the sockets are dragged in

            if (sum1 <= 1 && sum2 <= 1) // Are we dealing with a pair of pure sockets?
                goto pure; // move to pure section.
            else
                goto hybrid; // move to hybrid scetion


            hybrid: // In hybrid section socket1 gets priority as it is the starting location of the dragged socket

            if(modCount1.x > 0 && sum1 == 1) // check if socket 1 is a pure object socket
            {
                // 1 is a pure object, thus 2 must be the hybrid and will be the parent
                parent = socket2;
                child = socket1;
                goto connect;
            }

            if (modCount2.x > 0 && sum1 == 1)
            {
                // 2 is a pure object, thus 1 must be the hybrid and will be the parent.
                parent = socket1;
                child = socket2;
                goto connect;
            }

            if (modCount1.z > 0 && sum1 == 1) // check if socket 1 is a pure logic socket
            {
                // 1 is a pure logic, thus 2 must be the hybrid and will be the parent
                parent = socket2;
                child = socket1;
                goto connect;
            }

            if (modCount2.z > 0 && sum1 == 1)
            {
                // 2 is a pure logic, thus 1 must be the hybrid and will be the parent.
                parent = socket1;
                child = socket2;
                goto connect;
            }

            /// Remaining cases are conflicts between a hybrid and pure effect socket
            /// This can simply be resolved by giving priority to socket 1 as parent
            {
                parent = socket1;
                child = socket2;
                goto connect;
            }

            pure:
            // Effect-Object connection
            if (modCount1.x > 0 && modCount2.y > 0) // 1 is object and 2 is effect
            {
                parent = socket2; // Effect becomes parent
                child  = socket1; // Object becomes child
                goto connect;
            }
            else if (modCount1.y > 0 && modCount2.x > 0) // 1 is effect and 2 is object
            {
                parent = socket1; // Effect becomes parent
                child  = socket2; // Object becomes child
                goto connect;
            }

            // Logic-Effect connection
            if (modCount1.z > 0 && modCount2.y > 0) // 1 is logic and 2 is effect
            {
                parent = socket1; // Logic becomes parent
                child = socket2;  // Effect becomes child
                goto connect;
            }
            else if (modCount1.y > 0 && modCount2.z > 0) // 1 is effect and 2 is logic
            {
                parent = socket2; // Logic becomes parent
                child = socket1;  // Effect becomes child
                goto connect;
            }

            // Logic-Object connection
            if (modCount1.x > 0 && modCount2.z > 0) // 1 is object and 2 is logic
            {
                parent = socket2; // Logic becomes parent
                child = socket1;  // Object becomes child
                goto connect;
            }
            else if (modCount1.z > 0 && modCount2.x > 0) // 1 is logic and 2 is object
            {
                parent = socket1; // Logic becomes parent
                child = socket2;  // Object becomes child
                goto connect;
            }

            // Error check again in case of emergencies
            if (parent == null || child == null)
            {
                Debug.LogError("NULL PARENT OR CHILD.");

                if (parent == null)
                    Debug.LogError("p null!");
                if (child == null)
                    Debug.LogError("c null!");

                return false;
            }


            connect:
            {
                // Unpack prefab to save changes in the scene
                UnpackPrefab(parent.gameObject);
                UnpackPrefab(child.gameObject);

                // Add the new connection
                parent.AddSocket(child);
                Vector3 p1 = GetBoardPosition(parent.transform);
                Vector3 p2 = GetBoardPosition(child.transform);

                currentCable.ConfigurePatchCable(p1, p2);
                currentCable.SetCableSockets(parent, child);
                currentCable.SetCableType(PATCH_TYPE.NORMAL);

                currentCable = null;
            }


            // The connection is complete!

            return true; // Succesful connection

            /// NOTE TO MYSELF WHEN I KEEP CODING THIS FORWARD: CONSULT THE TECH NOTES AND THING OF THE CONNECTIONS IN TERMS OF WHAT THEY DO NOT HAVE

        }

        public static void DisconnectPatch(PatchCable cable) // order of the sockets does not matter
        {
            cable.Disconnect();
        }

        public static void CleanFailedConnection()
        {
            if (currentCable == null)
                return;

            if (Application.isEditor)
                Object.DestroyImmediate(currentCable.gameObject);
            else
                GameObject.Destroy(currentCable.gameObject);

            currentCable = null;
        }

        public static bool CheckForExistingConnection(PatchSocket parent, PatchSocket child)
        {
            foreach (PatchSocket socket in parent.children)
            {
                if (socket == child)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// x = object, y = effect, z = logic
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private static Vector3Int GetSocketModCount(PatchSocket socket)
        {
            Vector3Int modCount = new Vector3Int(0, 0, 0);

            foreach (PatchModifier mod in socket.modifiers)
            {
                if (mod is ObjectMaster)
                    ++modCount.x;

                if (mod is EffectMaster)
                    ++modCount.y;

                if (mod is LogicMaster)
                    ++modCount.z;
            }

            return modCount;
        }


    
    // Unity doesn't want to save scene changes made with our custom editor tool unless we unpack prefabs
    public static void UnpackPrefab(UnityEngine.Object prefabInstance)
    {
        #if UNITY_EDITOR
        if(PrefabUtility.GetPrefabInstanceStatus(prefabInstance as GameObject) == PrefabInstanceStatus.NotAPrefab)
        {
            return;
        }
        PrefabUtility.UnpackPrefabInstance((prefabInstance as GameObject), PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        //EditorSceneManager.MarkAllScenesDirty();
        //EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        #endif
    }
        private static void LogIllegalConnection(Vector3Int socket1, Vector3 socket2)
        {
            Debug.Log("Tried to make illegal connection!\n"
                     + "First socket: [" + socket1.x.ToString() + ", " + socket1.y.ToString() + ", " + socket1.z.ToString() + "]\n"
                    + "Second socket: [" + socket2.x.ToString() + ", " + socket2.y.ToString() + ", " + socket2.z.ToString() + "]");
        }
    }
}
