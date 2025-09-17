using UnityEngine;

namespace Localization {
    /// <summary>
    /// Базовый класс для конфигов, которые поддерживают локализацию
    /// </summary>
    public abstract class LocalizedConfig : ScriptableObject {
        [Header("Localization")]
        [SerializeField]
        protected string localizationKey;

        /// <summary>
        /// Получить локализованное название
        /// </summary>
        public virtual string GetLocalizedName() {
            if (string.IsNullOrEmpty(localizationKey)) {
                return name;
            }

            return LocalizationUtils.L(localizationKey);
        }

        /// <summary>
        /// Получить локализованное название для конкретного языка
        /// </summary>
        public virtual string GetLocalizedName(string language) {
            if (string.IsNullOrEmpty(localizationKey)) {
                return name;
            }

            return LocalizationUtils.L(localizationKey, language);
        }

        /// <summary>
        /// Установить ключ локализации
        /// </summary>
        public virtual void SetLocalizationKey(string key) {
            localizationKey = key;
        }

        /// <summary>
        /// Получить ключ локализации
        /// </summary>
        public virtual string GetLocalizationKey() {
            return localizationKey;
        }

        /// <summary>
        /// Проверить, есть ли ключ локализации
        /// </summary>
        public virtual bool HasLocalizationKey() {
            return !string.IsNullOrEmpty(localizationKey);
        }
    }

    /// <summary>
    /// Расширенный класс для конфигов с описанием
    /// </summary>
    public abstract class LocalizedConfigWithDescription : LocalizedConfig {
        [Header("Description Localization")]
        [SerializeField]
        protected string descriptionKey;

        /// <summary>
        /// Получить локализованное описание
        /// </summary>
        public virtual string GetLocalizedDescription() {
            if (string.IsNullOrEmpty(descriptionKey)) {
                return "";
            }

            return LocalizationUtils.L(descriptionKey);
        }

        /// <summary>
        /// Получить локализованное описание для конкретного языка
        /// </summary>
        public virtual string GetLocalizedDescription(string language) {
            if (string.IsNullOrEmpty(descriptionKey)) {
                return "";
            }

            return LocalizationUtils.L(descriptionKey, language);
        }

        /// <summary>
        /// Установить ключ описания
        /// </summary>
        public virtual void SetDescriptionKey(string key) {
            descriptionKey = key;
        }

        /// <summary>
        /// Получить ключ описания
        /// </summary>
        public virtual string GetDescriptionKey() {
            return descriptionKey;
        }
    }
}