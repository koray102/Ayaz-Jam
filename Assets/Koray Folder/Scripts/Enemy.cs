using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float point = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float dragForce = 5f;
    [SerializeField] private float destroyDuration = 2f;
    [SerializeField] private float maxChaseDistance = 30f;
    [SerializeField] private float reachToTreeDistance = 2f;
    [SerializeField] private GameObject tree;
    [SerializeField] private Count2Target uIManagerSc;
    private PlayerMovementPhysics playerMovementPhysicsSc;
    internal bool isDied;
    internal bool isGrabbed;
    private GameObject player;
    private Rigidbody rb;
    private Vector3 targetPosition;
    private float currentPoint;

    public Material targetMaterial; // Cutoff Height parametresini kontrol edeceğiniz materyal
    public string cutoffHeightParameter = "_Cutoff"; // Shader parametresinin adı
    public string olumParameter = "_OLUM";
    public float startValue = 3.14f; // Başlangıç değeri
    public float endValue = -0.5f; // Bitiş değeri
    public float duration = 1.0f; // Kayma süresi (saniye)

    private float elapsedTime;

    public float olumStartValue = 0f; // OLUM parametresinin başlangıç değeri
    public float olumEndValue = 1f; // OLUM parametresinin bitiş değeri
    public float cutoffStartValue = -0.5f; // Cutoff Height parametresinin başlangıç değeri
    public float cutoffEndValue = 3.13f; // Cutoff Height parametresinin bitiş değeri


    void Awake()
    {
        // Bu obje üzerindeki Renderer bileşeninden materyali al
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            targetMaterial = renderer.material;
        }
        else
        {
            Debug.LogError("Renderer bulunamadı! Bu script bir objeye Renderer bileşeniyle birlikte eklenmelidir.");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        playerMovementPhysicsSc = player.GetComponent<PlayerMovementPhysics>();
        
        tree = GameObject.Find("LifeTree");
        uIManagerSc = GameObject.FindGameObjectWithTag("UIManager").GetComponent<Count2Target>();

    }


    void Update()
    {
        if(!isDied)
        {
            // Hedefe doğru bak
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
        }

        currentPoint = isGrabbed? point * 5f : point;



        if (elapsedTime < duration && isDied)
        {
            elapsedTime += Time.deltaTime;

            // Linear interpolation (Lerp) ile yeni değerleri hesapla
            float olumValue = Mathf.Lerp(olumStartValue, olumEndValue, elapsedTime / duration);
            float cutoffValue = Mathf.Lerp(cutoffStartValue, cutoffEndValue, elapsedTime / duration);

            if(olumValue == olumEndValue)
            {
                Destroy(gameObject);
            }
            // Shader parametrelerini güncelle
            Debug.Log(targetMaterial.ToString());
            targetMaterial.SetFloat(olumParameter, olumValue);
            targetMaterial.SetFloat(cutoffHeightParameter, cutoffValue);
        }
        

    }


    void FixedUpdate()
    {
        if(!isDied)
        {
            if(player != null)
            {
                PhysicalMove();
            }
        }
    }


    private void PhysicalMove()
    {
        Vector3 direction;

        if(Vector3.Distance(transform.position, player.transform.position) < maxChaseDistance) // oyuncuyu görüyorsa
        {
            targetPosition = player.transform.position;
        }else
        {
            targetPosition = tree.transform.position;
        }

        direction = targetPosition - transform.position;
        direction.y = 0;

        rb.AddForce(direction.normalized * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        
        if(!isGrabbed)
        {
            LimitVelocityPhysically();
        }
    }


    private bool CheckReachedTarget(Vector3 targetPoint)
    {
        return Vector3.Distance(transform.position, targetPoint) < reachToTreeDistance;
    }


    internal void Die(float comboFactor)
    {
        isDied = true;
        if(isGrabbed)
        {
            playerMovementPhysicsSc.ReleaseGrab();
        }

        uIManagerSc.AddValue(currentPoint * comboFactor);

        //animasyon vfx falan oynatılacak

       
    }


    private void LimitVelocityPhysically()
    {
        // Get the player's current velocity
        Vector3 velocity = rb.velocity;

        // Check if the horizontal speed exceeds the max speed
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            // Apply a counter force proportional to the excess speed
            Vector3 excessVelocity = horizontalVelocity.normalized * (horizontalVelocity.magnitude - maxSpeed);
            rb.AddForce(-excessVelocity * dragForce, ForceMode.Acceleration);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //oyuncuya degerse ne olcak
        }
    }

}
