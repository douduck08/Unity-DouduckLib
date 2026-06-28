using System;
using System.IO;
using UnityEngine;

namespace DouduckLib.Save
{
    [System.Serializable]
    public class SlotSaveController<TSaveData> : BaseSaveController<TSaveData> where TSaveData : class, new()
    {
        [SerializeField] int _maxSlotNumber;
        [SerializeField] string _fileFormat;

        public int MaxSlotNumber => _maxSlotNumber;

        public SlotSaveController() : base()
        {
        }

        public SlotSaveController(string fileFormat, int maxSlotNumber)
            : base()
        {
            _fileFormat = fileFormat;
            _maxSlotNumber = maxSlotNumber;
        }

        public string GetSaveFileName(int slotIndex)
        {
            return GetFileName(slotIndex);
        }

        public bool HasSlot(int slotIndex)
        {
            var fileName = GetFileName(slotIndex);
            var filePath = Path.Combine(SaveFileHelper.DefaultSaveDir, fileName);
            return File.Exists(filePath);
        }

        public bool LoadSlot(int slotIndex)
        {
            var fileName = GetFileName(slotIndex);
            return Load(fileName);
        }

        public void SaveSlot(int slotIndex)
        {
            var fileName = GetFileName(slotIndex);
            Save(fileName);
        }

        public void DeleteSlot(int slotIndex)
        {
            var fileName = GetFileName(slotIndex);
            Delete(fileName);
        }

        protected virtual string GetFileName(int slotIndex)
        {
            return string.Format(_fileFormat, slotIndex);
        }
    }
}
