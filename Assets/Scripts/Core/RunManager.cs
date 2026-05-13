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
    public int Money;
    public GameObject CurrentCoinPrefab;
    public GameObject defaultCoinPrefab;

    [Header("Progress")]
    public int CurrentFloor = 1;
    public List<string> Inventory = new List<string>();

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

    public void InitializeRun()
    {
        CurrentHP = GameConstants.PLAYER_MAX_HP;
        Money = 0;
        CurrentFloor = 1;
        Inventory.Clear();
        CurrentCoinPrefab = defaultCoinPrefab;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        GameEventBus.PublishMoneyChanged(Money);
    }

    public void SubtractMoney(int amount)
    {
        Money -= amount;
        GameEventBus.PublishMoneyChanged(Money);
    }

    public void UpdateHP(int hp)
    {
        CurrentHP = hp;
    }

    // シーン遷移用ショートカット
    public void LoadMap() => SceneManager.LoadScene("MapScene");
    public void LoadBattle() => SceneManager.LoadScene("BattleScene");
    public void LoadStore() => SceneManager.LoadScene("StoreScene");
    public void LoadEvent() => SceneManager.LoadScene("EventScene");
}
