using System;
using System.Collections.Generic;
using System.Linq;
using TripData;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlaceScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class AddPlaceScreenController : MonoBehaviour
    {
        [SerializeField] private List<AddPlacePlane> _planes;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _addNewPlaceButton;

        private List<PlaceData> _placeDatas;
        
        public event Action<List<PlaceData>> OnSaveButtonClick;

        private void Awake()
        {
            _placeDatas = new List<PlaceData>();
            _addNewPlaceButton.onClick.AddListener(EnableNextPlane);
            _saveButton.onClick.AddListener(SavePlaces);
        }

        private void OnDestroy()
        {
            _addNewPlaceButton.onClick.RemoveListener(EnableNextPlane);
            _saveButton.onClick.RemoveListener(SavePlaces);
        }

        public void EnableScreen()
        {
            _placeDatas.Clear();
            DisablePlanes();
            ValidateSaveButton();
        }

        private void DisablePlanes()
        {
            _planes[0].Enable();
            
            for (var i = 1; i < _planes.Count; i++)
            {
                _planes[i].Disable();
            }
        }

        private void EnableNextPlane()
        {
            var inactivePlane = _planes.FirstOrDefault(plane => !plane.IsActive);
            if (inactivePlane != null)
            {
                inactivePlane.Enable();
            }
            
            ValidateSaveButton();
        }

        private void ValidateSaveButton()
        {
            bool hasAnyActivePlace = _planes.Any(plane => 
                plane.IsActive && !string.IsNullOrEmpty(plane.GetPlaceText()));
            
            _saveButton.interactable = hasAnyActivePlace;
        }

        private void SavePlaces()
        {
            _placeDatas.Clear();
            
            foreach (var plane in _planes)
            {
                if (plane.IsActive && !string.IsNullOrEmpty(plane.GetPlaceText()))
                {
                    _placeDatas.Add(new PlaceData { PlaceName = plane.GetPlaceText() });
                }
            }
            
            OnSaveButtonClick?.Invoke(_placeDatas);
        }
    }
}