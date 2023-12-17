using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TerrainFace
{
    ShapeGenerator shapeGenerator;      // Instance of shapeGenerator handling the terrain elevation at a given point
    Mesh mesh;                          // Mesh for the instance to work on
    int resolution;                     // Resolution of the plane mesh
    Vector3 localUp;                    // Which way the terrain face is facing (its Vector3.up)
    Vector3 axisA;                      // The terrain face relative Vector.right
    Vector3 axisB;                      // The terrain face relative Vector.forward

    /// <summary>
    ///     Constructor of TerrainFace
    /// </summary>
    /// <param name="shapeGenerator">The shape generator to use</param>
    /// <param name="mesh">The mesh to work on</param>
    /// <param name="resolution">The resolution of the plane mesh</param>
    /// <param name="localUp">Which way the terrain is facing</param>
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        axisA = new Vector3(localUp.y,localUp.z,localUp.x);
        axisB = Vector3.Cross(localUp,axisA);
    }

    /// <summary>
    ///     Build Mesh
    /// </summary>
    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];                  // We need resolution^2 vertices 
        int[] triangles = new int[(resolution -1) * (resolution - 1) * 6];          // Define the number of triangles
        int triIndex = 0;                                                           // Index that we'll increment for each vertex index we store in the triangle array

        // The uv map we will use to texturize the mesh (and so the planet)
        Vector2[] uv;
        if(mesh.uv.Length == vertices.Length) { uv = mesh.uv; }
        else { uv = new Vector2[vertices.Length]; }

        // For each vertex, evaluate it's position
        for (int y = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++)
            {
                // Vertex index
                int i = x + y * resolution;

                // Percentage on the plane shape on width and height
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                // Get the corresponding point on a cube position
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                // Normalize position to "bend" the plane (to make it look as a sphere)
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                // Evaluate the point position accordingly to the shape generator
                float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                // Scale it (so the points under the ocean are placed on the ocean surface
                vertices[i] = pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
                // Store the unscale value in the UV for the shader to know it's deepness
                uv[i].y = unscaledElevation;

                // Store the triangles depending on this point (except at the end of the mesh)
                if(x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        // Operate on the terrain face mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }

    /// <summary>
    ///     Since we have biomes on our planet, we need to store each vertex biome color on the UV
    /// </summary>
    /// <param name="colorGenerator">The color generator that will tell us on wich biome the point is situated</param>
    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        // Since we only use the first coordinate for the biome informations,
        // we need to keep the second one which we calculated earlier
        Vector2[] uv = mesh.uv;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
            }
        }
        mesh.uv = uv;
    }
}
