using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using AddTripScreen;
using OpenTripScreen;
using TripData;
using UnityEngine;
using UnityEngine.UI;

namespace MainScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class MainScreenController : MonoBehaviour
    {
        [SerializeField] private List<TripPlane> _planes;
        [SerializeField] private AddTripScreen.AddTripScreenController _addTripScreenController;
        [SerializeField] private Button[] _addTripButtons;
        [SerializeField] private GameObject _emptyDataObject;
        [SerializeField] private OpenTripScreen.OpenTripScreenController _openTripScreenController;
        [SerializeField] private AddTripScreenController _editTripScreen;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        private List<TripData.TripData> _activeTrips = new List<TripData.TripData>();

        public IReadOnlyCollection<TripPlane> PetPlanes => _planes;

        private void Awake()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _addTripScreenController.TripDataSaved += OnTripDataSaved;
            _addTripScreenController.BackClicked += EnableScreen;
            _editTripScreen.TripDataUpdated += OnTripDataUpdated;
            _editTripScreen.TripDeleted += OnTripDeleted;
            _editTripScreen.BackClicked += EnableScreen;
            _openTripScreenController.BackClicked += EnableScreen;

            foreach (var addTripButton in _addTripButtons)
            {
                addTripButton.onClick.AddListener(OpenAddTripScreen);
            }

            foreach (var tripPlane in _planes)
            {
                tripPlane.Clicked += OpenPlane;
            }
        }

        private void OnDisable()
        {
            _addTripScreenController.TripDataSaved -= OnTripDataSaved;
            _addTripScreenController.BackClicked -= EnableScreen;
            _editTripScreen.TripDataUpdated -= OnTripDataUpdated;
            _editTripScreen.TripDeleted -= OnTripDeleted;
            _editTripScreen.BackClicked -= EnableScreen;
            _openTripScreenController.BackClicked -= EnableScreen;

            foreach (var addTripButton in _addTripButtons)
            {
                addTripButton.onClick.RemoveListener(OpenAddTripScreen);
            }

            foreach (var tripPlane in _planes)
            {
                tripPlane.Clicked -= OpenPlane;
            }

            SaveAllData();
        }

        private void Start()
        {
            EnableScreen();
            DisableAllPlanes();
            
            var savedData = TripDataService.Load();

            foreach (var data in savedData)
            {
                EnablePlane(data);
            }

            ToggleEmptyDataObject();
        }

        private void OnTripDataSaved(TripData.TripData data)
        {
            EnablePlane(data);
            SaveAllData();
        }

        private void OnTripDataUpdated()
        {
            EnableScreen();
            SaveAllData();
        }

        private void OnTripDeleted(TripPlane plane)
        {
            DeletePlane(plane);
            SaveAllData();
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void OpenAddTripScreen()
        {
            _addTripScreenController.EnableScreen();
            DisableScreen();
        }

        private void EnablePlane(TripData.TripData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            EnableScreen();

            var availablePlane = _planes.FirstOrDefault(p => !p.IsActive);
            if (availablePlane == null)
                return;

            availablePlane.EnablePlane(data);
            ToggleEmptyDataObject();
        }

        private void DeletePlane(TripPlane plane)
        {
            if (plane == null)
                throw new ArgumentNullException(nameof(plane));

            EnableScreen();
            plane.Disable();
            ToggleEmptyDataObject();
        }

        private void DisableAllPlanes()
        {
            foreach (var tripPlane in _planes)
            {
                tripPlane.Disable();
            }

            ToggleEmptyDataObject();
        }

        private void OpenPlane(TripPlane plane)
        {
            if (plane == null)
                throw new ArgumentNullException(nameof(plane));

            _openTripScreenController.Enable(plane);
            DisableScreen();
        }

        private void ToggleEmptyDataObject()
        {
            _emptyDataObject.SetActive(!_planes.Any(p => p.IsActive));
        }
        
        private void SaveAllData()
        {
            var dataList = new List<TripData.TripData>();

            foreach (var plane in _planes)
            {
                if (plane.TripData != null)
                {
                    dataList.Add(plane.TripData);
                }
            }

            TripDataService.Save(dataList);
        }
    }
}