using TMPro;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI phaseText;

    private void OnEnable()
    {
        GameEventBus.OnPlayerDamaged += (d, h) => UpdatePlayerHP(h);
        GameEventBus.OnEnemyDamaged += (d, h) => UpdateEnemyHP(h);
        GameEventBus.OnTimerUpdated += UpdateTimer;
        GameEventBus.OnWaveStarted += ResetUI;
    }

    private void ResetUI()
    {
        UpdatePlayerHP(GameConstants.PLAYER_MAX_HP);
        UpdateEnemyHP(GameConstants.ENEMY_BASE_HP);
        timerText.text = "00.0";
    }

    public void SetPhaseText(string text) => phaseText.text = text;

    private void UpdatePlayerHP(int hp) => playerHPText.text = $"Player HP: {hp}";
    private void UpdateEnemyHP(int hp) => enemyHPText.text = $"Enemy HP: {hp}";
    private void UpdateTimer(float time) => timerText.text = time.ToString("F1");
}