using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization {
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Scriptable Objects/Localization/LocalizationData", order = 100)]
    public class LocalizationData : ScriptableObject {
        [SerializeField]
        public string url = "";

        [Serializable]
        public class LocalizationEntry {
            public string key;
            public string ru;
            public string en;

            public LocalizationEntry(string key, string ru, string en) {
                this.key = key;
                this.ru = ru;
                this.en = en;
            }
        }

        [Header("Localization Settings")]
        public string defaultLanguage = "en";

        public List<LocalizationEntry> entries = new List<LocalizationEntry>();

        private Dictionary<string, LocalizationEntry> _entriesCache;

        public void Initialize() {
            _entriesCache = new Dictionary<string, LocalizationEntry>();
            foreach (var entry in entries) {
                if (!string.IsNullOrEmpty(entry.key)) {
                    _entriesCache[entry.key] = entry;
                }
            }
        }

        public string GetText(string key, string language = null) {
            if (_entriesCache == null) {
                Initialize();
            }

            if (string.IsNullOrEmpty(language)) {
                language = defaultLanguage;
            }

            if (_entriesCache.TryGetValue(key, out var entry)) {
                return language switch {
                    "ru" => entry.ru,
                    "en" => entry.en,
                    _ => entry.en // fallback to English
                };
            }

            Debug.LogWarning($"Localization key not found: {key}");
            return key;
        }

        public bool HasKey(string key) {
            if (_entriesCache == null) {
                Initialize();
            }

            return _entriesCache.ContainsKey(key);
        }

        public void ClearCache() {
            _entriesCache?.Clear();
        }
    }
}