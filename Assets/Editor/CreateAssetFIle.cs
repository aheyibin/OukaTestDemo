using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateAssetFIle : UnityEditor.Editor
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("Ouka/CreateAssetFile")]
    static void CreateAssetFile()
    {
        ScriptableObject temp = ScriptableObject.CreateInstance<TestAssets>();
        AssetDatabase.CreateAsset(temp, "Assets/Data/TestAssets.asset");
        Debug.Log("CreateAssetFile Success");
    }
}
