using UnityEngine;
using System.Collections;

/// <summary>
/// 敵の攻撃時にカメラを敵にフォーカスさせる制御コンポーネント。
/// </summary>
public class EnemyCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraRig;
    public Transform focusTarget;

    [Header("Focus Settings")]
    public Vector3 focusOffset = new Vector3(0, 1, -3); // 敵から見たカメラの相対位置
    public float smoothTime = 0.3f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 currentVelocity;
    private float currentRotationVelocity;

    private bool isFocusing = false;

    private void Start()
    {
        if (focusTarget == null) focusTarget = transform;
        
        if (cameraRig != null)
        {
            originalPosition = cameraRig.position;
            originalRotation = cameraRig.rotation;
        }
    }

    private void OnEnable()
    {
        GameEventBus.OnEnemyAttackStarted += HandleAttackStarted;
        GameEventBus.OnEnemyAttackFinished += HandleAttackFinished;
    }

    private void OnDisable()
    {
        GameEventBus.OnEnemyAttackStarted -= HandleAttackStarted;
        GameEventBus.OnEnemyAttackFinished -= HandleAttackFinished;
    }

    private void HandleAttackStarted()
    {
        if (cameraRig == null) return;
        isFocusing = true;
        StopAllCoroutines();
        StartCoroutine(MoveCamera(focusTarget.position + focusTarget.TransformDirection(focusOffset), 
            Quaternion.LookRotation(focusTarget.position + Vector3.up - (focusTarget.position + focusTarget.TransformDirection(focusOffset)))));
    }

    private void HandleAttackFinished()
    {
        if (cameraRig == null) return;
        isFocusing = false;
        StopAllCoroutines();
        StartCoroutine(MoveCamera(originalPosition, originalRotation));
    }

    private IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot)
    {
        float elapsed = 0;
        Vector3 startPos = cameraRig.position;
        Quaternion startRot = cameraRig.rotation;

        while (elapsed < smoothTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / smoothTime);
            
            cameraRig.position = Vector3.Lerp(startPos, targetPos, t);
            cameraRig.rotation = Quaternion.Slerp(startRot, targetRot, t);
            
            yield return null;
        }

        cameraRig.position = targetPos;
        cameraRig.rotation = targetRot;
    }
}
