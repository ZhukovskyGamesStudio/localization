using System.Linq;
using UnityEngine;

namespace Localization {
    public class ExamplePlayerPrefsLocalizationManager : LocalizationManager {
        public override string LoadCurrentLanguage() {
            return PlayerPrefs.GetString("language", Languages.First());
        }

        public override void SaveCurrentLanguage(string value) {
            PlayerPrefs.SetString("language", value);
        }
    }
}