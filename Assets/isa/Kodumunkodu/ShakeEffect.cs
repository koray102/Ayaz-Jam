using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TMP_Text))]
public class ShakeEffect : MonoBehaviour
{
    public float shakeMagnitude = 10f; // Sallama efektinin �iddeti
    public float shakeDuration = 0.5f; // Sallama s�resi
    public float shakeDecay = 0.5f; // Sallama �iddetinin zamanla azalmas� i�in fakt�r
    public float minShakeMagnitude = 1f; // Sallaman�n sabit kalaca�� minimum �iddet

    private Vector3 originalPosition; // Metnin orijinal pozisyonu
    private TMP_Text textComponent;

    private bool isShaking = false; // Sallama durumunu kontrol etmek i�in

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        originalPosition = textComponent.rectTransform.localPosition;
    }

    public void TriggerShake()
    {
        if (!isShaking) // E�er zaten sallama yap�yorsa, yeni bir sallama ba�latma
        {
            StopAllCoroutines(); // Devam eden sallamalar� durdur
            StartCoroutine(ShakeCoroutine());
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        isShaking = true; // Sallama ba�lad�

        float elapsed = 0f;
        float initialShakeMagnitude = shakeMagnitude; // Ba�lang��taki sallama �iddeti

        // �lk ba�taki sallamay� yapal�m
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // Sallama �iddetini azalt
            float currentShakeMagnitude = Mathf.Lerp(initialShakeMagnitude, minShakeMagnitude, elapsed / shakeDuration);

            // Rastgele bir sallama ofseti olu�tur
            Vector3 shakeOffset = new Vector3(
                Random.Range(-currentShakeMagnitude, currentShakeMagnitude),
                Random.Range(-currentShakeMagnitude, currentShakeMagnitude),
                0f
            );

            // Ofseti metin pozisyonuna uygula
            textComponent.rectTransform.localPosition = originalPosition + shakeOffset;

            yield return null;
        }

        // Sallama bitti�inde pozisyonu eski haline getir
        textComponent.rectTransform.localPosition = originalPosition;

        // Sallama tamamland�
        isShaking = false;

        // Yava��a sabit bir seviyeye gelene kadar devam et
        while (!isShaking)
        {
            yield return new WaitForSeconds(0.1f); // Yava��a sallanmas� i�in k���k bir gecikme
            ApplyConstantShake(); // Sabit sallama efektini uygula
        }
    }

    private void ApplyConstantShake()
    {
        // Sabit bir �iddette sallama uygula
        Vector3 shakeOffset = new Vector3(
            Random.Range(-minShakeMagnitude, minShakeMagnitude),
            Random.Range(-minShakeMagnitude, minShakeMagnitude),
            0f
        );

        // Ofseti metin pozisyonuna uygula
        textComponent.rectTransform.localPosition = originalPosition + shakeOffset;
    }
}
