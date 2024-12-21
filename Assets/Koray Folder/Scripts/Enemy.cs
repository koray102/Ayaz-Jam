using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float point = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float chasingMaxSpeed = 5f;
    [SerializeField] private float destroyDuration = 2f;
    [SerializeField] private float maxChaseDistance = 30f;
    [SerializeField] private float reachToTreeDistance = 2f;
    [SerializeField] private GameObject tree;
    [SerializeField] private UIManager uIManagerSc;
    private PlayerMovementPhysics playerMovementPhysicsSc;
    private float maxSpeed;
    internal bool isDied;
    internal bool isGrabbed;
    private GameObject player;
    private Rigidbody rb;
    private Vector3 targetPosition;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        playerMovementPhysicsSc = player.GetComponent<PlayerMovementPhysics>();
    }


    void Update()
    {
        if(!isDied)
        {
            // Hedefe doğru bak
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
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
        
        maxSpeed = chasingMaxSpeed;

        if(Vector3.Distance(transform.position, player.transform.position) < maxChaseDistance) // oyuncuyu görüyorsa
        {
            targetPosition = player.transform.position;
        }else
        {
            Debug.Log("tree");
            targetPosition = tree.transform.position;
        }

        direction = targetPosition - transform.position;
        direction.y = 0;
        direction.Normalize();

        if(rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(direction * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
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

        uIManagerSc.point += point * comboFactor;

        //animasyon vfx falan oynatılacak

        Destroy(gameObject, destroyDuration);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //oyuncuya degerse ne olcak
        }
    }

}
