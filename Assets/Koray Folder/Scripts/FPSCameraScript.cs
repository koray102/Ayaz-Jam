using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCameraScript : MonoBehaviour
{
    [SerializeField] private PlayerMovementPhysics playerMovementPhysicsSc;
    private float shakeMagModified;
    private Vector3 originalPosition; // Kameranın orijinal pozisyonu
    public float sensitivity = 150f;
    public Transform playerTransform;
    private float xRotation;
    private float yRotation;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        // Keep the cursor at the middle of the screen and make is invisible.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalPosition = transform.localPosition;
    }

    
    void Update()
    {
        // Get the mouse movements which made at x and y axes.
        mouseX = Input.GetAxisRaw("Mouse X") *Time.deltaTime * sensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") *Time.deltaTime * sensitivity;

        // Keep the mouse movement which made at y axis between predetermined values.
        // We have to assign "mouseY" variable to another variable because the "Mouse Y" input does not take values as we are used to.
        xRotation -= mouseY; // If you put plus instead of minus the character will look the other way.
        xRotation = Mathf.Clamp(xRotation , -90f, 90f);

        // Rotate the camera according to mouse movement which made at y axis.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotate the players body according to movement of mouse which made at x axis.
        playerTransform.Rotate(Vector3.up * mouseX);

    }


    internal IEnumerator Shake(float shakeDuration, float shakeMagnitude)
    {
        float elapsed = 0f;
        shakeMagModified = playerMovementPhysicsSc.isGrounded? shakeMagnitude : shakeMagnitude * 2f;

        while (elapsed < shakeDuration)
        {
            // Rastgele bir pozisyon hesapla
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagModified;
            transform.localPosition = originalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Titreşim bittikten sonra kamerayı orijinal pozisyona getir
        transform.localPosition = originalPosition;
    }
}
