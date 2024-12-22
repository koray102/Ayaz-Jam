using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTree : MonoBehaviour
{
    internal int life = 100;
    [SerializeField] private GameObject dedCanva;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (life < 0)
        {
            Time.timeScale = 0f;
            dedCanva.SetActive(true);
        }
    }


    internal void decreaseLife(int damage)
    {
        life -= damage;
    }
}
