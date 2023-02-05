using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IPlanet), true)]
public class CustomEditorPlanetGen : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IPlanet myScript = (IPlanet)target;
        if (GUILayout.Button("Update"))
        {
            myScript.UpdateViaEditor();
        }
    }
}
