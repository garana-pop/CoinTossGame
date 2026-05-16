using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapNode : MonoBehaviour
{
    public MapNodeData data;
    public Button button;
    public TextMeshProUGUI nameText;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(OnNodeClicked);
    }

    private void Start()
    {
        if (data != null && nameText != null)
        {
            nameText.text = data.nodeName;
        }
    }

    private void OnNodeClicked()
    {
        Debug.Log($"MapNode clicked: {data.nodeName} ({data.nodeType})");
        if (RunManager.Instance == null)
        {
            Debug.LogError("RunManager.Instance is null! Cannot transition scene.");
            return;
        }

        switch (data.nodeType)
        {
            case MapNodeData.NodeType.Battle:
                RunManager.Instance.NextBattleType = RunManager.BattleType.Normal;
                RunManager.Instance.LoadBattle();
                break;
            case MapNodeData.NodeType.Elite:
                RunManager.Instance.NextBattleType = RunManager.BattleType.Elite;
                RunManager.Instance.LoadBattle();
                break;
            case MapNodeData.NodeType.Boss:
                RunManager.Instance.NextBattleType = RunManager.BattleType.Boss;
                RunManager.Instance.LoadBattle();
                break;
            case MapNodeData.NodeType.Store:
                RunManager.Instance.LoadStore();
                break;
            case MapNodeData.NodeType.Event:
                RunManager.Instance.LoadEvent();
                break;
            case MapNodeData.NodeType.Treasure:
                RunManager.Instance.LoadTreasure();
                break;
            case MapNodeData.NodeType.Rest:
                RunManager.Instance.LoadBreak();
                break;
        }
    }
}
