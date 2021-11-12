using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InspectorTest : MonoBehaviour
{
    public string Name;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(InspectorTest))]
public class InspectorTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Click Me"))
        {
            //Logic
            InspectorTest ctr = target as InspectorTest;
            ctr.Name = "ouka";
            EditorUtility.SetDirty(ctr);
        }

        if (GUILayout.Button("Click Me 2"))
        {
            serializedObject.FindProperty("Name").stringValue = "ouka2";
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif