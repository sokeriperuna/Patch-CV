using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


public enum PATCH_TYPE
{
    NORMAL,
    STATIC
}

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class PatchCable : MonoBehaviour
{
    protected Vector3 origin;
    protected Vector3 end;
    [SerializeField] protected  PATCH_TYPE type;

    protected PatchCableMaterials materials;

    protected MeshRenderer  meshRenderer;
    protected MeshFilter    meshFilter;
    protected MeshCollider  meshCollider;
    protected Mesh currentMesh;

    protected const float zFightingClearance = 0.0001f; // Minimum distance for objects to be apart in order to stop z-fighting
    protected const float cableThickness = 0.1f;

    [SerializeField] protected int layerIndex = 0; // 0 is patch board, 1 is first visible layer. Layers are spaced 1e-4 apart.

    [SerializeField] protected PatchSocket parentSocket;
    [SerializeField] protected PatchSocket childSocket;

    protected void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter   = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        currentMesh  = new Mesh();

        materials = Resources.Load<PatchCableMaterials>("Data/BasicPatchMats");

        DeterminePatchLayer();
        //Debug.Log(layerIndex.ToString());
    }

    public void SetCableType(PATCH_TYPE patchType)
    {
        this.type = patchType; 
        switch (patchType)
        {
            case PATCH_TYPE.NORMAL:
                meshRenderer.material = materials.normal;
                break;

            case PATCH_TYPE.STATIC:
                meshRenderer.material = materials.locked;
                break;

            default:
                break;
        }
        //Debug.Log("Changed cable type to: " + this.type.ToString());
    }

    public void ConfigurePatchCable(Vector3 startPoint, Vector3 endPoint)
    {
        //Debug.Log("Configure called");
        /// Generate mesh
        if(currentMesh != null)
            currentMesh.Clear();
        else
        {
            currentMesh = new Mesh();
        }

        UpdateMesh(startPoint, endPoint, cableThickness);
        meshFilter.mesh = currentMesh;

        SetWorldScale(new Vector3(1f, 1f, 1f));

        Vector3 normal = transform.parent.forward.normalized;
        //Vector3 normal = (Vector3.Cross(startPoint, endPoint)).normalized;
        transform.position = (startPoint+endPoint)*0.5f - normal*zFightingClearance*layerIndex; // We position the mesh at the average of the two positions + layer it to avoid z-fighting

        //Debug.DrawLine((startPoint + endPoint) * 0.5f, (startPoint + endPoint) * 0.5f + normal * 10f, Color.red);
    }

    public void SetCableSockets(PatchSocket parent, PatchSocket child)
    {
        parentSocket = parent;
        childSocket = child;
        Debug.Log(parent.ToString() + " + " + child.ToString());
    }

    public void ResetPatchCable()
    {
        ClearMesh();
    }

    public void Disconnect()
    {
        //Debug.Log("Destroying connection between " + parentSocket.ToString() + " and " + /*childSocket.ToString() +*/ '.');
        parentSocket.RemoveSocket(childSocket);
        if (Application.isEditor)
            UnityEngine.Object.DestroyImmediate(this.gameObject);
        else
            GameObject.Destroy(this.gameObject);
    }

    public PATCH_TYPE GetCableType()
    {
        return type;
    }
    public int LayerIndex { get { return layerIndex; } }

    protected void DeterminePatchLayer()
    {
        GameObject[] cables = GameObject.FindGameObjectsWithTag("Cable");

        
        int[] layerIndexes = new int[cables.Length];

        for(int i=0; i<cables.Length; i++)
        {
            if (cables[i] == this.gameObject)
                layerIndexes[i] = -1; // Mark self.
            else
                layerIndexes[i] = cables[i].GetComponent<PatchCable>().LayerIndex;
        }

        int newLayerIndex = 1;
        while(true)
        {
            if (ExistsInArray(newLayerIndex, layerIndexes))
                newLayerIndex++; // increment and try the next layer
            else
                break; // we found a value that doesn't exist, so we don't increment
        }

        layerIndex = newLayerIndex;
    }

    protected bool ExistsInArray(int i, int[] intArray)
    {
        bool exists = false;
        foreach (int comp in intArray)
            if (i == comp)
                exists = true;

        return exists;
    }

    protected void SetWorldScale(Vector3 newScale)
    {
        Transform myParent      = transform.parent;
        transform.parent        = null;
        transform.localScale    = newScale;
        transform.parent        = myParent;
    }

    protected void UpdateMesh(Vector3 start, Vector3 end, float thickness)
    {
        Vector3[] vertices = new Vector3[4]; // We generate a simple quad for now.

        Vector3 xDir = end - start;

        Vector3 normal      = transform.parent.forward; //We get the normal vector of the socket that this is parented to
        Debug.DrawLine(transform.position, transform.position + normal, Color.blue);

        Vector3 yDir        = Vector3.Cross(normal, xDir);
        Vector3 yDirNorm    = yDir.normalized;

        // Generate vertex positions
        {
            vertices[0] = -xDir * 0.5f - yDirNorm * thickness * 0.5f;
            vertices[1] =  xDir * 0.5f - yDirNorm * thickness * 0.5f;
            vertices[2] = -xDir * 0.5f + yDirNorm * thickness * 0.5f;
            vertices[3] =  xDir * 0.5f + yDirNorm * thickness * 0.5f;
        }

        int[] triangles = new int[6];

        // Generate triangles
        {
            // 1st triangle
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 3;

            // 2nd triangle 
            triangles[3] = 0;
            triangles[4] = 3;
            triangles[5] = 1;
        }

        Vector2[] UVs = new Vector2[4];

        // Generate UV coordinates
        {
            UVs[0] = new Vector2(0, 0);
            UVs[1] = new Vector2(1, 0);
            UVs[2] = new Vector2(0, 1);
            UVs[3] = new Vector2(1, 1);
        }

        currentMesh.vertices  = vertices;
        currentMesh.triangles = triangles;
        currentMesh.uv = UVs;
        currentMesh.RecalculateNormals();


        float dirMag = xDir.magnitude; // Pass magnitue through second UV channel
        Vector2[] magnitudeMap = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            magnitudeMap[i] = new Vector2(dirMag, 0f);
        }
        currentMesh.uv2 = magnitudeMap;


        meshCollider.sharedMesh = currentMesh;
    }

    protected void ClearMesh()
    {
        currentMesh.Clear();
        meshFilter.mesh = currentMesh;
        meshCollider.sharedMesh = currentMesh;
    }


}
