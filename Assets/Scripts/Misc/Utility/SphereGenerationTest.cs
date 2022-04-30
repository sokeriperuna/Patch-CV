using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SphereGenerationTest : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private PatchCableMaterials materials;

    private Mesh myMesh;

    public bool drawGizmos;
    public int resolution;
    public float radius;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        myMesh = GenerateSphereMesh();

        meshFilter.mesh = myMesh;

        materials = Resources.Load<PatchCableMaterials>("Data/BasicPatchMats");
        meshRenderer.material = materials.normal;
    }

    Mesh GenerateSphereMesh()
    {
        if (resolution <= 0 || radius <= 0f)
            return null;

        int vCount = resolution * resolution + 2;
        Vector3[] vertices = new Vector3[vCount]; // How many vertextes we will have connecting things all over

        // Generate vertex positions

        vertices[0] = new Vector3(0, -radius, 0f); // Set bottom vertex.

        float hSegement = 2 * Mathf.PI / resolution; // we have RES number of horizontal vertexes that wrap around the whole sphere
        float vSegment = Mathf.PI / (resolution + 1); // We have RES number of vertical vertexes that travel up the sphere

        for (int row = 1, index = 1; row < (resolution + 1); row++) // Generate all the vertices between the top and bottom
        {
            for (int column = 0; column < resolution; column++)
            {
                float xPos, yPos, zPos;

                // Y-axis position
                yPos = Mathf.Sin(-0.5f * Mathf.PI + vSegment * row); // Which row are we on? How high up should we be?

                // XZ-plane positions
                float rowRadius = Mathf.Sqrt(radius * radius - yPos * yPos); // We get the radius of the given row using the height of the row.

                // Get positions based on trigonometry and then scale them to the correct size.
                xPos = Mathf.Cos(hSegement * column) * rowRadius;
                zPos = Mathf.Sin(hSegement * column) * rowRadius;

                vertices[index++] = new Vector3(xPos, yPos, zPos);
            }
        }

        vertices[vCount-1] = new Vector3(0, radius, 0f); // Set top vertex.


        // How many triangles will we have?
        int middleQuadTris  = resolution * (resolution + 1) *2;
        int topTris         = 2 * resolution; 
        int[] triangles     = new int[(middleQuadTris + topTris)*3];

        int vIndex = 0; // Index of vertex that we're assinging in amongs the triangles.

        // Generate bottom triangles 
        for(int b=0; b<resolution; b++)
        {
            triangles[vIndex++] = 0;
            triangles[vIndex++] = b + 1;

            if ((b + 2) > resolution) // Check if we're about to go up a row
                triangles[vIndex++] =  1; // We've gone past the row so we go back to the start to connect the sphere bottom
            else
                triangles[vIndex++] =  b + 2; // Everything is ok, nothing to worry about.
        }

        // Generate middle quads
        for(int r=0; r<(resolution-1); r++) // we go through the bottoms of each row (there's res-1 of them) and generate quads
        {
            for (int c = 1; c < resolution; c++) // We go throug each bottom vertex except the last (+1 compensates for bottom vertex)
            {
                // Bottom left triangle
                triangles[vIndex++] = c +     r*resolution; // Bottom-left vertex of quad
                triangles[vIndex++] = c +    (r+1)*resolution; // Top-left vertex of quad
                triangles[vIndex++] = c + 1 + r*resolution; // Top-right vertex of quad

                // Top right triangle
                triangles[vIndex++] = c + 1 + r * resolution; // Bottom-right vertex of quad
                triangles[vIndex++] = c +    (r + 1) * resolution; // Top-left vertex of quad
                triangles[vIndex++] = c + 1 +(r + 1) * resolution; // Top-right vertex
            }

            /// We've reached the edge of the row so now we wrap back to where we started

            // Bottom left triangle
            triangles[vIndex++] = resolution +  r * resolution; // Bottom-left vertex of quad
            triangles[vIndex++] = resolution + (r+1) * resolution; // Top-left vertex of quad
            triangles[vIndex++] = 1 + r * resolution; // Top-right vertex of quad

            // Top right triangle
            triangles[vIndex++] = 1          +  r    * resolution; // Bottom right vertex
            triangles[vIndex++] = resolution + (r+1) * resolution; // Top-left vertex of quad
            triangles[vIndex++] = 1          + (r+1) * resolution; // Top-right vertex
        }

        int vertexBase = 1 + (resolution - 1) * resolution;
        // Generate top triangles
        for (int t = 0; t < resolution; t++)
        {
            triangles[vIndex++] = vertexBase + t;
            triangles[vIndex++] = vertices.Length - 1; // The top of the sphere

            if ((t + 2) > resolution)
                triangles[vIndex++] = vertexBase;
            else
                triangles[vIndex++] = vertexBase + t + 1;
        }

        Mesh newMesh = new Mesh();

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;

        newMesh.RecalculateNormals();

        //foreach (Vector3 v in newMesh.vertices)
            //Debug.Log(v.ToString());

        return newMesh;
    }

    private void OnDrawGizmos()
    {
        if(drawGizmos & myMesh != null)
        {
            int l = myMesh.vertices.Length;
            for (int i=0; i<l; i++)
            {
                if(i<(l-1))
                {
                    Gizmos.color = Color.Lerp(Color.red, Color.green, (((float)i) / l));
                    Gizmos.DrawLine(transform.position+myMesh.vertices[i], transform.position+myMesh.vertices[i + 1]);
                }

                Debug.Log(myMesh.vertices[i].ToString());
                Gizmos.DrawSphere(transform.position+ myMesh.vertices[i], 0.1f);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }

    }
}
