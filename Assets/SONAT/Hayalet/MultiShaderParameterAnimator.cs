using UnityEngine;

public class MultiShaderParameterAnimator : MonoBehaviour
{
    public Material targetMaterial; // Shader parametrelerini kontrol edeceðiniz materyal
    public string olumParameter = "OLUM"; // OLUM parametresinin adý
    public string cutoffHeightParameter = "Cutoff Height"; // Cutoff Height parametresinin adý
    public float olumStartValue = 0f; // OLUM parametresinin baþlangýç deðeri
    public float olumEndValue = 1f; // OLUM parametresinin bitiþ deðeri
    public float cutoffStartValue = -0.5f; // Cutoff Height parametresinin baþlangýç deðeri
    public float cutoffEndValue = 3.13f; // Cutoff Height parametresinin bitiþ deðeri
    public float duration = 1.0f; // Kayma süresi (saniye)

    private float elapsedTime;

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Linear interpolation (Lerp) ile yeni deðerleri hesapla
            float olumValue = Mathf.Lerp(olumStartValue, olumEndValue, elapsedTime / duration);
            float cutoffValue = Mathf.Lerp(cutoffStartValue, cutoffEndValue, elapsedTime / duration);

            // Shader parametrelerini güncelle
            targetMaterial.SetFloat(olumParameter, olumValue);
            targetMaterial.SetFloat(cutoffHeightParameter, cutoffValue);
        }
    }
}
