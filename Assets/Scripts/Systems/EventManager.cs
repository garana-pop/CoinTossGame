using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button option1Button;
    public Button option2Button;
    public TextMeshProUGUI option1Text;
    public TextMeshProUGUI option2Text;

    private void Start()
    {
        SetupRandomEvent();
    }

    private void SetupRandomEvent()
    {
        // シンプルなランダムイベントの例
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            titleText.text = "Mysterious Fountain";
            descriptionText.text = "You find a fountain with shimmering water. Do you drink from it?";
            option1Text.text = "Drink (Heal 20 HP)";
            option2Text.text = "Leave";

            option1Button.onClick.AddListener(() => {
                if (RunManager.Instance != null) {
                    RunManager.Instance.CurrentHP = Mathf.Min(GameConstants.PLAYER_MAX_HP, RunManager.Instance.CurrentHP + 20);
                }
                RunManager.Instance.LoadMap();
            });
            option2Button.onClick.AddListener(() => RunManager.Instance.LoadMap());
        }
        else
        {
            titleText.text = "Thief in the Shadows";
            descriptionText.text = "A thief demands some money. What do you do?";
            option1Text.text = "Pay 30 Money";
            option2Text.text = "Run away (Lose 10 HP)";

            option1Button.onClick.AddListener(() => {
                if (RunManager.Instance != null) {
                    RunManager.Instance.SubtractMoney(30);
                }
                RunManager.Instance.LoadMap();
            });
            option2Button.onClick.AddListener(() => {
                if (RunManager.Instance != null) {
                    RunManager.Instance.CurrentHP = Mathf.Max(0, RunManager.Instance.CurrentHP - 10);
                }
                RunManager.Instance.LoadMap();
            });
        }
    }
}
