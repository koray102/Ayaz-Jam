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
    [SerializeField] private bool isHunter;
    [SerializeField] private float dashCoolDown = 2f;
    [SerializeField] private GameObject dieSFX;
    private bool canDash = true;
    private bool isTargetPlayer;
    //[SerializeField] private MultiShaderParameterAnimator dissolveSc;
    private PlayerMovementPhysics playerMovementPhysicsSc;
    internal bool isDied;
    internal bool isGrabbed;
    private GameObject player;
    private Rigidbody rb;
    private Vector3 targetPosition;
    private float currentPoint;
    private Vector3 direction;



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

        if(Vector3.Distance(transform.position, player.transform.position) < maxChaseDistance) // oyuncuyu görüyorsa
        {
            isTargetPlayer = true;
            targetPosition = player.transform.position;
        }else
        {
            isTargetPlayer = false;
            targetPosition = tree.transform.position;
        }

        direction = targetPosition - transform.position;
        direction.y = 0;

        if(!isHunter)
        {
            rb.AddForce(direction.normalized * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }else
        {
            if(isTargetPlayer)
            {
                if(canDash)
                {
                    StartCoroutine(DashCountDown());
                }
            }else
            { 
                rb.AddForce(direction.normalized * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
        
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
        
        if(dieSFX != null)
        {
            Instantiate(dieSFX, transform.position, Quaternion.identity);
        }

        if(isGrabbed)
        {
            playerMovementPhysicsSc.ReleaseGrab();
        }

        uIManagerSc.AddValue(currentPoint * comboFactor);

        //animasyon vfx falan oynatılacak
        //StartCoroutine(dissolveSc.DieDissolve());

        Destroy(gameObject, destroyDuration);
       
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


    private IEnumerator DashCountDown()
    {
        float time = dashCoolDown;

        canDash = false;
        
        while(time > 0)
        {
            time -= Time.deltaTime;

            yield return null;
        }

        rb.AddForce(direction.normalized * moveSpeed * 2f, ForceMode.Impulse);
        canDash = true;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //oyuncuya degerse ne olcak
        }

        if (collision.gameObject.CompareTag("LifeTree"))
        {
            Debug.Log("AGAC");
        }
    }

}
