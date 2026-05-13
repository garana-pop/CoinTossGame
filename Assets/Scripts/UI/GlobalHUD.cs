using UnityEngine;
using TMPro;

public class GlobalHUD : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        GameEventBus.OnPlayerDamaged += (d, h) => UpdateHP(h);
        GameEventBus.OnMoneyChanged += UpdateMoney;
    }

    private void Start()
    {
        if (RunManager.Instance != null)
        {
            UpdateHP(RunManager.Instance.CurrentHP);
            UpdateMoney(RunManager.Instance.Money);
        }
    }

    private void UpdateHP(int hp) => hpText.text = $"HP: {hp}";
    private void UpdateMoney(int money) => moneyText.text = $"Money: {money}";
}
