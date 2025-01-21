using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[System.Serializable]
public class NotebookItem
{
    public string id;
    public string name;
    public string type; // "student", "object", or "room"
    public string description;
    public Vector3 position; // For AR positioning
    public DateTime timeAdded;
}

public class ARNotebookManager : MonoBehaviour
{
    [SerializeField] private GameObject notebookUI; // Reference to your notebook UI panel
    [SerializeField] private GameObject itemPrefab; // Prefab for displaying saved items
    [SerializeField] private Transform itemContainer; // Parent transform for instantiated items
    
    private List<NotebookItem> savedItems = new List<NotebookItem>();
    
    // Singleton pattern
    public static ARNotebookManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadSavedItems();
    }
    
    public void AddItemToNotebook(string name, string type, string description, Vector3 position)
    {
        NotebookItem newItem = new NotebookItem
        {
            id = Guid.NewGuid().ToString(),
            name = name,
            type = type,
            description = description,
            position = position,
            timeAdded = DateTime.Now
        };
        
        savedItems.Add(newItem);
        SaveItems();
        RefreshNotebookUI();
    }
    
    public void RemoveItem(string id)
    {
        savedItems.RemoveAll(item => item.id == id);
        SaveItems();
        RefreshNotebookUI();
    }
    
    public void ToggleNotebook()
    {
        notebookUI.SetActive(!notebookUI.activeSelf);
        if (notebookUI.activeSelf)
        {
            RefreshNotebookUI();
        }
    }
    
    private void RefreshNotebookUI()
    {
        // Clear existing items
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create new item entries
        foreach (var item in savedItems)
        {
            GameObject newEntry = Instantiate(itemPrefab, itemContainer);
            SetupItemUI(newEntry, item);
        }
    }
    
    private void SetupItemUI(GameObject itemUI, NotebookItem item)
    {
        // Assuming your itemPrefab has these components
        itemUI.transform.Find("NameText").GetComponent<Text>().text = item.name;
        itemUI.transform.Find("TypeText").GetComponent<Text>().text = item.type;
        itemUI.transform.Find("DescriptionText").GetComponent<Text>().text = item.description;
        
        // Add button to remove item
        Button removeButton = itemUI.transform.Find("RemoveButton").GetComponent<Button>();
        removeButton.onClick.AddListener(() => RemoveItem(item.id));
        
        // Add button to focus AR camera on item
        Button focusButton = itemUI.transform.Find("FocusButton").GetComponent<Button>();
        focusButton.onClick.AddListener(() => FocusOnItem(item));
    }
    
    private void FocusOnItem(NotebookItem item)
    {
        // Handle AR camera positioning/focusing
        // You'll need to implement this based on your AR setup
        Debug.Log($"Focusing on item at position: {item.position}");
    }
    
    private void SaveItems()
    {
        string jsonData = JsonUtility.ToJson(new { items = savedItems });
        PlayerPrefs.SetString("ARNotebookItems", jsonData);
        PlayerPrefs.Save();
    }
    
    private void LoadSavedItems()
    {
        if (PlayerPrefs.HasKey("ARNotebookItems"))
        {
            string jsonData = PlayerPrefs.GetString("ARNotebookItems");
            savedItems = JsonUtility.FromJson<Wrapper<List<NotebookItem>>>(jsonData).items;
        }
    }
    
    [Serializable]
    private class Wrapper<T>
    {
        public T items;
    }
}
