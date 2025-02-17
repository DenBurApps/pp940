using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CheckListScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class PackingList : MonoBehaviour
    {
        [SerializeField] private List<ChecklistItem> _packingList;
        [SerializeField] private Button _addNewItem;
        [SerializeField] private Button _clearAll;
        [SerializeField] private GameObject _emptyListObject;
        
        private ScreenVisabilityHandler _screenVisabilityHandler;
        
        public IReadOnlyCollection<ChecklistItem> ChecklistItems => _packingList;

        public event Action OnSave;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
            DisableAllPlanes();
        }
        
        private void OnEnable()
        {
            _clearAll.onClick.AddListener(ClearAllPlanes);
            _addNewItem.onClick.AddListener(EnablePlane);

            foreach (var checklistItem in _packingList)
            {
                checklistItem.Deleted += Save;
                checklistItem.Updated += Save;
            }
        }

        private void OnDisable()
        {
            _clearAll.onClick.RemoveListener(ClearAllPlanes);
            _addNewItem.onClick.RemoveListener(EnablePlane);

            foreach (var checklistItem in _packingList)
            {
                checklistItem.Deleted -= Save;
                checklistItem.Updated -= Save;
            }
        }
        
        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }
        
        public void DisableAllPlanes()
        {
            foreach (var checklistItem in _packingList)
            {
                checklistItem.Disable();
            }

            ToggleEmptyObject();
        }

        public void EnablePlane(CheckListData item)
        {
            _packingList.FirstOrDefault(p => !p.IsActive)?.Enable(item);

            ToggleEmptyObject();
        }

        private void EnablePlane()
        {
            _packingList.FirstOrDefault(p => !p.IsActive)?.Enable();
            ToggleEmptyObject();
        }

        private void ClearAllPlanes()
        {
            DisableAllPlanes();
            Save();
        }

        private void ToggleEmptyObject()
        {
            _emptyListObject.SetActive(_packingList.All(p => !p.IsActive));
        }

        private void Save()
        {
            OnSave?.Invoke();
        }
    }
}