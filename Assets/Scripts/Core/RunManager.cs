using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// シーンを跨いでゲームの進行状況（HP、所持金、アイテム等）を管理するシングルトンクラス。
/// </summary>
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Player Status")]
    public int CurrentHP;
    public int MaxHP = GameConstants.PLAYER_MAX_HP;
    public int Money;
    public GameObject CurrentCoinPrefab;
    public GameObject defaultCoinPrefab;

    public enum BattleType
    {
        Normal,
        Elite,
        Boss
    }

    [Header("Progress")]
    public int CurrentFloor = 1;
    public BattleType NextBattleType = BattleType.Normal;
    public MapGraphData currentMap;
    public List<string> Inventory = new List<string>();
    public float PlayTime { get; private set; }
    public List<string> HistoryLog { get; private set; } = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeRun();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        PlayTime += Time.deltaTime;
    }

    public void InitializeRun()
    {
        MaxHP = GameConstants.PLAYER_MAX_HP;
        CurrentHP = MaxHP;
        Money = 0;
        CurrentFloor = 1;
        Inventory.Clear();
        HistoryLog.Clear();
        PlayTime = 0;
        CurrentCoinPrefab = defaultCoinPrefab;

        InitializeMap();
        RecordHistory("Game Started");
    }

    private void InitializeMap()
    {
        currentMap = MapGenerator.GenerateMap(15, 18);
    }

    public void RecordHistory(string message)
    {
        HistoryLog.Add(message);
        GameEventBus.PublishHistoryAdded(message);
        Debug.Log($"[History] {message}");
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        RecordHistory($"Gained {amount} Money (Total: {Money})");
        GameEventBus.PublishMoneyChanged(Money);
    }

    public void SubtractMoney(int amount)
    {
        Money -= amount;
        RecordHistory($"Spent {amount} Money (Total: {Money})");
        GameEventBus.PublishMoneyChanged(Money);
    }

    public void UpdateHP(int hp)
    {
        int diff = hp - CurrentHP;
        CurrentHP = hp;
        if (diff > 0) RecordHistory($"Healed {diff} HP (Current: {CurrentHP}/{MaxHP})");
        else if (diff < 0) RecordHistory($"Took {Mathf.Abs(diff)} Damage (Current: {CurrentHP}/{MaxHP})");
    }

    public void AddItem(string itemName)
    {
        Inventory.Add(itemName);
        RecordHistory($"Obtained Item: {itemName}");
        GameEventBus.PublishItemObtained();
    }

    // シーン遷移用ショートカット
    public void LoadMap() => SceneManager.LoadScene("MapScene");
    public void LoadBattle() => SceneManager.LoadScene("BattleScene");
    public void LoadStore() => SceneManager.LoadScene("StoreScene");
    public void LoadEvent() => SceneManager.LoadScene("EventScene");
    public void LoadTreasure() => SceneManager.LoadScene("TreasureScene");
    public void LoadBreak() => SceneManager.LoadScene("BreakScene");
}
