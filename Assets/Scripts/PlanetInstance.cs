using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PlanetInstance : MonoBehaviour
{
    [SerializeField, HideInInspector] public string planetPath;

    private void OnValidate()
    {
        InitializePlanet();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePlanet();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x, 
            transform.rotation.eulerAngles.y + 40 * Time.deltaTime, 
            transform.rotation.eulerAngles.z
        );
    }

    public void InitializePlanet()
    {
        foreach (Transform child in transform)
        {
            StartCoroutine(Destroy(child.gameObject));
        }
        for (int i = 0; i < 6; i++)
        {
            GameObject face = new GameObject();
            face.transform.parent = transform;
            face.transform.position = transform.position;
            face.transform.rotation = transform.rotation;
            face.transform.localScale = transform.localScale;
            face.name = $"Face ({i})";
            face.AddComponent<MeshFilter>().mesh = AssetDatabase.LoadAssetAtPath<Mesh>(Path.Join(planetPath, $"Face ({i}).asset"));
            face.AddComponent<MeshRenderer>().material = AssetDatabase.LoadAssetAtPath<Material>(Path.Join(planetPath, "Material.mat"));
        }
    }

    IEnumerator Destroy(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }
}
