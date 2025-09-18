using UnityEngine;

namespace ZG_Localization {
    public class LocalizationKeyAttribute : PropertyAttribute {
        public string fileName;

        public LocalizationKeyAttribute(string fileName) {
            this.fileName = fileName;
        }
    }
}