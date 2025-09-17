using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Localization;

public class LocalizationEditor : EditorWindow
{
    private LocalizationData localizationData;
    private Vector2 scrollPosition;
    private string searchFilter = "";
    private bool showOnlyMissing = false;
    private string newKey = "";
    private string newRussian = "";
    private string newEnglish = "";

    [MenuItem("Tools/Localization/Localization Editor")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationEditor>("Localization Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Localization Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // LocalizationData Field
        localizationData = (LocalizationData)EditorGUILayout.ObjectField(
            "Localization Data", 
            localizationData, 
            typeof(LocalizationData), 
            false);

        if (localizationData == null)
        {
            EditorGUILayout.HelpBox("Выберите LocalizationData для редактирования", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();

        // Search and Filter
        GUILayout.Label("Search & Filter", EditorStyles.boldLabel);
        searchFilter = EditorGUILayout.TextField("Search", searchFilter);
        showOnlyMissing = EditorGUILayout.Toggle("Show Only Missing", showOnlyMissing);

        EditorGUILayout.Space();

        // Add New Entry
        GUILayout.Label("Add New Entry", EditorStyles.boldLabel);
        newKey = EditorGUILayout.TextField("Key", newKey);
        newRussian = EditorGUILayout.TextField("Russian", newRussian);
        newEnglish = EditorGUILayout.TextField("English", newEnglish);

        if (GUILayout.Button("Add Entry"))
        {
            AddNewEntry();
        }

        EditorGUILayout.Space();

        // Statistics
        ShowStatistics();

        EditorGUILayout.Space();

        // Entries List
        GUILayout.Label("Localization Entries", EditorStyles.boldLabel);
        ShowEntriesList();
    }

    private void ShowStatistics()
    {
        if (localizationData.entries == null) return;

        int totalEntries = localizationData.entries.Count;
        int missingRussian = localizationData.entries.Count(e => string.IsNullOrEmpty(e.ru));
        int missingEnglish = localizationData.entries.Count(e => string.IsNullOrEmpty(e.en));
        int completeEntries = localizationData.entries.Count(e => !string.IsNullOrEmpty(e.ru) && !string.IsNullOrEmpty(e.en));

        GUILayout.Label("Statistics", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Total Entries", totalEntries.ToString());
        EditorGUILayout.LabelField("Complete Entries", completeEntries.ToString());
        EditorGUILayout.LabelField("Missing Russian", missingRussian.ToString());
        EditorGUILayout.LabelField("Missing English", missingEnglish.ToString());
    }

    private void ShowEntriesList()
    {
        if (localizationData.entries == null) return;

        var filteredEntries = localizationData.entries
            .Where(e => string.IsNullOrEmpty(searchFilter) || 
                       e.key.ToLower().Contains(searchFilter.ToLower()) ||
                       e.ru.ToLower().Contains(searchFilter.ToLower()) ||
                       e.en.ToLower().Contains(searchFilter.ToLower()))
            .Where(e => !showOnlyMissing || 
                       string.IsNullOrEmpty(e.ru) || 
                       string.IsNullOrEmpty(e.en))
            .ToList();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < filteredEntries.Count; i++)
        {
            var entry = filteredEntries[i];
            bool isMissing = string.IsNullOrEmpty(entry.ru) || string.IsNullOrEmpty(entry.en);

            EditorGUILayout.BeginVertical("box");
            
            // Key
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", GUILayout.Width(100));
            string newKey = EditorGUILayout.TextField(entry.key);
            if (newKey != entry.key)
            {
                entry.key = newKey;
                EditorUtility.SetDirty(localizationData);
            }
            EditorGUILayout.EndHorizontal();

            // Russian
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Russian", GUILayout.Width(100));
            string newRussian = EditorGUILayout.TextField(entry.ru);
            if (newRussian != entry.ru)
            {
                entry.ru = newRussian;
                EditorUtility.SetDirty(localizationData);
            }
            EditorGUILayout.EndHorizontal();

            // English
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("English", GUILayout.Width(100));
            string newEnglish = EditorGUILayout.TextField(entry.en);
            if (newEnglish != entry.en)
            {
                entry.en = newEnglish;
                EditorUtility.SetDirty(localizationData);
            }
            EditorGUILayout.EndHorizontal();

            // Actions
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete", 
                    $"Are you sure you want to delete the entry '{entry.key}'?", 
                    "Delete", "Cancel"))
                {
                    localizationData.entries.Remove(entry);
                    EditorUtility.SetDirty(localizationData);
                    AssetDatabase.SaveAssets();
                }
            }

            if (isMissing)
            {
                EditorGUILayout.LabelField("⚠ Missing translation", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField("✓ Complete", EditorStyles.boldLabel);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();

        if (filteredEntries.Count == 0)
        {
            EditorGUILayout.HelpBox("No entries found matching the current filter.", MessageType.Info);
        }
    }

    private void AddNewEntry()
    {
        if (string.IsNullOrEmpty(newKey))
        {
            EditorUtility.DisplayDialog("Error", "Key cannot be empty!", "OK");
            return;
        }

        // Check if key already exists
        if (localizationData.entries.Any(e => e.key == newKey))
        {
            EditorUtility.DisplayDialog("Error", $"Key '{newKey}' already exists!", "OK");
            return;
        }

        var newEntry = new LocalizationData.LocalizationEntry(newKey, newRussian, newEnglish);
        localizationData.entries.Add(newEntry);
        
        EditorUtility.SetDirty(localizationData);
        AssetDatabase.SaveAssets();

        // Clear input fields
        newKey = "";
        newRussian = "";
        newEnglish = "";

        Debug.Log($"Added new localization entry: {newEntry.key}");
    }
} 