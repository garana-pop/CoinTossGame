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
        while (stateMachine.Current != GameStateMachine.State.GameOver)
        {
            // 1. Wave Start
            battleUI.SetPhaseText("WAVE START");
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
                playerManager.TakeDamage(GameConstants.ENEMY_ATTACK_DAMAGE);
                yield return new WaitForSeconds(GameConstants.DISPLAY_DURATION);
                
                // ターン終了時に器のコインをリセット
                vessel.ResetCoins();

                if (stateMachine.Current == GameStateMachine.State.GameOver) yield break;
}

            if (stateMachine.Current == GameStateMachine.State.GameOver) yield break;

            // 6. PowerUp Phase
            stateMachine.Transition(GameStateMachine.State.PowerUp);
            battleUI.SetPhaseText("POWER UP!");
            powerUpUI.ShowChoices();
            
            bool waitingForPowerUp = true;
            System.Action onPowerUp = () => waitingForPowerUp = false;
            GameEventBus.OnPowerUpSelected += onPowerUp;
            while (waitingForPowerUp) yield return null;
            GameEventBus.OnPowerUpSelected -= onPowerUp;

            // 7. Wave Transit
            stateMachine.Transition(GameStateMachine.State.WaveTransit);
            battleUI.SetPhaseText("NEXT WAVE...");
            vessel.ResetCoins();
            yield return new WaitForSeconds(GameConstants.WAVE_TRANSIT_DURATION);
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