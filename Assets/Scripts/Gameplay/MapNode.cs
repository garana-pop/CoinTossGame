using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapNode : MonoBehaviour
{
    public MapNodeData data;
    public Button button;
    public TextMeshProUGUI nameText;
    public Image iconImage;

    private MapNodeInstance instance;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(OnNodeClicked);
    }

    public void Setup(MapNodeInstance instance, MapNodeData data, bool isReachable)
    {
        this.instance = instance;
        this.data = data;
        
        if (nameText != null) nameText.text = data.nodeName;
        if (iconImage != null) iconImage.sprite = data.icon;

        // Visual State
        if (instance.isVisited)
        {
            SetVisualState(new Color(0.5f, 0.5f, 0.5f, 1f), false);
        }
        else if (isReachable)
        {
            SetVisualState(Color.white, true);
        }
        else
        {
            SetVisualState(new Color(0.3f, 0.3f, 0.3f, 0.5f), false);
        }
    }

    private void SetVisualState(Color color, bool interactable)
    {
        if (button != null) button.interactable = interactable;
        
        var images = GetComponentsInChildren<Image>();
        foreach (var img in images)
        {
            img.color = color;
        }
        if (nameText != null) nameText.color = color;
    }

    private void OnNodeClicked()
    {
        Debug.Log($"MapNode clicked: {data.nodeName} ({data.nodeType})");
        if (RunManager.Instance == null)
        {
            Debug.LogError("RunManager.Instance is null! Cannot transition scene.");
            return;
        }

        // Update Run State
        RunManager.Instance.currentMap.currentNodeID = instance.nodeID;
        instance.isVisited = true;
        RunManager.Instance.CurrentFloor = instance.layer + 1;

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
