using UnityEngine;

public class OlumAnimator : MonoBehaviour
{
    private Material material; // Objeye atanmýþ materyal
    private string cutoffParameterName = "_Cutoff"; // Cutoff parametresinin adý
    private string olumParameterName = "_OLUM";    // OLUM parametresinin adý

    private void Start()
    {
        // Objeye atanmýþ materyali al
        material = GetComponent<Renderer>().material;
    }
    private void Update()
    {

        //AnimateCutoffAndOLUM(0f, 5f, 0, 1, 2f);

    }
    public void AnimateCutoffAndOLUM(float cutoffStart, float cutoffEnd, float olumStart, float olumEnd, float duration)
    {
        StartCoroutine(AnimateParameters(cutoffStart, cutoffEnd, olumStart, olumEnd, duration));
    }

    private System.Collections.IEnumerator AnimateParameters(float cutoffStart, float cutoffEnd, float olumStart, float olumEnd, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Cutoff parametresini animasyonla deðiþtir
            float newCutoffValue = Mathf.Lerp(cutoffStart, cutoffEnd, t);
            if (material != null)
            {
                material.SetFloat(cutoffParameterName, newCutoffValue);
            }

            // OLUM parametresini animasyonla deðiþtir
            float newOlumValue = Mathf.Lerp(olumStart, olumEnd, t);
            if (material != null)
            {
                material.SetFloat(olumParameterName, newOlumValue);
            }

            yield return null; // Bir sonraki frame'e geç
        }

        // Animasyon bitiminde kesin deðerleri ayarla
        if (material != null)
        {
            material.SetFloat(cutoffParameterName, cutoffEnd);
            material.SetFloat(olumParameterName, olumEnd);
        }
    }
}
