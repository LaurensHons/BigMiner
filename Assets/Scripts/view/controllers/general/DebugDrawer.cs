using System.Collections.Generic;
using UnityEngine;

public class DebugDrawer
{
    public static List<GameObject> textList = new List<GameObject>();
    
    public DebugDrawer()
    {
        
    }
    
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 0) {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = CreateText(parent, text, localPosition, fontSize, color, textAnchor, textAlignment,
            sortingOrder);
        textList.Add(gameObject);
        return gameObject.GetComponent<TextMesh>();
    }
    
    public static TextMesh CreateAStarText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = CreateText(parent, text, localPosition, fontSize, color, textAnchor, textAlignment,
            sortingOrder);
        return gameObject.GetComponent<TextMesh>();
    }
    
    private static GameObject CreateText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color,
        TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = new Vector3(.1f, .1f, .1f);
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return gameObject;
    }

    public static void DeleteCreateWorldText()
    {
        if (textList != null && textList.Count != 0)
        {
            foreach (var gameObject in textList)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
