using UnityEngine;

public class CutoffAnimator : MonoBehaviour
{
    private Material material; // Objeye atanm�� materyal
    private string cutoffParameterName = "_Cutoff"; // Shader Graph'deki parametre ad�

    private void Start()
    {
        // Objeye atanm�� materyali al
        material = GetComponent<Renderer>().material;

        AnimateCutoff(5f,0f,3f);
    }

    public void AnimateCutoff(float startValue, float endValue, float duration)
    {
        StartCoroutine(AnimateCutoffCoroutine(startValue, endValue, duration));
    }

    private System.Collections.IEnumerator AnimateCutoffCoroutine(float startValue, float endValue, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float newValue = Mathf.Lerp(startValue, endValue, t);

            if (material != null)
            {
                material.SetFloat(cutoffParameterName, newValue);
            }

            yield return null; // Bir sonraki frame'e ge�
        }

        // Biti� de�erini kesin olarak ayarla
        if (material != null)
        {
            material.SetFloat(cutoffParameterName, endValue);
        }
    }
}
