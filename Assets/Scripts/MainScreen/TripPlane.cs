using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TripData;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    public class TripPlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _dateText;
        [SerializeField] private TMP_Text _placesText;
        [SerializeField] private TMP_Text _expensesText;
        [SerializeField] private Button _openButton;

        public event Action<TripPlane> Clicked; 
        
        public TripData.TripData TripData { get; private set; }
        public bool IsActive { get; private set; }

        private void OnEnable()
        {
            _openButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(OnButtonClicked);
        }

        public void EnablePlane(TripData.TripData data)
        {
            TripData = data ?? throw new ArgumentNullException(nameof(data));

            gameObject.SetActive(true);
            IsActive = true;
            
            UpdateTripData(TripData);
        }

        public void UpdateTripData(TripData.TripData data)
        {
            TripData = new TripData.TripData(
                data.Name,
                data.StartDate,
                data.EndDate,
                data.Note,
                new List<PlaceData>(data.PlaceDatas),
                new List<ExpenseData>(data.ExpenseDatas)
            );
    
            _nameText.text = TripData.Name;
            _dateText.text = $"{TripData.StartDate:ddd dd} - {TripData.EndDate:ddd dd}";
            _placesText.text = TripData.PlaceDatas.Count.ToString("00");
            _expensesText.text = TripData.ExpenseDatas.Count.ToString("00");
        }

        public void Disable()
        {
            if (TripData != null)
            {
                TripData = null;
            }
            
            gameObject.SetActive(false);
            IsActive = false;
        }

        private void OnButtonClicked()
        {
            Clicked?.Invoke(this);
        }
    }
}
