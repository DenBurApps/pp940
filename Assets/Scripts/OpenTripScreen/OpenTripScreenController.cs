using System;
using System.Collections.Generic;
using System.Linq;
using AddTripScreen;
using MainScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenTripScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class OpenTripScreenController : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _editButton;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _periodText;
        [SerializeField] private TMP_Text _noteText;
        [SerializeField] private TMP_Text _placesCountText;
        [SerializeField] private TMP_Text _expansesCountText;
        [SerializeField] private List<AddedInfoPlane> _placesPlanes;
        [SerializeField] private List<AddedInfoPlane> _expansesPlanes;
        [SerializeField] private AddTripScreenController _editTripScreen;

        private ScreenVisabilityHandler _screenVisabilityHandler;

        private TripPlane _currentPlane;

        public event Action BackClicked;
        
        public void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackClicked);
            _editButton.onClick.AddListener(OnEditClicked);

            _editTripScreen.BackFromEdit += _screenVisabilityHandler.EnableScreen;
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackClicked);
            _editButton.onClick.RemoveListener(OnEditClicked);
            
            _editTripScreen.BackFromEdit -= _screenVisabilityHandler.EnableScreen;
        }

        private void Start()
        {
            ClearAllData();
            DisableScreen();
        }

        public void Enable(TripPlane plane)
        {
            _currentPlane = plane ?? throw new ArgumentNullException(nameof(plane));
            
            DisableAllPlanes();
            _screenVisabilityHandler.EnableScreen();

            _nameText.text = _currentPlane.TripData.Name;
            _periodText.text = $"{_currentPlane.TripData.StartDate:MMM dd} - {_currentPlane.TripData.EndDate:MMM dd}";
            _noteText.text = _currentPlane.TripData.Note;

            _placesCountText.text = _currentPlane.TripData.PlaceDatas.Count.ToString();
            _expansesCountText.text = _currentPlane.TripData.ExpenseDatas.Count.ToString();
            
            
            if (_currentPlane.TripData.PlaceDatas.Count > 0)
            {
                foreach (var tripDataPlaceData in _currentPlane.TripData.PlaceDatas)
                {
                    _placesPlanes.FirstOrDefault(p => !p.IsActive)?.Enable(tripDataPlaceData.PlaceName);
                }
            }
            
            if (_currentPlane.TripData.ExpenseDatas.Count > 0)
            {
                foreach (var expenseData in _currentPlane.TripData.ExpenseDatas)
                {
                    _expansesPlanes.FirstOrDefault(p => !p.IsActive)?.Enable(expenseData.ExpenseName);
                }
            }
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void DisableAllPlanes()
        {
            foreach (var addedInfoPlane in _placesPlanes)
            {
                addedInfoPlane.Disable();
            }

            foreach (var addedInfoPlane in _expansesPlanes)
            {
                addedInfoPlane.Disable();
            }
        }
        
        private void ClearAllData()
        {
            DisableAllPlanes();
            _nameText.text = string.Empty;
            _periodText.text = string.Empty;
            _noteText.text = string.Empty;
            _placesCountText.text = "0";
            _expansesCountText.text = "0";
        }

        private void OnBackClicked()
        {
            _screenVisabilityHandler.DisableScreen();
            BackClicked?.Invoke();
        }

        private void OnEditClicked()
        {
            _screenVisabilityHandler.DisableScreen();
            _editTripScreen.EnableScreen(_currentPlane);
        }
    }
}
