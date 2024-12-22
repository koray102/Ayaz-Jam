using UnityEngine;

public class ShaderParameterAnimator : MonoBehaviour
{
    public Material targetMaterial; // Cutoff Height parametresini kontrol edece�iniz materyal
    public string parameterName = "Cutoff Height"; // Shader parametresinin ad�
    public float startValue = 3.14f; // Ba�lang�� de�eri
    public float endValue = -0.5f; // Biti� de�eri
    public float duration = 1.0f; // Kayma s�resi (saniye)

    private float elapsedTime;

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Linear interpolation (Lerp) ile de�er hesaplama
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);

            // Shader parametresini ayarla
            targetMaterial.SetFloat(parameterName, newValue);
        }
    }
}

