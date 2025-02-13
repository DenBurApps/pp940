using System;
using System.Collections.Generic;
using UnityEngine;

namespace TripData
{
    [Serializable]
    public class TripData
    {
        public string Name;
        public DateTime StartDate;
        public DateTime EndDate;
        public string Note;

        public List<PlaceData> PlaceDatas;
        public List<ExpenseData> ExpenseDatas;

        public TripData(string name, DateTime startDate, DateTime endDate, string note)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Note = note;
            PlaceDatas = new List<PlaceData>();
            ExpenseDatas = new List<ExpenseData>();
        }
    }

    [Serializable]
    public class PlaceData
    {
        public string PlaceName;
    }

    [Serializable]
    public class ExpenseData
    {
        public string ExpenseName;
    }
}
