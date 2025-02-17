using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ScrollFix : MonoBehaviour, IScrollHandler
{
    private ScrollRect _scrollRect;
    private List<TMP_InputField> _inputFields;
    private bool _isInputFieldFocused = false;
    
    public void AssignInputFields(List<TMP_InputField> list)
    {
        _inputFields = list;
        Initialize();
        Debug.Log(list.Count);
    }

    private void Initialize()
    {
        
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        if (!_isInputFieldFocused)
        {
            _scrollRect.OnScroll(eventData);
        }
    }
}