using System.Collections;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material unpaintedMaterial;
    [SerializeField] private Material paintedMaterial;

    [Header("Juice Settings")]
    [SerializeField] private float bounceAmount = 0.15f;
    [SerializeField] private float bounceDuration = 0.2f;

    [SerializeField] private MeshRenderer meshRenderer;
    private Vector3 originalScale;
    public bool IsPainted { get; private set; } = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsPainted)
        {
            PaintTile();
        }
    }

    public void PaintTile()
    {
        IsPainted = true;

        // Swap to the toon painted material
        if (paintedMaterial != null)
        {
            meshRenderer.sharedMaterial = paintedMaterial;
        }

        // Play squash animation
        StopAllCoroutines();
        StartCoroutine(AnimateBounce());
    }

    private IEnumerator AnimateBounce()
    {
        float elapsed = 0f;

        // Squash down (compress Y, expand X and Z)
        Vector3 squashedScale = new Vector3(
            originalScale.x * (1f + bounceAmount),
            originalScale.y * (1f - bounceAmount),
            originalScale.z * (1f + bounceAmount)
        );

        // Phase 1: Compress on contact
        while (elapsed < bounceDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (bounceDuration * 0.5f);
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, t);
            yield return null;
        }

        elapsed = 0f;

        // Phase 2: Snap back to original scale
        while (elapsed < bounceDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (bounceDuration * 0.5f);
            transform.localScale = Vector3.Lerp(squashedScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}