using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyYourself : MonoBehaviour
{
    [SerializeField] AudioClip clip;


    void Start()
    {
        Destroy(gameObject, clip.length + 0.5f);
    }
}
