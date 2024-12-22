using UnityEngine;
using TMPro;
using System.Collections;

public class komboscript : MonoBehaviour
{
    public TMP_Text comboLevelText; // Kombo seviyesini g�sterecek UI Text
    public ShakeEffect shakeEffect; // Sallama efektini y�netecek script

    public float transitionDuration = 0.5f; // Harfin gelme s�resi
    public Vector2 spawnOffsetRange = new Vector2(100f, 100f); // Rastgele ba�lang�� pozisyonu aral���

    private string[] comboLevels = { "D", "C", "B", "A", "S" }; // Kombo seviyeleri
    private int currentComboIndex = 0; // �u anki seviye indeksi
    private Vector3 originalPosition;

    private void Awake()
    {
        // Orijinal pozisyonu kaydet
        originalPosition = comboLevelText.rectTransform.localPosition;
    }


    internal void UpdateComboDisplay(int currenComboLVL)
    {
        // Mevcut kombo seviyesini g�ster ve animasyonu tetikle
        string newComboLevel = comboLevels[currenComboLVL];
        StartCoroutine(AnimateComboChange(newComboLevel));
    }

    private IEnumerator AnimateComboChange(string newComboLevel)
    {
        // Rastgele bir ba�lang�� pozisyonu se�
        Vector3 randomStartOffset = new Vector3(
            Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
            Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y),
            0f
        );

        Vector3 startPosition = originalPosition + randomStartOffset;
        comboLevelText.rectTransform.localPosition = startPosition;

        // Harfi hemen g�ncelle
        comboLevelText.text = newComboLevel;

        // Lineer olarak harfi orijinal pozisyona ta��rken animasyon yap
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
