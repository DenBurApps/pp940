using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InputFieldScrollFix : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TMP_InputField _inputField;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down on Container");
        
        if (RectTransformUtility.RectangleContainsScreenPoint(
                _inputField.textComponent.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera))
        {
            Debug.Log("Inside InputField - Activate Input");
            _inputField.ActivateInputField();
        }
        else
        {
            Debug.Log("Outside InputField - Deactivate Input");
            EventSystem.current.SetSelectedGameObject(null);
            _inputField.DeactivateInputField();
        }
    }
}