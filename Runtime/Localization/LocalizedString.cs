using System;
using UnityEngine;

namespace DouduckLib.Localization
{
    [System.Serializable]
    public struct LocalizedString
    {
        [SerializeField] bool _isLocalized;
        [SerializeField] string _fullKeyOrText;

        public LocalizedString(string fullKeyOrText, bool isLocalized = true)
        {
            _isLocalized = isLocalized;
            _fullKeyOrText = fullKeyOrText;
        }

        public readonly bool IsEmpty => string.IsNullOrEmpty(_fullKeyOrText);
        public readonly bool IsLocalized => _isLocalized;
        public readonly string FullKey => _isLocalized ? _fullKeyOrText : string.Empty;
        public readonly string RawText => !_isLocalized ? _fullKeyOrText : string.Empty;

        public void Reset() => _fullKeyOrText = "";
        public readonly bool HasValidKey() => !string.IsNullOrEmpty(_fullKeyOrText) && Localization.Get().IsValidKey(_fullKeyOrText);

        [Obsolete("Use ToString() instead.")]
        public readonly string AsString() => ToString();

        public override readonly string ToString() => _isLocalized ? Localization.Get().GetString(_fullKeyOrText) : _fullKeyOrText;

        public readonly string GetInfo() => $"LocalizedString: key = {_fullKeyOrText}, content = {ToString()}";

        public static LocalizedString Empty => new("", false);
    }
}
