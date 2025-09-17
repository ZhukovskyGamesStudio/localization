using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace Localization {
    public abstract class LocalizationManager : MonoBehaviour {
        public static LocalizationManager Instance;

        [Header("Localization Settings")]
        [SerializeField]
        private LocalizationData[] _localizationFiles;

        public event Action<string> OnLanguageChanged;

        public string CurrentLanguage {
            get => LoadCurrentLanguage();
            private set {
                if (LoadCurrentLanguage() != value) {
                    SaveCurrentLanguage(value);
                    OnLanguageChanged?.Invoke(value);
                }
            }
        }

        public abstract string LoadCurrentLanguage();
        public abstract void SaveCurrentLanguage(string value);
        
        public LocalizationData[] LocalizationFiles => _localizationFiles;

        private void Awake() {
            Instance = this;
            InitializeLocalization();
        }

        private void InitializeLocalization() {
            if (_localizationFiles != null && _localizationFiles.Length > 0) {
                foreach (var file in _localizationFiles) {
                    if (file != null) file.Initialize();
                }
            } else {
                Debug.LogError("LocalizationFiles не назначены в LocalizationManager!");
            }
        }

        public string GetText(string key) {
            if (_localizationFiles == null || _localizationFiles.Length == 0) {
                Debug.LogError("LocalizationFiles не назначены!");
                return key;
            }

            foreach (var file in _localizationFiles) {
                if (file != null && file.HasKey(key))
                    return file.GetText(key, CurrentLanguage);
            }

            Debug.LogWarning($"Localization key not found: {key}");
            return key;
        }

        public string GetText(string key, string language) {
            if (_localizationFiles == null || _localizationFiles.Length == 0) {
                Debug.LogError("LocalizationFiles не назначены!");
                return key;
            }

            foreach (var file in _localizationFiles) {
                if (file != null && file.HasKey(key))
                    return file.GetText(key, language);
            }

            Debug.LogWarning($"Localization key not found: {key}");
            return key;
        }

        public bool HasKey(string key) {
            if (_localizationFiles == null) return false;
            foreach (var file in _localizationFiles) {
                if (file != null && file.HasKey(key)) return true;
            }

            return false;
        }

        public void SetLanguage(string language) {
            CurrentLanguage = language;
        }

        public void SetLanguageToRussian() {
            SetLanguage("ru");
        }

        public void SetLanguageToEnglish() {
            SetLanguage("en");
        }

        public void ChangeLanguageToNext() {
            int cur = Languages.IndexOf(CurrentLanguage);
            cur++;
            cur %= Languages.Count;
            SetLanguage(Languages[cur]);
        }

        protected readonly List<string> Languages = new List<string>() {
            "en",
            "ru"
        };
    }
}