using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CheckListScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class ToDoList : MonoBehaviour
    {
        [SerializeField] private List<ChecklistItem> _toDoList;
        [SerializeField] private Button _addNewItem;
        [SerializeField] private Button _clearAll;
        [SerializeField] private GameObject _emptyListObject;
        [SerializeField] private ScrollFix _scrollFix;

        private ScreenVisabilityHandler _screenVisabilityHandler;
        
        public IReadOnlyCollection<ChecklistItem> ChecklistItems => _toDoList;
        
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
            
            foreach (var checklistItem in _toDoList)
            {
                checklistItem.Deleted += Save;
                checklistItem.Updated += Save;
            }
            
            _scrollFix.AssignInputFields(_toDoList.Select(i => i.InputField).ToList());
        }

        private void OnDisable()
        {
            _clearAll.onClick.RemoveListener(ClearAllPlanes);
            _addNewItem.onClick.RemoveListener(EnablePlane);
            
            foreach (var checklistItem in _toDoList)
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
        
        public void EnablePlane(CheckListData item)
        {
            _toDoList.FirstOrDefault(p => !p.IsActive)?.Enable(item);
            ToggleEmptyObject();
        }

        public void DisableAllPlanes()
        {
            foreach (var checklistItem in _toDoList)
            {
                checklistItem.Disable();
            }
            
            ToggleEmptyObject();
        }
        
        private void EnablePlane()
        {
            _toDoList.FirstOrDefault(p => !p.IsActive)?.Enable();
            
            ToggleEmptyObject();
        }
        
        private void ClearAllPlanes()
        {
            DisableAllPlanes();
            Save();
        }
        
        private void ToggleEmptyObject()
        {
            _emptyListObject.SetActive(_toDoList.All(p => !p.IsActive));
        }

        private void Save()
        {
            OnSave?.Invoke();
        }
    }
}
