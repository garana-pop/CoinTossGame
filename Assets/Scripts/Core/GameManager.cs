using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public CoinLauncher launcher;
    public Vessel vessel;
    public PlayerManager playerManager;
    public EnemyManager enemyManager;
    public ScoreManager scoreManager;
    
    [Header("UI")]
    public BattleUI battleUI;
    public PowerUpUI powerUpUI;

    private GameStateMachine stateMachine;
    private float turnTimer;
    private bool isEnemyDefeated = false;

    private void Awake()
    {
        stateMachine = new GameStateMachine();
        
        // RunManagerがいない場合はデバッグ用に作成（MapSceneから開始するのが正）
        if (RunManager.Instance == null)
        {
            GameObject rm = new GameObject("RunManager_Debug", typeof(RunManager));
        }

        if (launcher == null) launcher = FindFirstObjectByType<CoinLauncher>();
if (vessel == null) vessel = FindFirstObjectByType<Vessel>();
        if (playerManager == null) playerManager = FindFirstObjectByType<PlayerManager>();
        if (enemyManager == null) enemyManager = FindFirstObjectByType<EnemyManager>();
        if (scoreManager == null) scoreManager = FindFirstObjectByType<ScoreManager>();
        if (battleUI == null) battleUI = FindFirstObjectByType<BattleUI>();
        if (powerUpUI == null) powerUpUI = FindFirstObjectByType<PowerUpUI>();
    }

    private void OnEnable()
    {
        GameEventBus.OnEnemyDefeated += HandleEnemyDefeated;
        GameEventBus.OnPlayerDefeated += HandlePlayerDefeated;
    }

    private void OnDisable()
    {
        GameEventBus.OnEnemyDefeated -= HandleEnemyDefeated;
        GameEventBus.OnPlayerDefeated -= HandlePlayerDefeated;
    }

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        // 1. Battle Start
        battleUI.SetPhaseText("BATTLE START");
        GameEventBus.PublishWaveStarted();
        isEnemyDefeated = false;
        yield return new WaitForSeconds(1.0f);

        while (!isEnemyDefeated && stateMachine.Current != GameStateMachine.State.GameOver)
        {
            // 2. Player Phase (Launching)
            stateMachine.Transition(GameStateMachine.State.Launching);
            battleUI.SetPhaseText("PLAYER PHASE");
            launcher.enabled = true;
            turnTimer = GameConstants.TURN_TIME_LIMIT;
            
            while (turnTimer > 0)
            {
                turnTimer -= Time.deltaTime;
                GameEventBus.PublishTimerUpdated(turnTimer);
                yield return null;
            }
            
            launcher.enabled = false;

            // 3. Judging Phase
            stateMachine.Transition(GameStateMachine.State.Judging);
            battleUI.SetPhaseText("JUDGING...");
            yield return new WaitForSeconds(GameConstants.JUDGE_DURATION);

            // 4. Damage Phase
            stateMachine.Transition(GameStateMachine.State.DamagePhase);
            battleUI.SetPhaseText("PLAYER ATTACK!");
            int coins = vessel.GetCoinCount();
            int damage = coins * GameConstants.SCORE_PER_COIN;
            enemyManager.TakeDamage(damage);
            yield return new WaitForSeconds(GameConstants.DISPLAY_DURATION);

            if (isEnemyDefeated) break;

            // 5. Enemy Phase
            stateMachine.Transition(GameStateMachine.State.EnemyPhase);
            battleUI.SetPhaseText("ENEMY PHASE");
            GameEventBus.PublishEnemyAttackStarted();
            playerManager.TakeDamage(GameConstants.ENEMY_ATTACK_DAMAGE);
            yield return new WaitForSeconds(GameConstants.DISPLAY_DURATION);
            GameEventBus.PublishEnemyAttackFinished();

            // ターン終了時に器のコインをリセット
            vessel.ResetCoins();

            if (stateMachine.Current == GameStateMachine.State.GameOver) yield break;
        }

        if (stateMachine.Current == GameStateMachine.State.GameOver)
        {
            yield return new WaitForSeconds(2.0f);
            // 本来はGameOverシーンへ遷移するが、一旦初期化してMapへ戻るかタイトルへ
            if (RunManager.Instance != null) RunManager.Instance.InitializeRun();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
            yield break;
        }

        // 6. Victory & Reward
        battleUI.SetPhaseText("VICTORY!");
        if (RunManager.Instance != null)
        {
            int reward = 50; // 固定報酬。本来はEnemyManagerから取得などが望ましい
            RunManager.Instance.AddMoney(reward);
        }
        yield return new WaitForSeconds(2.0f);

        // 7. Back to Map
        if (RunManager.Instance != null)
        {
            RunManager.Instance.CurrentFloor++;
            RunManager.Instance.LoadMap();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene");
        }
    }

    private void HandleEnemyDefeated()
    {
        isEnemyDefeated = true;
    }

    private void HandlePlayerDefeated()
    {
        stateMachine.Transition(GameStateMachine.State.GameOver);
        battleUI.SetPhaseText("GAME OVER");
    }
}