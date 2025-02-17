using System.Collections.Generic;
using System.Linq;
using TMPro;
using TripData;
using UnityEngine;

namespace AddTripScreen
{
    public class AddedExpensesController : MonoBehaviour
    {
        [SerializeField] private Color _emptyTextColor;
        [SerializeField] private Color _filledTextColor;

        [SerializeField] private GameObject _planesHolder;
        [SerializeField] private List<AddedInfoPlane> _planes;
        [SerializeField] private TMP_Text _countText;

        public List<ExpenseData> ExpenseDatas { get; private set; }

        private void Start()
        {
            ExpenseDatas = new List<ExpenseData>();
            ResetData();
        }

        public void AssignPlaces(List<ExpenseData> data)
        {
            if (data == null) return;

            ExpenseDatas = new List<ExpenseData>(data);
            AssignDataToPlanes();
        }

        public void ResetData()
        {
            ExpenseDatas?.Clear();

            foreach (var addedInfoPlane in _planes)
            {
                addedInfoPlane.Disable();
            }

            UpdateCountText(0);
        }

        private void AssignDataToPlanes()
        {
            if (ExpenseDatas.Count <= 0)
            {
                UpdateCountText(0);
                _planesHolder.SetActive(false);
                return;
            }

            _planesHolder.SetActive(true);

            foreach (var plane in _planes)
            {
                plane.Disable();
            }

            for (int i = 0; i < ExpenseDatas.Count && i < _planes.Count; i++)
            {
                EnablePlane(ExpenseDatas[i].ExpenseName);
            }

            UpdateCountText(ExpenseDatas.Count);
        }

        private void UpdateCountText(int count)
        {
            _countText.text = count.ToString();
            _countText.color = count > 0 ? _filledTextColor : _emptyTextColor;
        }

        private void EnablePlane(string name)
        {
            var availablePlane = _planes.FirstOrDefault(p => !p.IsActive);
            if (availablePlane == null) return;

            availablePlane.Enable(name);
        }
    }
}