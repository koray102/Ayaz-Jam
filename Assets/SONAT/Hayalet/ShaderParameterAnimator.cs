using UnityEngine;

public class ShaderParameterAnimator : MonoBehaviour
{
    public Material targetMaterial; // Cutoff Height parametresini kontrol edeceðiniz materyal
    public string parameterName = "Cutoff Height"; // Shader parametresinin adý
    public float startValue = 3.14f; // Baþlangýç deðeri
    public float endValue = -0.5f; // Bitiþ deðeri
    public float duration = 1.0f; // Kayma süresi (saniye)

    private float elapsedTime;

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Linear interpolation (Lerp) ile deðer hesaplama
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);

            // Shader parametresini ayarla
            targetMaterial.SetFloat(parameterName, newValue);
        }
    }
}

