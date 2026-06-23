using UnityEngine;

namespace DouduckLib.Localization
{
    public abstract class LocalizationComponent : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            var localization = Localization.Get();
            if (localization != null)
            {
                localization.OnLanguageChanged += OnLanguageChanged;
            }
            OnLanguageChanged();
        }

        protected virtual void OnDisable()
        {
            var localization = Localization.Get();
            if (localization != null)
            {
                localization.OnLanguageChanged -= OnLanguageChanged;
            }
        }

        protected abstract void OnLanguageChanged();
    }
}
