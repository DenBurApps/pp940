using System;
using System.Collections.Generic;
using System.Linq;
using AddPlaceScreen;
using TripData;
using UnityEngine;
using UnityEngine.UI;

namespace AddExpenseScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AddExpenseScreenController : MonoBehaviour
    {
        [SerializeField] private List<AddInfoPlane> _planes;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _addNewPlaceButton;
        [SerializeField] private GameObject _emptyPlane;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        
        private List<ExpenseData> _expenseDatas;

        public event Action<List<ExpenseData>> OnSaveButtonClick;
        public event Action BackClicked;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            _expenseDatas = new List<ExpenseData>();
           
        }

        private void OnEnable()
        {
            _addNewPlaceButton.onClick.AddListener(EnableNextPlane);
            _saveButton.onClick.AddListener(SavePlaces);
            _backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnDisable()
        {
            _addNewPlaceButton.onClick.RemoveListener(EnableNextPlane);
            _saveButton.onClick.RemoveListener(SavePlaces);
            _backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void Start()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
            _expenseDatas.Clear();
            DisablePlanes();
            ValidateSaveButton();
        }

        private void DisablePlanes()
        {
            foreach (var addPlacePlane in _planes)
            {
                addPlacePlane.Disable();
            }

            ToggleEmptyPlane();
        }
        
        private void OnBackClicked()
        {
            BackClicked?.Invoke();
            _screenVisabilityHandler.DisableScreen();
        }

        private void EnableNextPlane()
        {
            var inactivePlane = _planes.FirstOrDefault(plane => !plane.IsActive);
            if (inactivePlane != null)
            {
                inactivePlane.Enable();
            }

            ValidateSaveButton();
            ToggleEmptyPlane();
        }

        private void ToggleEmptyPlane()
        {
            _emptyPlane.SetActive(_planes.All(p => !p.IsActive));
        }

        private void ValidateSaveButton()
        {
            bool hasAnyActivePlace = _planes.Any(plane =>
                plane.IsActive);

            _saveButton.interactable = hasAnyActivePlace;
        }

        private void SavePlaces()
        {
            _expenseDatas.Clear();

            foreach (var plane in _planes)
            {
                if (plane.IsActive && !string.IsNullOrEmpty(plane.GetPlaceText()))
                {
                    _expenseDatas.Add(new ExpenseData { ExpenseName = plane.GetPlaceText() });
                }
            }

            OnSaveButtonClick?.Invoke(_expenseDatas);
            _screenVisabilityHandler.DisableScreen();
        }
    }
}