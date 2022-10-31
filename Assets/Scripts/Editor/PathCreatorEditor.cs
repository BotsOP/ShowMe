using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathCreator))]
public class PathCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathCreator pathCreator = (PathCreator)target;
        if(GUILayout.Button("Add segment"))
        {
            pathCreator.AddSegment();
        }
        if(GUILayout.Button("Remove segment"))
        {
            pathCreator.RemoveSegment();
        }
        if(GUILayout.Button("Reset"))
        {
            pathCreator.ResetPoints();
        }
    }
}
