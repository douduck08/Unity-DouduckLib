using UnityEngine;
using TMPro;

namespace DouduckLib.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizationText : LocalizationComponent
    {
        [SerializeField] LocalizedString _localizedString;

        TMP_Text _textComponent;

        protected override void OnLanguageChanged()
        {
            _textComponent ??= GetComponent<TMP_Text>();
            if (_textComponent != null)
            {
                _textComponent.text = _localizedString.ToString();
            }
        }

        public void SetLocalizedString(LocalizedString localizedString)
        {
            _localizedString = localizedString;
            OnLanguageChanged();
        }

        public void SetText(string text)
        {
            _localizedString = new LocalizedString(text, false);
            OnLanguageChanged();
        }

        public void SetLocalizationKey(string ns, string key)
        {
            SetLocalizationKey($"{ns}.{key}");
        }

        public void SetLocalizationKey(string fullKey)
        {
            _localizedString = new LocalizedString(fullKey, true);
            OnLanguageChanged();
        }
    }
}
