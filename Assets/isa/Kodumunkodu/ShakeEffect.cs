using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class ShakeEffect : MonoBehaviour
{
    public float shakeMagnitude = 10f; // Sallama efektinin þiddeti
    public float shakeDuration = 0.5f; // Sallama süresi
    public float shakeDecay = 0.5f; // Sallama þiddetinin zamanla azalmasý için faktör
    public float minShakeMagnitude = 1f; // Sallamanýn sabit kalacaðý minimum þiddet

    private Vector3 originalPosition; // Metnin orijinal pozisyonu
    private TMP_Text textComponent;

    private bool isShaking = false; // Sallama durumunu kontrol etmek için

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        originalPosition = textComponent.rectTransform.localPosition;
    }

    public void TriggerShake()
    {
        if (!isShaking) // Eðer zaten sallama yapýyorsa, yeni bir sallama baþlatma
        {
            StopAllCoroutines(); // Devam eden sallamalarý durdur
            StartCoroutine(ShakeCoroutine());
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        isShaking = true; // Sallama baþladý

        float elapsed = 0f;
        float initialShakeMagnitude = shakeMagnitude; // Baþlangýçtaki sallama þiddeti

        // Ýlk baþtaki sallamayý yapalým
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // Sallama þiddetini azalt
            float currentShakeMagnitude = Mathf.Lerp(initialShakeMagnitude, minShakeMagnitude, elapsed / shakeDuration);

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

        // Sallama bittiðinde pozisyonu eski haline getir
        textComponent.rectTransform.localPosition = originalPosition;

        // Sallama tamamlandý
        isShaking = false;

        // Yavaþça sabit bir seviyeye gelene kadar devam et
        while (!isShaking)
        {
            yield return new WaitForSeconds(0.1f); // Yavaþça sallanmasý için küçük bir gecikme
            ApplyConstantShake(); // Sabit sallama efektini uygula
        }
    }

    private void ApplyConstantShake()
    {
        // Sabit bir þiddette sallama uygula
        Vector3 shakeOffset = new Vector3(
            Random.Range(-minShakeMagnitude, minShakeMagnitude),
            Random.Range(-minShakeMagnitude, minShakeMagnitude),
            0f
        );

        // Ofseti metin pozisyonuna uygula
        textComponent.rectTransform.localPosition = originalPosition + shakeOffset;
    }
}
