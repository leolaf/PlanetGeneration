using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class SphericalPlane : MonoBehaviour
{
    [SerializeField][Range(1, 500)] private int resolution = 100;

    [SerializeField][Range(1, 50)] public float width = 10;

    [SerializeField][Range(1, 50)] private float height = 10;

    [SerializeField,HideInInspector] private MeshFilter meshFilter;

    private void OnValidate()
    {
        BuildMesh();
    }

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    void BuildMesh()
    {
        Mesh msh = new Mesh();
        msh.name = "Spherical Plane";

        msh.vertices = CreateVertices();
        msh.triangles = BuildTriangles();

        meshFilter.mesh = msh;           
    }

    /// <summary>
    ///     Creates all needed vertices
    /// </summary>
    Vector3[] CreateVertices()
    {
        Vector3[] vertices = new Vector3[(resolution + 1)*(resolution + 1)];
        float x_start = -width / 2f;
        float y_start = height / 2f;
        float x_step = width / (float)resolution;
        float y_step = height / (float)resolution;
        for (int line = 0; line < resolution + 1; line++)
        {
            for (int column = 0; column < resolution + 1; column++)
            {
                Vector3 pointInWorld = new Vector3(x_start + column * x_step,transform.position.magnitude, y_start - line * y_step).normalized - Vector3.up;
                vertices[line * (resolution + 1) + column] = pointInWorld * 10;
            }
        }
        return vertices;
    }

    int[] BuildTriangles()
    {
        int[] triangles = new int[resolution * resolution * 2 * 3];

        for (int line = 0; line < resolution; line++)
        {
            for (int column = 0; column < resolution; column++)
            {
                BuildQuad(line, column, ref triangles);
            }
        }

        return triangles;
    }

    void BuildQuad(int line, int column, ref int[] triangles)
    {
        int start_triangle_index = (resolution + 1) * line + column;
        int quad_start = 6 * (column + line * resolution);
        triangles[quad_start] = start_triangle_index;
        triangles[quad_start + 1] = start_triangle_index + 1;
        triangles[quad_start + 2] = start_triangle_index + 1 + resolution;
        triangles[quad_start + 3] = start_triangle_index + 1;
        triangles[quad_start + 4] = start_triangle_index + 2 + resolution;
        triangles[quad_start + 5] = start_triangle_index + 1 + resolution;
    }
}
