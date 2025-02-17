using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace TripData
{
    public static class TripDataService
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "TripData.json");

        public static void Save(List<TripData> tripDatas)
        {
            TripDataListWrapper wrapper = new TripDataListWrapper(tripDatas);
            string json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
            File.WriteAllText(SavePath, json);
        }

        public static List<TripData> Load()
        {
            if (!File.Exists(SavePath))
            {
                return new List<TripData>();
            }

            var json = File.ReadAllText(SavePath);
            var wrapper = JsonConvert.DeserializeObject<TripDataListWrapper>(json);
            return wrapper.TpetDataList;
        }

        [System.Serializable]
        private class TripDataListWrapper
        {
            public List<TripData> TpetDataList;

            public TripDataListWrapper(List<TripData> tpetDataList)
            {
                TpetDataList = tpetDataList;
            }
        }
    }
}