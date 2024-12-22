using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float startSpawnRate = 3f;
    [SerializeField] private float desiaredSpawnRate = 0.7f;
    [SerializeField] private float duration = 180f;
    private float elapsedTime = 0f; // Geçen süre
    private float spawnTime;


    void Start()
    {
        spawnTime = startSpawnRate;

        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsedTime < duration)
        {
            // Zamanı güncelle
            elapsedTime += Time.deltaTime;

            // Lineer interpolasyon ile değeri azalt
            spawnTime = Mathf.Lerp(startSpawnRate, desiaredSpawnRate, elapsedTime / duration);
        }else
        {
            // Süre tamamlandığında tam hedef değere ulaş
            spawnTime = desiaredSpawnRate;
        }
    }


    private IEnumerator Spawn()
    {
        while(true)
        {
            Transform randomTransform = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];

            Instantiate(ghostPrefab, randomTransform.position, Quaternion.identity);

            yield return new WaitForSeconds(spawnTime);
        }
    }
}
