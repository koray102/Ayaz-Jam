using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LifeTree : MonoBehaviour
{
    [SerializeField] private GameObject dedCanva;
    [SerializeField] private Light directionalLight;
    internal float life = 100;
    private float startLife;
    private Coroutine intensityCoroutine;


    private void Start()
    {
        startLife = life;
    }

    void Update()
    {
        if (life < 0)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            dedCanva.SetActive(true);
        }
    }


    internal void DecreaseLife(int damage)
    {
        life -= damage;

        SetDirectionalLightIntensity();
    }

    private void SetDirectionalLightIntensity()
    {
        float direcionalLightIntensity = (startLife - life) / startLife;
        
        if(intensityCoroutine != null)
        {
            StopCoroutine(intensityCoroutine);
        }

        intensityCoroutine = StartCoroutine(DecreaseIntensity(direcionalLightIntensity * 2.5f));
    }


    private IEnumerator DecreaseIntensity(float targetIntensity)
    {
        float elapsedTime = 0f;
        float volumeIntenisty = 5f;
        
        while (elapsedTime < volumeIntenisty)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / volumeIntenisty; // Zamana bağlı bir interpolasyon faktörü
            float newIntensity;
            newIntensity = Mathf.Lerp(directionalLight.intensity, targetIntensity, t);
            directionalLight.intensity = newIntensity;

            yield return null; // Bir sonraki kareyi bekle
        }

        directionalLight.intensity = targetIntensity;
    }
}
