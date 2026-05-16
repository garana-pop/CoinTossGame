using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HistoryPanel : MonoBehaviour
{
    public GameObject historyItemPrefab;
    public Transform contentContainer;
    public Button closeButton;

    private void OnEnable()
    {
        GameEventBus.OnHistoryAdded += AddHistoryItem;
        RefreshAll();
        if (closeButton != null) closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        GameEventBus.OnHistoryAdded -= AddHistoryItem;
        if (closeButton != null) closeButton.onClick.RemoveAllListeners();
    }

    private void RefreshAll()
    {
        // Clear existing
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // Load from RunManager
        if (RunManager.Instance != null)
        {
            foreach (var log in RunManager.Instance.HistoryLog)
            {
                AddHistoryItem(log);
            }
        }
    }

    private void AddHistoryItem(string message)
    {
        if (historyItemPrefab != null)
        {
            GameObject item = Instantiate(historyItemPrefab, contentContainer);
            var text = item.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = message;
        }
        else
        {
            // Fallback to simple Text object if no prefab
            GameObject go = new GameObject("HistoryItem", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(contentContainer, false);
            var text = go.GetComponent<TextMeshProUGUI>();
            text.text = message;
            text.fontSize = 18;
        }
    }
}
