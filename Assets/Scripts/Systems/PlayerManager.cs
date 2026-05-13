using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int CurrentHP { get; private set; }
    public int MaxHP => GameConstants.PLAYER_MAX_HP;

    private void Awake()
    {
        if (RunManager.Instance != null)
        {
            CurrentHP = RunManager.Instance.CurrentHP;
        }
        else
        {
            CurrentHP = MaxHP;
        }
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
        // マップ形式ではRunManagerが管理するため、ここでは何もしないか、
        // 必要に応じて初期化処理を書く
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        CurrentHP = Mathf.Max(0, CurrentHP);
        
        if (RunManager.Instance != null)
        {
            RunManager.Instance.UpdateHP(CurrentHP);
        }

        GameEventBus.PublishPlayerDamaged(damage, CurrentHP);

        if (CurrentHP <= 0)
        {
            GameEventBus.PublishPlayerDefeated();
        }
    }
}