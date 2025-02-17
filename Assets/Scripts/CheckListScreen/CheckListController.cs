using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using CheckListScreen;

namespace CheckListScreen
{
    [RequireComponent(typeof(ScreenVisabilityHandler))]
    public class CheckListController : MonoBehaviour
    {
        private const string SaveFilePath = "/checklist_data.json";

        [SerializeField] private Color _defaultButtonColor;
        [SerializeField] private Color _selectedButtonColor;

        [SerializeField] private Button _toDoButton;
        [SerializeField] private Button _packingListButton;
        [SerializeField] private ToDoList _doList;
        [SerializeField] private PackingList _packingList;

        private SaveData _saveData;
        private ScreenVisabilityHandler _screenVisabilityHandler;

        private void Awake()
        {
            _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        }

        private void OnEnable()
        {
            _toDoButton.onClick.AddListener(ShowToDoList);
            _packingListButton.onClick.AddListener(ShowPackingList);
        }

        private void OnDisable()
        {
            _toDoButton.onClick.RemoveListener(ShowToDoList);
            _packingListButton.onClick.RemoveListener(ShowPackingList);

            _doList.OnSave -= SaveData;
            _packingList.OnSave -= SaveData;
        }

        private void Start()
        {
            LoadData();
            InitializeLists();
            DisableScreen();
        }

        public void EnableScreen()
        {
            _screenVisabilityHandler.EnableScreen();
        }

        public void DisableScreen()
        {
            _screenVisabilityHandler.DisableScreen();
        }

        private void ShowToDoList()
        {
            _doList.EnableScreen();
            _packingList.DisableScreen();

            _toDoButton.image.color = _selectedButtonColor;
            _packingListButton.image.color = _defaultButtonColor;
        }

        private void ShowPackingList()
        {
            _doList.DisableScreen();
            _packingList.EnableScreen();

            _toDoButton.image.color = _defaultButtonColor;
            _packingListButton.image.color = _selectedButtonColor;
        }

        private void SaveData()
        {
            try 
            {
                Debug.Log($"Starting SaveData() - ToDo items: {_doList.ChecklistItems.Count(x => x.IsActive)}, " +
                          $"Packing items: {_packingList.ChecklistItems.Count(x => x.IsActive)}");

                if (_doList == null || _packingList == null)
                {
                    Debug.LogError("Lists are null in SaveData()");
                    return;
                }

                var todoItems = _doList.ChecklistItems
                    .Where(item => item.IsActive && !item.CheckListData.IsPacking)
                    .Select(item => item.CheckListData)
                    .ToList();

                var packingItems = _packingList.ChecklistItems
                    .Where(item => item.IsActive && item.CheckListData.IsPacking)
                    .Select(item => item.CheckListData)
                    .ToList();

                Debug.Log($"Filtered items - ToDo: {todoItems.Count}, Packing: {packingItems.Count}");

                _saveData = new SaveData
                {
                    ToDoItems = todoItems,
                    PackingItems = packingItems
                };

                string path = Application.persistentDataPath + SaveFilePath;
                Debug.Log($"Saving to path: {path}");

                string json = JsonConvert.SerializeObject(_saveData, new JsonSerializerSettings 
                { 
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogError("JSON serialization resulted in empty string");
                    return;
                }

                // Записываем файл
                File.WriteAllText(path, json);

                Debug.Log($"Successfully saved JSON: {json}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in SaveData(): {e.Message}\n{e.StackTrace}");
            }
        }

        private void LoadData()
        {
            string path = Application.persistentDataPath + SaveFilePath;

            if (!File.Exists(path))
            {
                _saveData = new SaveData
                {
                    ToDoItems = new List<CheckListData>(),
                    PackingItems = new List<CheckListData>()
                };
                return;
            }

            string json = File.ReadAllText(path);
            _saveData = JsonConvert.DeserializeObject<SaveData>(json);
        }

        private void InitializeLists()
        {
            foreach (var packingData in _saveData.PackingItems)
            {
                _packingList.EnablePlane(packingData);
            }

            foreach (var todoData in _saveData.ToDoItems)
            {
                _doList.EnablePlane(todoData);
            }

            ShowToDoList();
            
            _doList.OnSave += SaveData;
            _packingList.OnSave += SaveData;
        }
    }
}

[System.Serializable]
public class SaveData
{
    public List<CheckListData> ToDoItems { get; set; }
    public List<CheckListData> PackingItems { get; set; }
}