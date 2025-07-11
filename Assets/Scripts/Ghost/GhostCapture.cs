using UnityEngine;
using System.Collections;
using TMPro;

public class GhostCapture : MonoBehaviour
{
    public BookGenre ghostGenre; 

    public float timeToCapture = 3f;
    public Color startColor = Color.white;
    public ParticleSystem captureEffect;

    private SpriteRenderer spriteRenderer;
    private Coroutine captureCoroutine;
    public bool IsBeingCaptured { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = startColor;
    }

    public void StartCaptureProcess()
    {
        if (IsBeingCaptured) return;
        IsBeingCaptured = true;
        captureCoroutine = StartCoroutine(CaptureRoutine());
    }

    public void CancelCaptureProcess()
    {
        if (!IsBeingCaptured) return;
        if (captureCoroutine != null)
        {
            StopCoroutine(captureCoroutine);
            captureCoroutine = null;
        }
        IsBeingCaptured = false;
        if (spriteRenderer != null)
            spriteRenderer.color = startColor;
        if (captureEffect != null && captureEffect.isPlaying)
            captureEffect.Stop();
    }

    private IEnumerator CaptureRoutine()
    {
        float elapsedTime = 0f;
        Color originalColor = spriteRenderer != null ? spriteRenderer.color : startColor;

        if (captureEffect != null)
            captureEffect.Play();

        while (elapsedTime < timeToCapture)
        {
            elapsedTime += Time.deltaTime;
            float process = elapsedTime / timeToCapture;
            float newAlpha = startColor.a - process;

            if (spriteRenderer != null)
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

            yield return null;
        }
        OnCaptured();
    }

    private void OnCaptured()
    {
        if (captureEffect != null)
            captureEffect.Stop();

        
         if (GameManager.Instance != null)
         {
             GameManager.Instance.GhostCaptured();
         }

        Destroy(gameObject);
    }
}