using UnityEngine;

namespace FlavorfulStory.LocalizationSystem
{
    public class UILocalizer : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                LocalizationService.SetLanguageStatic(LocalizationService.CurrentLanguage == "RU" ? "EN" : "RU");
        }
    }
}