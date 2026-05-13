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
        if (RunManager.Instance == null) return;

        switch (data.nodeType)
        {
            case MapNodeData.NodeType.Battle:
            case MapNodeData.NodeType.Boss:
                RunManager.Instance.LoadBattle();
                break;
            case MapNodeData.NodeType.Store:
                RunManager.Instance.LoadStore();
                break;
            case MapNodeData.NodeType.Event:
                RunManager.Instance.LoadEvent();
                break;
        }
    }
}
