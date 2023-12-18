using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/*
 * Each planet is a sphere, we use 6 planes (corresponding to every face of a cube).
 * We then normalize the position of each triangle to get a sphere shape.
 * - Pros : each triangles are roughly the same size and the resolution of the plane can be changed easily
 * - Cons : the surface normals aren't conistent around the edges, which will lead to visible seams when lit
 * 
 * The planet script is responsible for creating the 6 terrains faces
 * and telling them which direction their normals are facing.
 */
public class Planet : MonoBehaviour
{
    [Range(2, 255)] public int resolution = 10;                                     // Resolution of a TerrainFace
    public bool autoUpdate = true;
    public string planetName = "Planet";

    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };      // Faces that can be shown
    public FaceRenderMask faceRenderMask;                                           // Face to show in editor

    public ShapeSettings shapeSettings;                                             // Settings (file) to determine the planet shape
    public ColorSettings colorSettings;                                             // Settings (file) to determine the planet colors

    [HideInInspector] public bool shapeSettingsFoldout;                             // Is shape settings unwrapped in inspector
    [HideInInspector] public bool colorSettingsFoldout;                             // Is color settings unwrapped in inspector

    ShapeGenerator shapeGenerator = new ShapeGenerator();                           // Instance of ShapeGenerator that will build the planet mesh
    ColorGenerator colorGenerator = new ColorGenerator();                           // Instance of ColorGenerator that will build the planet texture

    [SerializeField, HideInInspector] 
    MeshFilter[] meshFilters;                                                       // Keep meshes in memory in order to not create GameObject at each update
    TerrainFace[] terrainFaces;                                                     // TerrainFace array holding each terrain face of the planet

    // On Inspector value change, generate planet
    private void OnValidate()
    {
        GeneratePlanet();
    }

    /// <summary>
    ///     Initialize all planet attributes and gameObject structure
    /// </summary>
    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        // loop to create each faces of the sphere
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject($"Face ({i})");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    /// <summary>
    ///     Generate the planet shape and colors
    /// </summary>
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    /// <summary>
    ///     When shape settings is updated then show results if autoUpdate is true
    /// </summary>
    public void OnShapeSettingsUpdated()
    {
        if (!autoUpdate) { return; }
        Initialize();
        GenerateMesh();
    }

    /// <summary>
    ///     When color settings is updated then show results if autoUpdate is true
    /// </summary>
    public void OnColorSettingsUpdated()
    {
        if (!autoUpdate) { return; }
        Initialize();
        GenerateColors();
    }

    /// <summary>
    ///     Generate each faces mesh
    /// </summary>
    void GenerateMesh()
    {
        for(int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    /// <summary>
    ///     Generate planet color handlers
    /// </summary>
    void GenerateColors()
    {
        colorGenerator.UpdateColors();
        for (int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colorGenerator);
            }
        }
    }

    /// <summary>
    ///     Save planet for people to use it as prefabs.
    /// </summary>
    public void SavePlanet()
    {
        string relativePathToSave = Path.Join("PlanetSaves", planetName);
        string planetAssetPath = Path.Join("Assets", relativePathToSave);

        // Save texture
        Directory.CreateDirectory(planetAssetPath);
        AssetDatabase.CreateAsset(colorGenerator.texture, Path.Join(planetAssetPath, "texture.asset"));
        AssetDatabase.SaveAssets();

        // Save Material
        Shader planetShader = Shader.Find("Shader Graphs/Planet");
        Material planetMat = null;
        try
        {
            planetMat = new Material(planetShader);
        }
        catch
        {
            Debug.LogError("Could not save planet : Shader not found");
        }
        AssetDatabase.CreateAsset(planetMat, Path.Join(planetAssetPath, "Material.mat"));
        AssetDatabase.SaveAssets();

        // Don't forget to apply elevation min max and texture on the material asset
        var mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Join(planetAssetPath, "Material.mat"));
        var txture = AssetDatabase.LoadAssetAtPath<Texture>(Path.Join(planetAssetPath, "texture.asset"));
        mat.SetVector("_elevationMinMax", new Vector4(shapeGenerator.elevationMinMax.Min, shapeGenerator.elevationMinMax.Max));
        mat.SetTexture("_texture", txture);
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Save shape and color settings
        ColorSettings newColorSettings = colorSettings.Clone();
        newColorSettings.planetMaterial = mat;
        ShapeSettings newShapeSettings = shapeSettings.Clone();
        AssetDatabase.CreateAsset(newColorSettings, Path.Join(planetAssetPath, "colorSettings.asset"));
        AssetDatabase.CreateAsset(newShapeSettings, Path.Join(planetAssetPath, "shapeSettings.asset"));
        AssetDatabase.SaveAssets();

        // Save mesh
        foreach(var meshFilter in meshFilters)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = meshFilter.sharedMesh.vertices;
            mesh.triangles = meshFilter.sharedMesh.triangles;
            mesh.normals = meshFilter.sharedMesh.normals;
            mesh.uv = meshFilter.sharedMesh.uv;
            AssetDatabase.CreateAsset(mesh, Path.Join(planetAssetPath, $"{meshFilter.gameObject.name}.asset"));
        }
        AssetDatabase.SaveAssets();

        // Save a planet instance as a prefab
        GameObject savePrefab = new GameObject();
        savePrefab.AddComponent<PlanetInstance>().planetPath = planetAssetPath;
        PrefabUtility.SaveAsPrefabAsset(savePrefab, Path.Join(planetAssetPath, $"{planetName}.prefab"));
    }
}
