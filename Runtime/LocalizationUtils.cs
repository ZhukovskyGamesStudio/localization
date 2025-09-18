#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace ZG_Localization {
    public static class LocalizationUtils {
        /// <summary>
        /// Получить локализованный текст по ключу
        /// </summary>
        public static string L(string key) {
            if (LocalizationManager.Instance == null) {
                return key;
            }

            return LocalizationManager.Instance.GetText(key);
        }

        /// <summary>
        /// Получить локализованный текст по ключу с параметрами
        /// </summary>
        public static string L(string key, params object[] args) {
            string text = L(key);
            return string.Format(text, args);
        }

        /// <summary>
        /// Получить локализованный текст по ключу для конкретного языка
        /// </summary>
        public static string L(string key, string language) {
            if (LocalizationManager.Instance == null) {
                return key;
            }

            return LocalizationManager.Instance.GetText(key, language);
        }

        /// <summary>
        /// Проверить, существует ли ключ локализации
        /// </summary>
        public static bool HasKey(string key) {
            if (LocalizationManager.Instance == null) {
                return false;
            }

            return LocalizationManager.Instance.HasKey(key);
        }

        /// <summary>
        /// Получить список всех ключей локализации
        /// </summary>
        public static List<string> GetAllKeys() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                var keys = new List<string>();
                var guids = AssetDatabase.FindAssets("t:LocalizationData");
                foreach (var guid in guids) {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!path.EndsWith("Localization.asset")) continue;
                    var asset = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);
                    if (asset == null) continue;
                    foreach (var entry in asset.entries) {
                        if (!string.IsNullOrEmpty(entry.key))
                            keys.Add(entry.key);
                    }
                }

                return keys;
            }
#endif
            var keysRt = new List<string>();
            var files = LocalizationManager.Instance?.LocalizationFiles;
            if (files == null) return keysRt;
            foreach (var file in files) {
                if (file == null) continue;
                foreach (var entry in file.entries) {
                    if (!string.IsNullOrEmpty(entry.key))
                        keysRt.Add(entry.key);
                }
            }

            return keysRt;
        }

        /// <summary>
        /// Получить ключи из конкретного файла по суффиксу
        /// </summary>
        public static List<string> GetKeysForFile(string shortName) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                var guids = AssetDatabase.FindAssets("t:LocalizationData");
                foreach (var guid in guids) {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!path.EndsWith("Localization.asset")) continue;
                    var asset = AssetDatabase.LoadAssetAtPath<LocalizationData>(path);
                    if (asset != null && asset.name == shortName + "Localization") {
                        var keys = new List<string>();
                        foreach (var entry in asset.entries) {
                            if (!string.IsNullOrEmpty(entry.key))
                                keys.Add(entry.key);
                        }

                        return keys;
                    }
                }

                return new List<string>();
            }
#endif
            var files = LocalizationManager.Instance?.LocalizationFiles;
            if (files == null) return new List<string>();
            foreach (var file in files) {
                if (file == null) continue;
                if (file.name == shortName + "Localization") {
                    var keys = new List<string>();
                    foreach (var entry in file.entries) {
                        if (!string.IsNullOrEmpty(entry.key))
                            keys.Add(entry.key);
                    }

                    return keys;
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Получить текущий язык
        /// </summary>
        public static string GetCurrentLanguage() {
            return LocalizationManager.Instance?.CurrentLanguage ?? "en";
        }

        /// <summary>
        /// Установить язык
        /// </summary>
        public static void SetLanguage(string language) {
            LocalizationManager.Instance?.SetLanguage(language);
        }

        /// <summary>
        /// Переключиться на русский язык
        /// </summary>
        public static void SetRussian() {
            SetLanguage("ru");
        }

        /// <summary>
        /// Переключиться на английский язык
        /// </summary>
        public static void SetEnglish() {
            SetLanguage("en");
        }

        /// <summary>
        /// Получить количество записей локализации
        /// </summary>
        public static int GetLocalizationCount() {
            int count = 0;
            var files = LocalizationManager.Instance?.LocalizationFiles;
            if (files == null) return 0;
            foreach (var file in files) {
                if (file != null && file.entries != null)
                    count += file.entries.Count;
            }

            return count;
        }

        public static string GetDeviceLanguage() {
            SystemLanguage language = Application.systemLanguage;
            Debug.Log("Используемый язык: " + language.ToString());
            return "en";
            switch (language) {
                case SystemLanguage.Russian:
                    return "ru";
                case SystemLanguage.English:
                    return "en";
                // можно добавить другие языки по необходимости
                default:
                    return "en";
            }
        }
    }
}