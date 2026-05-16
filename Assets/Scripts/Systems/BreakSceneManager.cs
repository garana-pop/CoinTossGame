using UnityEngine;
using UnityEngine.UI;

public class BreakSceneManager : MonoBehaviour
{
    public Button restButton;
    public float healPercentage = 0.3f;

    private void Start()
    {
        if (restButton != null)
        {
            restButton.onClick.AddListener(OnRestClicked);
        }
    }

    private void OnRestClicked()
    {
        if (RunManager.Instance != null)
        {
            int healAmount = Mathf.RoundToInt(RunManager.Instance.MaxHP * healPercentage);
            int newHP = Mathf.Min(RunManager.Instance.CurrentHP + healAmount, RunManager.Instance.MaxHP);
            RunManager.Instance.UpdateHP(newHP);
            RunManager.Instance.LoadMap();
        }
    }
}
