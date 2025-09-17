using System;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Localization;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class LocalizationImporter : EditorWindow
{
    private string googleSheetUrl = "";
    private LocalizationData localizationData;
    private bool isImporting = false;
    private string statusMessage = "";
    private MessageType statusType = MessageType.Info;
    private static CancellationTokenSource _сts;

    [MenuItem("Tools/Localization/Import from Google Sheet")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationImporter>("Localization Importer");
        _сts = new CancellationTokenSource();
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheet CSV Importer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // URL Field
        GUILayout.Label("Google Sheet CSV URL", EditorStyles.boldLabel);
        googleSheetUrl = EditorGUILayout.TextField("URL", googleSheetUrl);
        
        EditorGUILayout.HelpBox(
            "Для получения CSV ссылки:\n" +
            "1. Открой Google таблицу\n" +
            "2. Файл → Опубликовать в интернете\n" +
            "3. Выбери лист и формат CSV\n" +
            "4. Скопируй ссылку", 
            MessageType.Info);
        
        EditorGUILayout.Space();

        // LocalizationData Field
        GUILayout.Label("Localization Data Asset", EditorStyles.boldLabel);
        localizationData = (LocalizationData)EditorGUILayout.ObjectField(
            "Localization Data", 
            localizationData, 
            typeof(LocalizationData), 
            false);
        
        EditorGUILayout.Space();

        // Status Message
        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.HelpBox(statusMessage, statusType);
        }

        EditorGUILayout.Space();

        // Import Button
        GUI.enabled = !isImporting && localizationData != null;
        if (GUILayout.Button(isImporting ? "Importing..." : "Import from Google Sheet"))
        {
            ImportLocalization().Forget();
        }
        GUI.enabled = true;

        // Test Button
        if (GUILayout.Button("Test URL"))
        {
            TestUrl().Forget();
        }
    }

    private async UniTaskVoid ImportLocalization()
    {
        if (localizationData == null)
        {
            ShowStatus("Выбери LocalizationData asset!", MessageType.Error);
            return;
        }
        
        if (string.IsNullOrEmpty(googleSheetUrl)) {
            googleSheetUrl = localizationData.url;
        }
        
        if (string.IsNullOrEmpty(googleSheetUrl)) {
            googleSheetUrl = localizationData.url;
            ShowStatus("Укажи ссылку на Google таблицу!", MessageType.Error);
            return;
        }

        isImporting = true;
        ShowStatus("Начинаю импорт...", MessageType.Info);
        await DownloadAndParse(_сts.Token);
    }

    private async UniTaskVoid TestUrl()
    {
        if (string.IsNullOrEmpty(googleSheetUrl))
        {
            ShowStatus("Укажи ссылку для тестирования!", MessageType.Warning);
            return;
        }

        await TestUrlCoroutine(_сts.Token);
    }

    private async UniTask TestUrlCoroutine(System.Threading.CancellationToken token)
    {
        ShowStatus("Тестирую ссылку...", MessageType.Info);
        
        using (UnityWebRequest www = UnityWebRequest.Get(googleSheetUrl))
        {
            var op = www.SendWebRequest();
            while (!op.isDone && !token.IsCancellationRequested)
            {
                await UniTask.Yield();
            }
            if (token.IsCancellationRequested) return;

            if (www.result != UnityWebRequest.Result.Success)
            {
                ShowStatus($"Ошибка: {www.error}", MessageType.Error);
            }
            else
            {
                string content = www.downloadHandler.text;
                if (string.IsNullOrEmpty(content))
                {
                    ShowStatus("Ссылка работает, но содержимое пустое", MessageType.Warning);
                }
                else
                {
                    ShowStatus($"Ссылка работает! Получено {content.Length} символов", MessageType.Info);
                }
            }
        }
    }

    private async UniTask DownloadAndParse(System.Threading.CancellationToken token)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(googleSheetUrl))
        {
            var op = www.SendWebRequest();
            while (!op.isDone && !token.IsCancellationRequested)
            {
                await UniTask.Yield();
            }
            if (token.IsCancellationRequested)
            {
                isImporting = false;
                return;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                ShowStatus($"Ошибка загрузки: {www.error}", MessageType.Error);
                isImporting = false;
                return;
            }

            string csvContent = www.downloadHandler.text;
            if (string.IsNullOrEmpty(csvContent))
            {
                ShowStatus("Получено пустое содержимое", MessageType.Error);
                isImporting = false;
                return;
            }

            ParseCSV(csvContent);
        }
    }

    private void ParseCSV(string csv)
    {
        try
        {
            csv = csv.Replace("\r\n", "\n").Replace("\r", "\n");
            csv = csv.Trim('\uFEFF');
            var lines = SplitCsvLines(csv);
            if (lines.Count < 2)
            {
                ShowStatus("CSV файл должен содержать заголовок и хотя бы одну строку данных", MessageType.Error);
                isImporting = false;
                return;
            }
            var headerLine = lines[0].Trim();
            char delimiter = headerLine.Contains("|") ? '|' : ',';
            var headers = ParseCSVLine(headerLine, delimiter);
            if (headers.Length < 3)
            {
                ShowStatus("CSV должен содержать минимум 3 колонки: Key, ru, en", MessageType.Error);
                isImporting = false;
                return;
            }
            localizationData.entries.Clear();
            var keys = new HashSet<string>();
            int importedCount = 0;
            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                var columns = ParseCSVLine(line, delimiter);
                if (columns.Length >= 3)
                {
                    var key = columns[0].Trim();
                    if (string.IsNullOrEmpty(key)) continue;
                    if (keys.Contains(key)) continue;
                    keys.Add(key);
                    var entry = new LocalizationData.LocalizationEntry(
                        key,
                        columns[1].Trim(),
                        columns[2].Trim()
                    );
                    localizationData.entries.Add(entry);
                    importedCount++;
                }
            }
            EditorUtility.SetDirty(localizationData);
            AssetDatabase.SaveAssets();
            localizationData.ClearCache();
            ShowStatus($"Импорт завершен! Импортировано {importedCount} записей", MessageType.Info);
        }
        catch (System.Exception e)
        {
            ShowStatus($"Ошибка парсинга CSV: {e.Message}", MessageType.Error);
        }
        isImporting = false;
    }

    private List<string> SplitCsvLines(string csv)
    {
        var result = new List<string>();
        bool inQuotes = false;
        string currentLine = "";
        for (int i = 0; i < csv.Length; i++)
        {
            char c = csv[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
                currentLine += c;
            }
            else if (c == '\n' && !inQuotes)
            {
                result.Add(currentLine);
                currentLine = "";
            }
            else
            {
                currentLine += c;
            }
        }
        if (!string.IsNullOrEmpty(currentLine))
            result.Add(currentLine);
        return result;
    }

    private string[] ParseCSVLine(string line, char delimiter)
    {
        var result = new List<string>();
        bool inQuotes = false;
        string currentField = "";
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField += '"';
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                result.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }
        result.Add(currentField);
        return result.ToArray();
    }

    private void ShowStatus(string message, MessageType type)
    {
        statusMessage = message;
        statusType = type;
        Repaint();
    }

    private void OnDestroy() {
        _сts?.Cancel();
    }
} 