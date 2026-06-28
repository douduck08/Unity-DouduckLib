using System;
using System.IO;
using UnityEngine;
using DouduckLib;

namespace DouduckLib.Save
{
    [System.Serializable]
    public abstract class BaseSaveController<TSaveData> where TSaveData : class, new()
    {
        [SerializeField, ReadOnly] protected TSaveData _data;

        public TSaveData Data => _data;

        public event Action<TSaveData> OnPreSave;
        public event Action<TSaveData> OnPostLoad;

        public BaseSaveController()
        {
            ResetToDefault();
        }

        public bool Load(string fileName)
        {
            if (SaveFileHelper.Load<TSaveData>(fileName, out var loadedData))
            {
                _data = loadedData;
                OnPostLoad?.Invoke(Data);
                return true;
            }

            ResetToDefault();
            return false;
        }

        public void Save(string fileName)
        {
            OnPreSave?.Invoke(Data);
            SaveFileHelper.Save(Data, fileName);
        }

        public void Delete(string fileName)
        {
            var filePath = Path.Combine(SaveFileHelper.DefaultSaveDir, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            ResetToDefault();
        }

        public void ResetToDefault()
        {
            _data = GetDefaultData();
            OnPostLoad?.Invoke(Data);
        }

        protected virtual TSaveData GetDefaultData()
        {
            return new TSaveData();
        }
    }

    [System.Serializable]
    public class SaveController<TSaveData> : BaseSaveController<TSaveData> where TSaveData : class, new()
    {
        [SerializeField] string _fileName;

        public string FileName => _fileName;

        public SaveController() : base()
        {
        }

        public SaveController(string fileName) : base()
        {
            _fileName = fileName;
            ResetToDefault();
        }

        public bool HasSaveFile()
        {
            var filePath = Path.Combine(SaveFileHelper.DefaultSaveDir, _fileName);
            return File.Exists(filePath);
        }

        public bool Load() => Load(_fileName);
        public void Save() => Save(_fileName);
        public void Delete() => Delete(_fileName);
    }
}
