using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Scythe : MonoBehaviour
{
    [SerializeField] private float duration = 1f; // Degrees per second
    [SerializeField] private GameObject scythe; // Degrees per second
    [SerializeField] private bool loopRotation = false;
    private bool rotationDone = true;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Attack();

            if(rotationDone)
            {
                RotateObject();
            }
        }

    }


    private void Attack()
    {
        foreach (GameObject enemy in enemiesInRange)
        {
            // Burda enemynin icindeki bi fonksiyonu cagirirsin o hem vfx oynatir sonra da yok eder
            Destroy(enemy);
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
