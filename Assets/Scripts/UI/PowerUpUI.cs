using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerUpUI : MonoBehaviour
{
    public GameObject panel;
    public Button[] choiceButtons;
    public GameObject cubeCoinPrefab;

    private void OnEnable()
    {
        panel.SetActive(false);
    }

    public void ShowChoices()
    {
        panel.SetActive(true);
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => SelectPowerUp(index));
        }
    }

    private void SelectPowerUp(int index)
{
        panel.SetActive(false);
        ApplyEffect(index);
        GameEventBus.PublishPowerUpSelected();
    }

    private void ApplyEffect(int index)
    {
        switch(index)
        {
            case 0: // Launch Efficiency
                var launcher = Object.FindFirstObjectByType<CoinLauncher>();
                launcher.AddCoinsPerLaunch(1);
                Debug.Log("PowerUp: Launch Efficiency +1");
                break;
            case 1: // Shape Change (Cube)
                var launcher2 = Object.FindFirstObjectByType<CoinLauncher>();
                launcher2.ChangeCoinPrefab(cubeCoinPrefab);
                Debug.Log("PowerUp: Shape Changed to Cube");
                break;
            case 2: // Vessel Friction (Dampening)
                var vessel = Object.FindFirstObjectByType<Vessel>();
                vessel.IncreaseDampening(0.2f); // 0.5 -> 0.3 (lower factor = more dampening)
                Debug.Log("PowerUp: Vessel Friction Increased");
                break;
        }
    }
}