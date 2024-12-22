using UnityEngine;
using TMPro;
using System.Collections;


[RequireComponent(typeof(TMP_Text))]
public class ShakeEffect : MonoBehaviour
{
    public float shakeMagnitude = 10f; // Sallama efektinin þiddeti
    public float shakeDuration = 0.5f; // Sallama süresi
    public float shakeDecay = 0.5f; // Sallama þiddetinin zamanla azalmasý için faktör
    private Vector3 originalPosition; // Metnin orijinal pozisyonu

    private TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        originalPosition = textComponent.rectTransform.localPosition;
    }

    public void TriggerShake()
    {
        StopAllCoroutines(); // Devam eden sallamalarý durdur
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;
        float initialShakeMagnitude = shakeMagnitude; // Baþlangýçtaki sallama þiddeti

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // Sallama þiddetini azalt
            float currentShakeMagnitude = Mathf.Lerp(initialShakeMagnitude, 0f, elapsed / shakeDuration);

            // Rastgele bir sallama ofseti oluþtur
            Vector3 shakeOffset = new Vector3(
                Random.Range(-currentShakeMagnitude, currentShakeMagnitude),
                Random.Range(-currentShakeMagnitude, currentShakeMagnitude),
                0f
            );

            // Ofseti metin pozisyonuna uygula
            textComponent.rectTransform.localPosition = originalPosition + shakeOffset;

            yield return null;
        }

        // Pozisyonu eski haline getir
        textComponent.rectTransform.localPosition = originalPosition;
    }
}