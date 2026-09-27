using System.Collections;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField]
    private Material paintedMaterial;

    [Header("Juice Settings")]
    [SerializeField]
    private float bounceAmount = 0.15f;
    [SerializeField]
    private float bounceDuration = 0.2f;

    [SerializeField]
    private MeshRenderer meshRenderer;
    private Vector3 originalScale;
    public bool IsPainted { get; private set; }

    private void Awake()
    {
        originalScale = meshRenderer.transform.localScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsPainted)
        {
            for (var i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.GetContact(i);
                var isActuallyTouching = contact.separation <= 0.00001f;

                if (isActuallyTouching)
                {
                    Debug.Log(collision.gameObject);
                    PaintTile();
                    break;
                }
            }
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
        var elapsed = 0f;

        // Squash down (compress Y, expand X and Z)
        var squashedScale = new Vector3(
            originalScale.x * (1f + bounceAmount),
            originalScale.y * (1f - bounceAmount),
            originalScale.z * (1f + bounceAmount)
        );

        // Phase 1: Compress on contact
        while (elapsed < bounceDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / (bounceDuration * 0.5f);
            meshRenderer.transform.localScale = Vector3.Lerp(originalScale, squashedScale, t);
            yield return null;
        }

        elapsed = 0f;

        // Phase 2: Snap back to original scale
        while (elapsed < bounceDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / (bounceDuration * 0.5f);
            meshRenderer.transform.localScale = Vector3.Lerp(squashedScale, originalScale, t);
            yield return null;
        }

        meshRenderer.transform.localScale = originalScale;
    }
}