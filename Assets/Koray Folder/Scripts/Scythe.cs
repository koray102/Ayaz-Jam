using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using System;

public class Scythe : MonoBehaviour
{
    [Header("Cam Shake")]
    [SerializeField] private float shakeDuration = 0.1f; // Titreşim süresi
    [SerializeField] private float shakeMagnitude = 0.25f; // Titreşim büyüklüğü
    

    [SerializeField] internal float duration = 1f; // Degrees per second
    [SerializeField] private float oyuncuDostuZaman = 0.5f; // Degrees per second
    [SerializeField] private GameObject scythe; // Degrees per second
    [SerializeField] private bool loopRotation = false;
    [SerializeField] private float comboResetTime;
    [SerializeField] private float zombieResetTime;
    [SerializeField] private int comboUpTreshold;
    [SerializeField] private List<float> comboFactors = new List<float>{1f, 2f, 4f, 10f, 20f};
    [SerializeField] private int maxComboLevel;
    [SerializeField] private Cooldown cooldownScS;
    [SerializeField] private FPSCameraScript fpsCameraScriptSc;
    [SerializeField] private Volume rageVolume;
    private Coroutine volumeCoroutine;
    private Coroutine camShakeCoroutine;
    private int killedZombies = 0;
    private Coroutine zombieCountDownCoroutine;
    private int currentComboLevel = 0;
    private Coroutine comboCountDownCoroutine;
    private bool rotationDone = true;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && rotationDone)
        {
            StartCoroutine(Attack());

            RotateObject();
        }

        //UIManagerSc.comboLevel = currentComboLevel;
    }


    private IEnumerator Attack()
    {
        float elapsedTime = 0f;

        while (elapsedTime < oyuncuDostuZaman)
        {
            elapsedTime += Time.deltaTime;

            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                GameObject enemy = enemiesInRange[i];
                if (enemy != null)
                {
                    killedZombies++;

                    SetCombo();

                    if(camShakeCoroutine != null)
                    {
                        StopCoroutine(camShakeCoroutine);
                    }
                    
                    camShakeCoroutine = StartCoroutine(fpsCameraScriptSc.Shake(shakeDuration, shakeMagnitude));

                    enemy.GetComponent<Enemy>().Die(comboFactors[currentComboLevel]); // Enemy'nin fonksiyonunu çağır
                    enemiesInRange.RemoveAt(i); // Orijinal koleksiyondan kaldır
                }
            }

            yield return null; // Bir sonraki kareyi bekle
        }
    }


    private void RotateObject()
    {
        rotationDone = false;

        scythe.transform.DOLocalRotate(Vector3.right * 360f, duration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutCubic) // Linear motion for consistent speed
            .SetLoops(loopRotation ? -1 : 1, LoopType.Restart)
            .OnComplete(() =>
            {
                rotationDone = true;
            });
    }

    
    internal void SetCombo()
    {
        if(comboCountDownCoroutine != null)
        {
            StopCoroutine(comboCountDownCoroutine);
        }

        if(killedZombies >= comboUpTreshold) // kestigi zombi kombo artirmaya yetiyosa
        {
            killedZombies = 0;

            comboCountDownCoroutine = StartCoroutine(comboCountDown());

            if(currentComboLevel < maxComboLevel)
            {
                currentComboLevel++;
                SetRageVolume();
            }
        }else // kestigi zombi daha yetmiyosa geri sayimi baslat
        {
            if(zombieCountDownCoroutine != null)
            {
                StopCoroutine(zombieCountDownCoroutine);
            }
            zombieCountDownCoroutine = StartCoroutine(zombieCountDown());

            if(currentComboLevel != 0)
            {
                comboCountDownCoroutine = StartCoroutine(comboCountDown());
            }
        }
    }


    private IEnumerator zombieCountDown()
    {
        float time = zombieResetTime;
        
        while(time > 0)
        {
            time -= Time.deltaTime;
           // UIManagerSc.zombieCoolDown = time;

            yield return null;
        }
      //  UIManagerSc.zombieCoolDown = 0;

        killedZombies = 0;
    }


    private IEnumerator comboCountDown()
    {
        float time = comboResetTime;
        
        while(time > 0)
        {
            time -= Time.deltaTime;
          //  UIManagerSc.comboCoolDown = time;

            yield return null;
        }
       // UIManagerSc.comboCoolDown = 0;

        if(currentComboLevel != 0)
        {
            comboCountDownCoroutine = StartCoroutine(comboCountDown());
            currentComboLevel--;
            SetRageVolume();
        }
    }


    private void SetRageVolume()
    {
        float rageVolumeIntensity = currentComboLevel / (float)maxComboLevel;
        
        if(volumeCoroutine != null)
        {
            StopCoroutine(volumeCoroutine);
        }

        volumeCoroutine = StartCoroutine(IncreaseIntensity(rageVolumeIntensity));
    }


    private IEnumerator IncreaseIntensity(float targetIntensity)
    {
        float elapsedTime = 0f;
        float volumeIntenisty = 3f;
        
        while (elapsedTime < volumeIntenisty)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / volumeIntenisty; // Zamana bağlı bir interpolasyon faktörü
            float newIntensity;
            newIntensity = Mathf.Lerp(rageVolume.weight, targetIntensity, t);
            rageVolume.weight = newIntensity;

            yield return null; // Bir sonraki kareyi bekle
        }

        rageVolume.weight = targetIntensity;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}
