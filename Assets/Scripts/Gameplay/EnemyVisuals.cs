using UnityEngine;
using System.Collections;

/// <summary>
/// 敵の画像切り替えと表示状態を管理するコンポーネント。
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyVisuals : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite hitSprite;

    [Header("Settings")]
    public float hitDisplayDuration = 0.3f;

    private SpriteRenderer spriteRenderer;
    private Coroutine hitCoroutine;
    private bool isAttacking = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        GameEventBus.OnWaveStarted += HandleWaveStarted;
        GameEventBus.OnEnemyDamaged += HandleEnemyDamaged;
        GameEventBus.OnEnemyAttackStarted += HandleEnemyAttackStarted;
        GameEventBus.OnEnemyAttackFinished += HandleEnemyAttackFinished;
    }

    private void OnDisable()
    {
        GameEventBus.OnWaveStarted -= HandleWaveStarted;
        GameEventBus.OnEnemyDamaged -= HandleEnemyDamaged;
        GameEventBus.OnEnemyAttackStarted -= HandleEnemyAttackStarted;
        GameEventBus.OnEnemyAttackFinished -= HandleEnemyAttackFinished;
    }

    private void HandleWaveStarted()
    {
        SetSprite(idleSprite);
        isAttacking = false;
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
    }

    private void HandleEnemyDamaged(int damage, int remainingHP)
    {
        if (isAttacking) return; // 攻撃中は攻撃画像を優先

        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        hitCoroutine = StartCoroutine(ShowHitSprite());
    }

    private void HandleEnemyAttackStarted()
    {
        isAttacking = true;
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        SetSprite(attackSprite);
    }

    private void HandleEnemyAttackFinished()
    {
        isAttacking = false;
        SetSprite(idleSprite);
    }

    private IEnumerator ShowHitSprite()
    {
        SetSprite(hitSprite);
        yield return new WaitForSeconds(hitDisplayDuration);
        if (!isAttacking)
        {
            SetSprite(idleSprite);
        }
        hitCoroutine = null;
    }

    private void SetSprite(Sprite sprite)
    {
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
