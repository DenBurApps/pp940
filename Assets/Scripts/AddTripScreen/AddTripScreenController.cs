using System;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using MainScreen;
using TMPro;
using TripData;
using UnityEngine;
using UnityEngine.UI;

namespace AddTripScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AddTripScreenController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_Text _startDateText;
        [SerializeField] private TMP_Text _endDateText;
        [SerializeField] private Button _startDateButton;
        [SerializeField] private Button _endDateButton;
        [SerializeField] private TMP_InputField _noteInput;
        [SerializeField] private AddedPlacesController _addedPlacesController;
        [SerializeField] private AddedExpensesController _addedExpensesController;
        [SerializeField] private AddExpenseScreen.AddExpenseScreenController _addExpenseScreen;
        [SerializeField] private AddPlaceScreen.AddPlaceScreenController _addPlaceScreen;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _addPlaceButton;
        [SerializeField] private Button _addExpanseButton;
        [SerializeField] private DatePickerSettings _startDatePicker;
        [SerializeField] private DatePickerSettings _endDatePicker;
        [SerializeField] private GameObject _startDatePickerPlane;
        [SerializeField] private GameObject _endDatePickerPlane;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private bool _startDateSelected;
        private DateTime _startDate;
        private DateTime _endDate;
        private TripPlane _currentTripPlane;
        private bool _isEditMode;
        private bool _isStartDatePickerActive = false;
        private bool _isEndDatePickerActive = false;
        private bool _isProcessingDateSelection = false;

        public event Action<TripData.TripData> TripDataSaved;
        public event Action<TripPlane> TripDeleted;
        public event Action TripDataUpdated;
        public event Action BackClicked;
        public event Action BackFromEdit;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            InitializeDefaultValues();
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackButtonClicked);
            _saveButton.onClick.AddListener(OnSaveButtonClicked);
            _startDateButton.onClick.AddListener(OnStartDateButtonClicked);
            _endDateButton.onClick.AddListener(OnEndDateButtonClicked);

            _nameInput.onValueChanged.AddListener(_ => ToggleSaveButton());
            _noteInput.onValueChanged.AddListener(_ => ToggleSaveButton());

            _startDatePicker.Content.OnSelectionChanged.AddListener(SetStartDate);
            _endDatePicker.Content.OnSelectionChanged.AddListener(SetEndDate);
            
            _addPlaceScreen.OnSaveButtonClick += PlacesAdded;
            _addPlaceScreen.BackClicked += EnableScreen;
            _addExpenseScreen.OnSaveButtonClick += ExpansesAdded;
            _addExpenseScreen.BackClicked += EnableScreen;

            _addExpanseButton.onClick.AddListener(OnAddExpanseClicked);
            _addPlaceButton.onClick.AddListener(OnAddPlaceClicked);
            _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
            _saveButton.onClick.RemoveListener(OnSaveButtonClicked);
            _startDateButton.onClick.RemoveListener(OnStartDateButtonClicked);
            _endDateButton.onClick.RemoveListener(OnEndDateButtonClicked);

            _nameInput.onValueChanged.RemoveListener(_ => ToggleSaveButton());
            _noteInput.onValueChanged.RemoveListener(_ => ToggleSaveButton());

            _startDatePicker.Content.OnSelectionChanged.RemoveListener(SetStartDate);
            _endDatePicker.Content.OnSelectionChanged.RemoveListener(SetEndDate);

            _addPlaceScreen.OnSaveButtonClick -= PlacesAdded;
            _addPlaceScreen.BackClicked -= EnableScreen;
            _addExpenseScreen.OnSaveButtonClick -= ExpansesAdded;
            _addExpenseScreen.BackClicked -= EnableScreen;

            _addExpanseButton.onClick.RemoveListener(OnAddExpanseClicked);
            _addPlaceButton.onClick.RemoveListener(OnAddPlaceClicked);
            _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
        }

        private void Start()
        {
            DisableScreen();
        }

        private void InitializeDefaultValues()
        {
            _startDate = DateTime.Today;
            _endDate = DateTime.Today;
            _startDateText.text = _startDate.ToString("dd MMM");
            _endDateText.text = _endDate.ToString("dd MMM");
            _saveButton.interactable = false;
            _startDatePickerPlane.SetActive(false);
            _endDatePickerPlane.SetActive(false);
            _isEditMode = false;
            _currentTripPlane = null;
        }

        public void EnableScreen(TripPlane tripDataToEdit = null)
        {
            _screenVisabilityHandler.EnableScreen();

            if (tripDataToEdit == null) return;
            _isEditMode = true;
            _currentTripPlane = tripDataToEdit;
            PopulateFieldsWithExistingData(tripDataToEdit.TripData);
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();

            _isEditMode = false;
            _currentTripPlane = null;
        }

        private void PopulateFieldsWithExistingData(TripData.TripData tripData)
        {
            _nameInput.text = tripData.Name;
            _noteInput.text = tripData.Note;
            _startDate = tripData.StartDate;
            _endDate = tripData.EndDate;
            _startDateText.text = _startDate.ToString("dd MMM");
            _endDateText.text = _endDate.ToString("dd MMM");

            _addedPlacesController.AssignPlaces(tripData.PlaceDatas);
            _addedExpensesController.AssignPlaces(tripData.ExpenseDatas);

            ToggleSaveButton();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void ResetValues()
        {
            _addedExpensesController.ResetData();
            _addedPlacesController.ResetData();

            _nameInput.text = string.Empty;
            _noteInput.text = string.Empty;
            _startDate = DateTime.Today;
            _endDate = DateTime.Today;
            _startDateText.text = _startDate.ToString("dd MMM");
            _endDateText.text = _endDate.ToString("dd MMM");
            _saveButton.interactable = false;
            _isEditMode = false;
            _currentTripPlane = null;
            _isStartDatePickerActive = false;
            _isEndDatePickerActive = false;
        }

        private void ToggleSaveButton()
        {
            _saveButton.interactable = !string.IsNullOrEmpty(_nameInput.text) &&
                                       !string.IsNullOrEmpty(_noteInput.text) &&
                                       _startDate != _endDate &&
                                       _endDate > _startDate;
        }

        private void OnSaveButtonClicked()
        {
            var tripData = new TripData.TripData
            (
                _nameInput.text,
                _startDate,
                _endDate,
                _noteInput.text,
                new List<PlaceData>(_addedPlacesController.PlaceDatas),
                new List<ExpenseData>(_addedExpensesController.ExpenseDatas)
            );

            if (_isEditMode && _currentTripPlane != null)
            {
                _currentTripPlane.UpdateTripData(tripData);
                TripDataUpdated?.Invoke();
            }
            else
            {
                TripDataSaved?.Invoke(tripData);
            }

            DisableScreen();
            ResetValues();
        }

        public void ResetDatePickers()
        {
            _isStartDatePickerActive = false;
            _isEndDatePickerActive = false;
            _startDatePickerPlane.SetActive(false);
            _endDatePickerPlane.SetActive(false);
        }

        private void OnStartDateButtonClicked()
        {
            if (_isStartDatePickerActive || _isEndDatePickerActive)
                return;

            _startDatePicker.Content.Selection.SelectOne(_startDate);
            
            _isProcessingDateSelection = false;
            _isStartDatePickerActive = true;
            _startDatePickerPlane.SetActive(true);
        }

        private void OnEndDateButtonClicked()
        {
            if (_isStartDatePickerActive || _isEndDatePickerActive) 
                return;
            
            _endDatePicker.Content.Selection.SelectOne(_endDate);

            _isProcessingDateSelection = false;
            _isEndDatePickerActive = true;
            _endDatePickerPlane.SetActive(true);
        }
        
        private void SetStartDate()
        {
            if (_isProcessingDateSelection) return;
            _isProcessingDateSelection = true;

            var selection = _startDatePicker.Content.Selection;
            var selectedDate = selection.GetItem(0);

            _startDate = selectedDate;
            _startDateText.text = _startDate.ToString("dd MMM");

            _startDatePickerPlane.SetActive(false);
            _isStartDatePickerActive = false;
            ToggleSaveButton();
            
            _isProcessingDateSelection = false;
        }
        
        private void SetEndDate()
        {
            if (_isProcessingDateSelection) return;
            _isProcessingDateSelection = true;

            var selection = _endDatePicker.Content.Selection;
            var selectedDate = selection.GetItem(0);

            _endDate = selectedDate;
            _endDateText.text = _endDate.ToString("dd MMM");

            _endDatePickerPlane.SetActive(false);
            _isEndDatePickerActive = false;
            ToggleSaveButton();
            
            _isProcessingDateSelection = false;
        }
        
        private void OnAddPlaceClicked()
        {
            _addPlaceScreen.EnableScreen();
            DisableScreen();
        }

        private void PlacesAdded(List<PlaceData> placeDatas)
        {
            EnableScreen();
            _addedPlacesController.AssignPlaces(placeDatas);
            ToggleSaveButton();
        }

        private void OnAddExpanseClicked()
        {
            _addExpenseScreen.EnableScreen();
            DisableScreen();
        }

        private void ExpansesAdded(List<ExpenseData> expenseDatas)
        {
            EnableScreen();
            _addedExpensesController.AssignPlaces(expenseDatas);
            ToggleSaveButton();
        }
        

        private void OnBackButtonClicked()
        {
            if (_isEditMode)
            {
                BackFromEdit?.Invoke();
            }
            else
            {
                BackClicked?.Invoke();
            }

            DisableScreen();
            ResetValues();
        }

        private void OnDeleteButtonClicked()
        {
            if (_isEditMode)
            {
                TripDeleted?.Invoke(_currentTripPlane);
                DisableScreen();
                return;
            }

            BackClicked?.Invoke();
            DisableScreen();
        }
    }
}