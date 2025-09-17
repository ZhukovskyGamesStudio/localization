using UnityEditor;
using UnityEngine;
using Localization;

public class LocalizationPrefabCreator
{
    [MenuItem("Tools/Localization/Create LocalizationManager Prefab")]
    public static void CreateLocalizationManagerPrefab()
    {
        GameObject localizationManagerGO = new("LocalizationManager");
        
        LocalizationManager localizationManager = localizationManagerGO.AddComponent<LocalizationManager>();
        
        LocalizationData localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>("Assets/Scripts/Localization/LocalizationData.asset");
        
        if (localizationData == null)
        {
            localizationData = ScriptableObject.CreateInstance<LocalizationData>();
            AssetDatabase.CreateAsset(localizationData, "Assets/Scripts/Localization/LocalizationData.asset");
            AssetDatabase.SaveAssets();
        }
        
        SerializedObject serializedObject = new(localizationManager);
        SerializedProperty localizationDataProperty = serializedObject.FindProperty("_localizationData");
        localizationDataProperty.objectReferenceValue = localizationData;
        serializedObject.ApplyModifiedProperties();
        
        string prefabPath = "Assets/Prefabs/Managers/LocalizationManager.prefab";
        
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(localizationManagerGO, prefabPath);
        
        Object.DestroyImmediate(localizationManagerGO);
        
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }
    
    [MenuItem("Tools/Localization/Setup Localization System")]
    public static void SetupLocalizationSystem()
    {
        LocalizationData localizationData = AssetDatabase.LoadAssetAtPath<LocalizationData>("Assets/Scripts/Localization/LocalizationData.asset");
        
        if (localizationData == null)
        {
            localizationData = ScriptableObject.CreateInstance<LocalizationData>();
            AssetDatabase.CreateAsset(localizationData, "Assets/Scripts/Localization/LocalizationData.asset");
            AssetDatabase.SaveAssets();
        }
        
        CreateLocalizationManagerPrefab();
        
        LocalizationImporter.ShowWindow();
    }
} 