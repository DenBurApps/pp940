using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AddPlaceScreen
{
    public class AddPlacePlane : MonoBehaviour
    {
        [SerializeField] private Color _defaultIconColor;
        [SerializeField] private Color _selectedIconColor;
        
        [SerializeField] private Image _icon;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private TMP_InputField _inputField;

        public event Action OnTextChanged;
        public bool IsActive { get; private set; }

        private void OnEnable()
        {
            _inputField.onValueChanged.AddListener(ValidateIconColor);
            _deleteButton.onClick.AddListener(Disable);
        }

        private void OnDisable()
        {
            _inputField.onValueChanged.RemoveListener(ValidateIconColor);
            _deleteButton.onClick.RemoveListener(Disable);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
            IsActive = true;
            
            ValidateIconColor(_inputField.text);
        }

        public void Disable()
        {
            _inputField.text = string.Empty;
            ValidateIconColor(_inputField.text);
            
            gameObject.SetActive(false);
            IsActive = false;
        }
        
        public string GetPlaceText()
        {
            return _inputField.text;
        }

        private void ValidateIconColor(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _icon.color = _defaultIconColor;
            }
            else
            {
                _icon.color = _selectedIconColor;
            }
    
            OnTextChanged?.Invoke();
        }

    }
}
