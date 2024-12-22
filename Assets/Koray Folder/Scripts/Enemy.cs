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
    [SerializeField] private UIManager uIManagerSc;
    private PlayerMovementPhysics playerMovementPhysicsSc;
    internal bool isDied;
    internal bool isGrabbed;
    private GameObject player;
    private Rigidbody rb;
    private Vector3 targetPosition;
    private float currentPoint;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        playerMovementPhysicsSc = player.GetComponent<PlayerMovementPhysics>();
        
        tree = GameObject.Find("LifeTree");
        uIManagerSc = GameObject.FindGameObjectWithTag("UIMANAGER").GetComponent<UIManager>();
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

        uIManagerSc.point += currentPoint * comboFactor;

        //animasyon vfx falan oynatılacak

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


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //oyuncuya degerse ne olcak
        }
    }

}
