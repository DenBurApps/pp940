using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CheckListScreen
{
    public class ChecklistItem : MonoBehaviour
    {
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _defaultSprite;

        [SerializeField] private Button _checkButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private bool _isPacking;

        private bool _isChecked;
        
        public event Action Deleted;
        public event Action Updated;
        
        public TMP_InputField InputField => _inputField;
        public bool IsActive { get; private set; }
        public CheckListData CheckListData { get; private set; }
        
        private void OnEnable()
        {
            _checkButton.onClick.AddListener(ToggleButton);
            _inputField.onValueChanged.AddListener(SetName);
            _deleteButton.onClick.AddListener(Delete);
        }

        private void OnDisable()
        {
            _checkButton.onClick.RemoveListener(ToggleButton);
            _inputField.onValueChanged.RemoveListener(SetName);
            _deleteButton.onClick.RemoveListener(Delete);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            IsActive = true;

            CheckListData = new CheckListData
            {
                IsPacking = _isPacking,
                IsChecked = _isChecked,
                Name = _inputField.text
            };
    
            Debug.Log($"Enabled item with IsPacking={_isPacking}");
        }

        public void Enable(CheckListData data)
        {
            gameObject.SetActive(true);
            IsActive = true;
            CheckListData = data;
            
            _isChecked = data.IsChecked;
            _checkButton.image.sprite = _isChecked ? _selectedSprite : _defaultSprite;
            _inputField.text = data.Name;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            IsActive = false;

            _inputField.text = string.Empty;
            CheckListData = null;
            _isChecked = false;
            _checkButton.image.sprite = _defaultSprite;
        }

        private void Delete()
        {
            gameObject.SetActive(false);
            IsActive = false;

            CheckListData = null;
            _isChecked = false;
            _checkButton.image.sprite = _defaultSprite;
            Deleted?.Invoke();
        }

        private void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            CheckListData.Name = name;
            _inputField.text = name;
            Updated?.Invoke();
        }

        private void ToggleButton()
        {
            _isChecked = !_isChecked;
            _checkButton.image.sprite = _isChecked ? _selectedSprite : _defaultSprite;
    
            CheckListData.IsChecked = _isChecked;
    
            Updated?.Invoke();
        }
    }

    public class CheckListData
    {
        public string Name;
        public bool IsChecked;
        public bool IsPacking;
    }
}