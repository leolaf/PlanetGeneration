using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetInstance))]
public class PlanetInstanceEditor : Editor
{
    PlanetInstance planetInstance;



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Show Planet"))
        {
            planetInstance.InitializePlanet();
        }
    }

    private void OnEnable()
    {
        planetInstance = (PlanetInstance)target;
    }
}
