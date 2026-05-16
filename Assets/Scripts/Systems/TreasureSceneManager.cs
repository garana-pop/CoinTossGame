using UnityEngine;
using UnityEngine.UI;

public class TreasureSceneManager : MonoBehaviour
{
    public Button openButton;
    public string testItemName = "Lucky Coin";

    private void Start()
    {
        if (openButton != null)
        {
            openButton.onClick.AddListener(OnOpenClicked);
        }
    }

    private void OnOpenClicked()
    {
        if (RunManager.Instance != null)
        {
            RunManager.Instance.AddItem(testItemName);
            RunManager.Instance.LoadMap();
        }
    }
}
