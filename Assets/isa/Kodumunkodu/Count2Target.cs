using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class Count2Target : MonoBehaviour
{
    public float countDuration = 1f; // Duration for the count
    public float shakeMagnitude = 10f; // Magnitude of the visual shake
    public float puan; // Amount to add
    public TMP_Text skor; // Reference to the score text

    private float currentValue = 0f, targetValue = 0f;
    private Coroutine _C2T;
    private Vector3 originalPosition;

    void Start()
    {
        currentValue = float.Parse(skor.text);
        targetValue = currentValue;
        originalPosition = skor.rectTransform.localPosition;
    }

    private void Update()
    {
        AddValue(puan);
    }

    IEnumerator CountTo(float targetValue)
    {
        var rate = Mathf.Abs(targetValue - currentValue) / countDuration;

        while (currentValue != targetValue)
        {
            currentValue = Mathf.MoveTowards(currentValue, targetValue, rate * Time.deltaTime);

            // Apply a visual shake effect to the position of the text
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeMagnitude, shakeMagnitude),
                Random.Range(-shakeMagnitude, shakeMagnitude),
                0f
            );
            skor.rectTransform.localPosition = originalPosition + shakeOffset;

            // Update the displayed value
            skor.text = ((int)currentValue).ToString();

            yield return null;
        }

        // Reset position and set final value once counting is complete
        skor.rectTransform.localPosition = originalPosition;
        skor.text = ((int)currentValue).ToString();
    }

    public void AddValue(float value)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            targetValue += puan;

            if (_C2T != null)
                StopCoroutine(_C2T);

            _C2T = StartCoroutine(CountTo(targetValue));
        }
    }

    public void SetTarget(float target)
    {
        targetValue = target;

        if (_C2T != null)
            StopCoroutine(_C2T);

        _C2T = StartCoroutine(CountTo(targetValue));
    }
}
