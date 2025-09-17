using UnityEngine;

namespace Localization {
    public class LocalizationKeyAttribute : PropertyAttribute {
        public string fileName;

        public LocalizationKeyAttribute(string fileName) {
            this.fileName = fileName;
        }
    }
}