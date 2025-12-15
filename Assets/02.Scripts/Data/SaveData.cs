using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class SaveData
    {
        public int Gold;
        public List<MercenarySaveData> Mercenaries = new List<MercenarySaveData>();
        public string[] PartyIds = new string[4];
    }

    [Serializable]
    public class MercenarySaveData
    {
        public string Id;
        public string UnitDataId;
        public string CustomName;
        public int Level;

        public MercenarySaveData(MercenaryData merc)
        {
            Id = merc.Id;
            UnitDataId = merc.UnitDataId;
            CustomName = merc.CustomName;
            Level = merc.Level;
        }

        public MercenaryData ToMercenaryData()
        {
            return new MercenaryData(this);
        }
    }
}