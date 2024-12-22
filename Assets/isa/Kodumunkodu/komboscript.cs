using UnityEngine;
using TMPro;
using System.Collections;

public class komboscript : MonoBehaviour
{
    public TMP_Text comboLevelText; // Kombo seviyesini gösterecek UI Text
    public ShakeEffect shakeEffect; // Sallama efektini yönetecek script

    public float transitionDuration = 0.5f; // Harfin gelme süresi
    public Vector2 spawnOffsetRange = new Vector2(100f, 100f); // Rastgele baþlangýç pozisyonu aralýðý

    private string[] comboLevels = { "D", "C", "B", "A", "S" }; // Kombo seviyeleri
    private int currentComboIndex = 0; // Þu anki seviye indeksi
    private Vector3 originalPosition;

    private void Start()
    {
        // Orijinal pozisyonu kaydet
        originalPosition = comboLevelText.rectTransform.localPosition;

        // Ýlk komboyu göster
        UpdateComboDisplay();
    }

    private void Update()
    {
        // T tuþuna basarak kombo seviyesini artýr
        if (Input.GetKeyDown(KeyCode.T))
        {
            IncreaseComboLevel();
        }
    }

    public void IncreaseComboLevel()
    {
        // Maksimum seviyeyi kontrol et
        if (currentComboIndex < comboLevels.Length - 1)
        {
            currentComboIndex++;
            UpdateComboDisplay();
        }
    }

    private void UpdateComboDisplay()
    {
        // Mevcut kombo seviyesini göster ve animasyonu tetikle
        string newComboLevel = comboLevels[currentComboIndex];
        StartCoroutine(AnimateComboChange(newComboLevel));
    }

    private IEnumerator AnimateComboChange(string newComboLevel)
    {
        // Rastgele bir baþlangýç pozisyonu seç
        Vector3 randomStartOffset = new Vector3(
            Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
            Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y),
            0f
        );

        Vector3 startPosition = originalPosition + randomStartOffset;
        comboLevelText.rectTransform.localPosition = startPosition;

        // Harfi hemen güncelle
        comboLevelText.text = newComboLevel;

        // Lineer olarak harfi orijinal pozisyona taþýrken animasyon yap
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            comboLevelText.rectTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsed / transitionDuration);
            yield return null;
        }

        // Pozisyonu orijinal haline getir
        comboLevelText.rectTransform.localPosition = originalPosition;

        // Sallama efektini tetikle
        if (shakeEffect != null)
        {
            shakeEffect.TriggerShake();
        }
    }
}
