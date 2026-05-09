using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int CurrentHP { get; private set; }
    public int MaxHP => GameConstants.PLAYER_MAX_HP;

    private void Awake()
    {
        CurrentHP = MaxHP;
    }

    private void OnEnable()
    {
        GameEventBus.OnWaveStarted += ResetHPOnFirstWave;
    }

    private void OnDisable()
    {
        GameEventBus.OnWaveStarted -= ResetHPOnFirstWave;
    }

    private void ResetHPOnFirstWave()
    {
        // 最初のウェーブ開始時にのみ全回復させるなどの処理（必要なら）
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        CurrentHP = Mathf.Max(0, CurrentHP);
        GameEventBus.PublishPlayerDamaged(damage, CurrentHP);

        if (CurrentHP <= 0)
        {
            GameEventBus.PublishPlayerDefeated();
        }
    }
}