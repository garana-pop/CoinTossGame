using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject mapCanvas;

    private void Start()
    {
        // ここで現在の進行度に応じたマップの表示制御などを行う
        Debug.Log($"Current Floor: {RunManager.Instance?.CurrentFloor ?? 1}");
    }
}
