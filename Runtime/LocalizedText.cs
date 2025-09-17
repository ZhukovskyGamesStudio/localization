using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Localization {
    public class LocalizedText : MonoBehaviour {
        [Header("Localization Settings")]
        [SerializeField]
        private string localizationKey;

        [SerializeField]
        private bool updateOnStart = true;

        private Text _textComponent;
        private TextMeshProUGUI _tmpComponent;
        private TextMeshPro _tmpWorldComponent;

        private void Awake() {
            GetTextComponents();
        }

        private void Start() {
            if (updateOnStart) {
                UpdateText();
            }

            if (LocalizationManager.Instance != null) {
                LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
            }
        }

        private void OnDestroy() {
            if (LocalizationManager.Instance != null) {
                LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        private void GetTextComponents() {
            _textComponent = GetComponent<Text>();
            _tmpComponent = GetComponent<TextMeshProUGUI>();
            _tmpWorldComponent = GetComponent<TextMeshPro>();
        }

        private void OnLanguageChanged(string newLanguage) {
            UpdateText();
        }

        public void UpdateText() {
            if (string.IsNullOrEmpty(localizationKey)) {
                Debug.LogWarning($"LocalizationKey не установлен для {gameObject.name}");
                return;
            }

            string localizedText = LocalizationManager.Instance?.GetText(localizationKey) ?? localizationKey;

            if (_textComponent != null) {
                _textComponent.text = localizedText;
            } else if (_tmpComponent != null) {
                _tmpComponent.text = localizedText;
            } else if (_tmpWorldComponent != null) {
                _tmpWorldComponent.text = localizedText;
            }
        }

        public void SetLocalizationKey(string key) {
            localizationKey = key;
            UpdateText();
        }

        public string GetLocalizationKey() {
            return localizationKey;
        }
    }
}