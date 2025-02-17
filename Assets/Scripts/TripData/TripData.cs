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

        public TripData(string name, DateTime startDate, DateTime endDate, string note, List<PlaceData> placeDatas, List<ExpenseData> expenseDatas)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Note = note;
            PlaceDatas = placeDatas;
            ExpenseDatas = expenseDatas;
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
