using TMPro;
using UnityEngine;

namespace AddTripScreen
{
    public class AddedInfoPlane : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;

        public bool IsActive { get; private set; }

        public void Enable(string name)
        {
            gameObject.SetActive(true);
            IsActive = true;

            _nameText.text = name;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
            IsActive = false;

            _nameText.text = string.Empty;
        }
    }
}
