using UnityEngine;

public class MultiShaderParameterAnimator : MonoBehaviour
{
    public Material targetMaterial; // Shader parametrelerini kontrol edece�iniz materyal
    public string olumParameter = "OLUM"; // OLUM parametresinin ad�
    public string cutoffHeightParameter = "Cutoff Height"; // Cutoff Height parametresinin ad�
    public float olumStartValue = 0f; // OLUM parametresinin ba�lang�� de�eri
    public float olumEndValue = 1f; // OLUM parametresinin biti� de�eri
    public float cutoffStartValue = -0.5f; // Cutoff Height parametresinin ba�lang�� de�eri
    public float cutoffEndValue = 3.13f; // Cutoff Height parametresinin biti� de�eri
    public float duration = 1.0f; // Kayma s�resi (saniye)

    private float elapsedTime;

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Linear interpolation (Lerp) ile yeni de�erleri hesapla
            float olumValue = Mathf.Lerp(olumStartValue, olumEndValue, elapsedTime / duration);
            float cutoffValue = Mathf.Lerp(cutoffStartValue, cutoffEndValue, elapsedTime / duration);

            // Shader parametrelerini g�ncelle
            targetMaterial.SetFloat(olumParameter, olumValue);
            targetMaterial.SetFloat(cutoffHeightParameter, cutoffValue);
        }
    }
}
