using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    private int waveCount = 0;

    private void OnEnable()
    {
        GameEventBus.OnWaveStarted += HandleWaveStarted;
    }

    private void OnDisable()
    {
        GameEventBus.OnWaveStarted -= HandleWaveStarted;
    }

    private void HandleWaveStarted()
    {
        int floor = 1;
        if (RunManager.Instance != null)
        {
            floor = RunManager.Instance.CurrentFloor;
        }
        else
        {
            waveCount++;
            floor = waveCount;
        }

        MaxHP = Mathf.RoundToInt(GameConstants.ENEMY_BASE_HP * (1 + (floor - 1) * GameConstants.ENEMY_HP_SCALE_PER_WAVE));
        CurrentHP = MaxHP;
        Debug.Log($"Enemy spawned: Floor {floor}, HP {CurrentHP}");
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        CurrentHP = Mathf.Max(0, CurrentHP);
        GameEventBus.PublishEnemyDamaged(damage, CurrentHP);

        if (CurrentHP <= 0)
        {
            GameEventBus.PublishEnemyDefeated();
        }
    }
}