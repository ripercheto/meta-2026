using UnityEngine;
using UnityEngine.Events;

public class BallEscapeDetector : MonoBehaviour
{
    [Header("Box Reference")]
    public Transform hollowBoxTransform;
    public HollowBoxCollider hollowBoxSetup;

    [Header("Fail State Event")]
    [Tooltip("Triggered immediately when the ball clips outside the box.")]
    public UnityEvent OnBallEscaped;

    private bool isFailed = false;

    private void LateUpdate()
    {
        if (isFailed || hollowBoxTransform == null || hollowBoxSetup == null) return;

        // Convert ball position into the local space of the box
        Vector3 localPos = hollowBoxTransform.InverseTransformPoint(transform.position);

        // Calculate maximum allowed inner boundary limits
        float ballRadius = transform.localScale.x * 0.5f;
        float limitX = (hollowBoxSetup.outerSize.x * 0.5f) - hollowBoxSetup.wallThickness - ballRadius;
        float limitY = (hollowBoxSetup.outerSize.y * 0.5f) - hollowBoxSetup.wallThickness - ballRadius;
        float limitZ = (hollowBoxSetup.outerSize.z * 0.5f) - hollowBoxSetup.wallThickness - ballRadius;

        // Check if ball is outside any face boundary (plus a tiny tolerance buffer)
        float tolerance = 0.01f; 
        if (Mathf.Abs(localPos.x) > limitX + tolerance ||
            Mathf.Abs(localPos.y) > limitY + tolerance ||
            Mathf.Abs(localPos.z) > limitZ + tolerance)
        {
            TriggerFailState();
        }
    }

    private void TriggerFailState()
    {
        isFailed = true;
        Debug.LogWarning("BALL ESCAPED! Fail state triggered.");

        // Invoke inspector events (e.g., play sound, show UI, reload scene)
        OnBallEscaped?.Invoke();
    }

    /// <summary>
    /// Call this when restarting or resetting the level.
    /// </summary>
    public void ResetFailState()
    {
        isFailed = false;
    }
}