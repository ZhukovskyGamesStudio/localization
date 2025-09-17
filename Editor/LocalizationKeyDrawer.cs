using UnityEditor;
using UnityEngine;
using System.Linq;
using Localization;

[CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
public class LocalizationKeyDrawer : PropertyDrawer
{
    private static string _lastFileName;
    private static string[] _keys;

    private static void LoadKeys(string fileName)
    {
        var keys = LocalizationUtils.GetKeysForFile(fileName);
        _keys = keys.ToArray();
        _lastFileName = fileName;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
{
    if (property.propertyType != SerializedPropertyType.String)
    {
        EditorGUI.LabelField(position, label.text, "Use with string only");
        return;
    }

    var attr = (LocalizationKeyAttribute)attribute;
    //if (_lastFileName != attr.fileName)
        LoadKeys(attr.fileName);

    int currentIndex = Mathf.Max(0, System.Array.IndexOf(_keys, property.stringValue));
    string[] keys = _keys;
    GUIContent[] keyContents = keys
        .Select(k => new GUIContent(k, GetEnTooltip(k, attr.fileName)))
        .ToArray();

    int selectedIndex = EditorGUI.Popup(
        position,
        new GUIContent(label.text, GetEnTooltip(property.stringValue, attr.fileName)),
        currentIndex,
        keyContents
    );
    if (selectedIndex >= 0 && selectedIndex < _keys.Length)
        property.stringValue = _keys[selectedIndex];
}
    
    private string GetEnTooltip(string key, string fileName)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var guids = AssetDatabase.FindAssets("t:LocalizationData");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith("Localization.asset")) continue;
                var asset = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);
                if (asset != null && asset.name == fileName + "Localization")
                {
                    foreach (var entry in asset.entries)
                    {
                        if (entry.key == key)
                        {
                            // ищем английский перевод
                                return entry.en;
                        }
                    }
                }
            }
            return "";
        }
#endif
        // В рантайме используем обычный способ
        return LocalizationUtils.L(key, "en") ?? "";
    }
} 