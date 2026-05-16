using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GlobalHUD : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI timeText;
    public Transform inventoryContainer;
    public GameObject inventoryIconPrefab;
    public Button historyButton;
    public GameObject historyPanel;
    public Button settingsButton;
    public GameObject settingsPanel;

    private void OnEnable()
    {
        GameEventBus.OnPlayerDamaged += (d, h) => UpdateHP(h);
        GameEventBus.OnMoneyChanged += UpdateMoney;
        GameEventBus.OnItemObtained += UpdateInventory;
    }

    private void OnDisable()
    {
        GameEventBus.OnPlayerDamaged -= (d, h) => UpdateHP(h);
        GameEventBus.OnMoneyChanged -= UpdateMoney;
        GameEventBus.OnItemObtained -= UpdateInventory;
    }

    private void Start()
    {
        if (RunManager.Instance != null)
        {
            UpdateHP(RunManager.Instance.CurrentHP);
            UpdateMoney(RunManager.Instance.Money);
            UpdateInventory();
        }

        if (historyButton != null && historyPanel != null)
        {
            historyButton.onClick.RemoveAllListeners();
            historyButton.onClick.AddListener(() => historyPanel.SetActive(true));
            historyPanel.SetActive(false);
        }

        if (settingsButton != null && settingsPanel != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
            settingsPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (RunManager.Instance != null && timeText != null)
        {
            float t = RunManager.Instance.PlayTime;
            int hours = Mathf.FloorToInt(t / 3600);
            int minutes = Mathf.FloorToInt((t % 3600) / 60);
            int seconds = Mathf.FloorToInt(t % 60);
            timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    private void UpdateHP(int hp) 
    {
        if (RunManager.Instance != null)
            hpText.text = $"HP: {hp} / {RunManager.Instance.MaxHP}";
        else
            hpText.text = $"HP: {hp}";
    }

    private void UpdateMoney(int money) => moneyText.text = $"Money: {money}";

    private void UpdateInventory()
    {
        if (inventoryContainer == null) return;

        // Clear existing
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        if (RunManager.Instance == null) return;

        foreach (string itemName in RunManager.Instance.Inventory)
        {
            if (inventoryIconPrefab != null)
            {
                GameObject icon = Instantiate(inventoryIconPrefab, inventoryContainer);
                // Optionally set icon image based on itemName if you have a mapping
            }
            else
            {
                GameObject go = new GameObject("ItemIcon", typeof(RectTransform), typeof(TextMeshProUGUI));
                go.transform.SetParent(inventoryContainer, false);
                var text = go.GetComponent<TextMeshProUGUI>();
                text.text = "[I]"; // Placeholder
            }
        }
    }
}
