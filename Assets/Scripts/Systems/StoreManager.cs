using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public Button backButton;
    public Button buyHealButton;
    public Button buyItemButton;
    public ItemData itemToSell;

    private void Start()
    {
        UpdateMoneyDisplay();
        if (backButton != null) backButton.onClick.AddListener(() => RunManager.Instance.LoadMap());
        if (buyHealButton != null) buyHealButton.onClick.AddListener(() => BuyHPRecovery(50, 20));
        if (buyItemButton != null) buyItemButton.onClick.AddListener(() => BuyItem(100, itemToSell));
    }

    private void UpdateMoneyDisplay()
    {
        if (RunManager.Instance != null)
        {
            moneyText.text = $"Money: {RunManager.Instance.Money}";
        }
    }

    public void BuyHPRecovery(int cost, int amount)
    {
        if (RunManager.Instance != null && RunManager.Instance.Money >= cost)
        {
            RunManager.Instance.SubtractMoney(cost);
            RunManager.Instance.CurrentHP = Mathf.Min(GameConstants.PLAYER_MAX_HP, RunManager.Instance.CurrentHP + amount);
            UpdateMoneyDisplay();
            Debug.Log($"Bought HP Recovery! Current HP: {RunManager.Instance.CurrentHP}");
        }
    }

    public void BuyItem(int cost, ItemData item)
    {
        if (item == null) return;
        if (RunManager.Instance != null && RunManager.Instance.Money >= cost)
        {
            RunManager.Instance.SubtractMoney(cost);
            RunManager.Instance.Inventory.Add(item.itemName);
            if (item.coinPrefabOverride != null)
            {
                RunManager.Instance.CurrentCoinPrefab = item.coinPrefabOverride;
            }
            UpdateMoneyDisplay();
            GameEventBus.PublishItemObtained();
            Debug.Log($"Bought Item: {item.itemName}");
        }
    }
    }
