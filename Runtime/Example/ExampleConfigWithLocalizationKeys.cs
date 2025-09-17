using Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "ExampleConfigWithLocalizationKeys", menuName = "Localization/ExampleConfigWithLocalizationKeys", order = 0)]
public class ExampleConfigWithLocalizationKeys : ScriptableObject {
    public string JustString;

    [LocalizationKey("ExampleList")]
    public string ExampleKey;
}