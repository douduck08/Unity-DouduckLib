using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DouduckLib.Localization
{
    [RequireComponent(typeof(Image))]
    public class LocalizationImage : LocalizationComponent
    {
        [Serializable]
        public struct LanguageSpritePair
        {
            public string languageCode;
            public Sprite sprite;
        }

        [SerializeField] List<LanguageSpritePair> _sprites = new();
        [SerializeField] Sprite _defaultSprite;

        Image _imageComponent;

        protected override void OnLanguageChanged()
        {
            _imageComponent ??= GetComponent<Image>();
            if (_imageComponent == null) return;

            var currentLanguage = Localization.Get().GetCurrentLanguageCode();
            var targetSprite = FindSprite(currentLanguage);
            _imageComponent.sprite = targetSprite != null ? targetSprite : _defaultSprite;
        }

        Sprite FindSprite(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) return null;

            foreach (var pair in _sprites)
            {
                if (pair.languageCode == languageCode)
                {
                    return pair.sprite;
                }
            }
            return null;
        }

        public void SetSprites(List<LanguageSpritePair> sprites, Sprite defaultSprite = null)
        {
            _sprites = sprites ?? new List<LanguageSpritePair>();
            if (defaultSprite != null)
            {
                _defaultSprite = defaultSprite;
            }
            OnLanguageChanged();
        }
    }
}
