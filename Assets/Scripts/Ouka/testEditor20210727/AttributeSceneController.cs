using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AttributeSceneController : MonoBehaviour
{
    [Preview]
    public Sprite sprite;
}


public class Preview : PropertyAttribute
{
    public Preview()
    {

    }

    [Multiline(5)]
    public string Comment;

}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Preview))]
public class PreviewDrawer : PropertyDrawer
{
    //调整整体高度
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 64f;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(position, property, label);

        // Preview
        Texture2D previewTexture = GetAssetPreview(property);
        if (previewTexture != null)
        {
            Rect previewRect = new Rect()
            {
                x = position.x + GetIndentLength(position),
                y = position.y + EditorGUIUtility.singleLineHeight,
                width = position.width,
                height = 64
            };
            GUI.Label(previewRect, previewTexture);

            //Rect previewRect2 = new Rect()
            //{
            //    x = previewRect.x + GetIndentLength(previewRect),
            //    y = previewRect.y + EditorGUIUtility.singleLineHeight,
            //    width = position.width,
            //    height = 64
            //};
            //GUI.Label(previewRect2,"miaoshu是XXXXX");
        }
        EditorGUI.EndProperty();
    }

    public static float GetIndentLength(Rect sourceRect)
    {
        Rect indentRect = EditorGUI.IndentedRect(sourceRect);
        float indentLength = indentRect.x - sourceRect.x;

        return indentLength;
    }

    Texture2D GetAssetPreview(SerializedProperty property)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (property.objectReferenceValue != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
                return previewTexture;
            }
            return null;
        }
        return null;
    }
}
#endif
