using UnityEngine;
using UnityEngine.UI;

namespace Localization {
    /// <summary>
    /// Пример использования системы локализации
    /// </summary>
    public class LocalizationExample : MonoBehaviour {
        [Header("UI References")]
        [SerializeField]
        private Text welcomeText;

        [SerializeField]
        private Text buttonText;

        [SerializeField]
        private Button languageButton;

        [Header("Localization Keys")]
        [SerializeField]
        private string welcomeKey = "WELCOME_MESSAGE";

        [SerializeField]
        private string buttonKey = "CHANGE_LANGUAGE";

        private void Start() {
            // Подписываемся на изменение языка
            if (LocalizationManager.Instance != null) {
                LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
            }

            // Настраиваем кнопку
            if (languageButton != null) {
                languageButton.onClick.AddListener(ToggleLanguage);
            }

            // Обновляем тексты
            UpdateTexts();
        }

        private void OnDestroy() {
            // Отписываемся от события
            if (LocalizationManager.Instance != null) {
                LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        private void OnLanguageChanged(string newLanguage) {
            UpdateTexts();
        }

        private void UpdateTexts() {
            // Используем утилиты для получения локализованного текста
            if (welcomeText != null) {
                welcomeText.text = LocalizationUtils.L(welcomeKey);
            }

            if (buttonText != null) {
                buttonText.text = LocalizationUtils.L(buttonKey);
            }
        }

        private void ToggleLanguage() {
            string currentLang = LocalizationUtils.GetCurrentLanguage();

            if (currentLang == "ru") {
                LocalizationUtils.SetEnglish();
            } else {
                LocalizationUtils.SetRussian();
            }
        }

        // Пример использования с параметрами
        public void ShowFormattedMessage(string playerName, int score) {
            string message = LocalizationUtils.L("PLAYER_SCORE", playerName, score);
        }

        // Пример получения текста для конкретного языка
        public void ShowAllLanguages(string key) {
            string russian = LocalizationUtils.L(key, "ru");
            string english = LocalizationUtils.L(key, "en");
        }
    }
}