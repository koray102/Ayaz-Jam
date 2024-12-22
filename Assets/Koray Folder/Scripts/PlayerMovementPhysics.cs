using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementPhysics : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private Scythe scytheSc;


    [Header("Movement")]
    [SerializeField] private float speedInput;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float stepDistance;
    [SerializeField] private float springStrength;
    [SerializeField] private float springDamper;
    [SerializeField] private float maxFrictionSpeed;
    [SerializeField] private float frictionFactor;
    [SerializeField] private float dragForce;
    public Transform groundCheckTransform;
    public float radiusOfSphere = 0.3f;
    public LayerMask groundMask;


    [Header("Grab")]
    [SerializeField] private float maxGrabDistance = 10;
    [SerializeField] private LayerMask ignoreRaycast;
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float duration;
    [SerializeField] private GameObject hand;
    [SerializeField] private Transform handStartPos;
    [SerializeField] private Cooldown cooldownSc;
    private float grabCoolDown;
    private bool canGrab = true;
    private Coroutine grabCoroutine;
    private Coroutine throwHandCoroutine;
    private Coroutine pullHandCoroutine;
    private quaternion handStartRot;
    private LineRenderer lineRenderer;


    [Header("VFX")]
    [SerializeField] private ParticleSystem windVFX;
    [SerializeField] private ParticleSystem shockVFX;
    [SerializeField] private float windMinVelo;


    [Header("SFX")]
    [SerializeField] private AudioSource grabSFX;
    [SerializeField] private AudioSource powerLandingSFX;


    [Header("Life")]
    [SerializeField] float healtDecreaseCooldown = 0.3f;
    [SerializeField] private List<float> grabCoolDownList = new List<float>{5f, 3f, 0f};
    [SerializeField] private List<float> scytheAttackDuration = new List<float>{1f, 0.7f, 0f};
    [SerializeField] private List<float> maxVeloList = new List<float>{4f, 7f, 13f};
    [SerializeField] private List<GameObject> hearthUI = new List<GameObject>();
    [SerializeField] private GameObject dedCanva;
    [SerializeField] private menuscript menuSc;
    private int health = 3;
    private bool canLoseHealth = true;
    internal bool isDead;


    [Header("Power Landing")]
    [SerializeField] private float minSpeedToLand = 15f;
    [SerializeField] private float damageRange = 8f;
    [SerializeField] private float pointFactor = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private FPSCameraScript fpsCameraScriptSc;
    [SerializeField] private float landShakeDuration = 0.3f; // Titreşim süresi
    [SerializeField] private float landShakeMagnitude = 0.7f; // Titreşim büyüklüğü


    private float restLength;
    private float lastLength;
    private bool grapInput;
    private bool isGrabbing;
    private Vector3 grabPoint;
    private  Vector3 grabLastPosition;
    private GameObject grabbedObject;
    private Rigidbody grabbedRb;
    private float speed;
    private bool isJumping;
    private Rigidbody playerRb;
    private float horizontal;
    private float vertical;
    internal bool isGrounded;
    private Vector3 downDir;


    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // A line needs two points

        handStartRot = hand.transform.localRotation;
        
        SetStats();
    }


    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        grapInput = Input.GetKeyDown(KeyCode.E);

        downDir = transform.up * -1;

        //isGrounded = Physics.CheckSphere(groundCheckTransform.position, radiusOfSphere, groundMask);
      
        if(Input.GetKey(KeyCode.LeftShift))
        {
            speed = isGrounded ? runSpeed : runSpeed / 2;
        }else
        {
            speed = isGrounded ? speedInput : speedInput / 2;
        }
        
        if(Input.GetKeyDown("space") && isGrounded)
        {
            //Debug.Log("jump");
            isJumping = true;
        }

        
        if(grapInput) // Grab acikken kappatmak icin
        {
            ReleaseGrab();
        }


        if(canGrab && !isGrabbing && grapInput && Physics.SphereCast(cam.transform.position, 0.5f, cam.transform.forward, out RaycastHit hit, maxGrabDistance, ~ignoreRaycast))
        {
            if(grabSFX != null)
            {
                grabSFX.PlayOneShot(grabSFX.clip);
            }

            grabbedObject = hit.transform.gameObject;
            grabbedRb = grabbedObject.GetComponent<Rigidbody>();

            if(grabbedObject.CompareTag("Enemy"))
            {
                grabbedObject.GetComponent<Enemy>().isGrabbed = true;
            }

            isGrabbing = true;
            restLength = hit.distance;
            grabPoint = hit.point;
            
            grabCoroutine = StartCoroutine(PullGrab(restLength, 1));

            if(pullHandCoroutine != null)
            {
                StopCoroutine(pullHandCoroutine);
            }

            throwHandCoroutine = StartCoroutine(ThrowHand(grabPoint));

            StartCoroutine(GrabCountDown());
        }


        if(isGrabbing)
        {
            lineRenderer.enabled = true;
            DrawRayLine();
        }else
        {
            hand.transform.localRotation = handStartRot;
            lineRenderer.enabled = false;
        }

        if(playerRb.velocity.magnitude > windMinVelo)
        {
            windVFX.Play();
        }else if(windVFX.isPlaying)
        {
            windVFX.Stop();
        }
    }


    void FixedUpdate()
    {   
        if(Physics.Raycast(groundCheckTransform.position, downDir, out RaycastHit hit, stepDistance, ~ignoreRaycast))
        {
            isGrounded = true;
            
            Float(hit);
        }else
        {
            isGrounded = false;
        }

        RawPhysicalMovement();

        Jump();

        if(isGrabbing)
        {
            Vector3 grabOffset = Vector3.zero;

            // Calculate the direction from pointA to pointB
            Vector3 direction = grabPoint - transform.position;

            // Draw the line as a ray
            Debug.DrawRay(transform.position, direction, Color.green);

            if(grabbedObject != null && grabbedRb != null && !grabbedObject.isStatic)
            {
                Vector3 grabCurrentPosition = grabbedObject.transform.position;

                if(grabLastPosition.magnitude != 0)
                {
                    grabOffset = grabCurrentPosition - grabLastPosition;
                }
                
                grabLastPosition = grabCurrentPosition;

                grabPoint += grabOffset;
            }
            
            SpringSystem(grabPoint);
        }
    }


    private void RawPhysicalMovement()
    {
        Vector3 verticalMoveDirection = transform.forward * vertical;
        Vector3 horizontalMoveDirection = transform.right * horizontal;

        Vector3 totalMove = verticalMoveDirection + horizontalMoveDirection;

        if(totalMove.magnitude > 0)
        {
            playerRb.AddForce(totalMove.normalized * speed);
        }else if(!isGrabbing && isGrounded)
        {
            ApplyFriction();
        }

        playerRb.AddForce(totalMove.normalized * speed, ForceMode.VelocityChange);

        LimitVelocityPhysically();
    }


    private void LimitVelocityPhysically()
    {
        // Get the player's current velocity
        Vector3 velocity = playerRb.velocity;

        // Check if the horizontal speed exceeds the max speed
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            // Apply a counter force proportional to the excess speed
            Vector3 excessVelocity = horizontalVelocity.normalized * (horizontalVelocity.magnitude - maxSpeed);
            playerRb.AddForce(-excessVelocity * dragForce, ForceMode.Acceleration);
        }
    }

    
    private void ApplyFriction()
    {
        Vector3 forwardDir = transform.forward;
        Vector3 rightDir = transform.right;

        Vector3 playerVelocity = playerRb.velocity;

        // Forward/backward friction
        float forwardVel = Vector3.Dot(forwardDir, playerVelocity);
        float desiredForwardFrictionVel = Mathf.Clamp(-forwardVel, -maxFrictionSpeed, maxFrictionSpeed);
        desiredForwardFrictionVel *= frictionFactor;
        float desiredForwardFrictionAccel = desiredForwardFrictionVel / Time.fixedDeltaTime;

        // Right/left friction
        float rightVel = Vector3.Dot(rightDir, playerVelocity);
        float desiredRightFrictionVel = Mathf.Clamp(-rightVel, -maxFrictionSpeed, maxFrictionSpeed);
        desiredRightFrictionVel *= frictionFactor;
        float desiredRightFrictionAccel = desiredRightFrictionVel / Time.fixedDeltaTime;

        // Apply forces for both directions
        playerRb.AddForce(desiredForwardFrictionAccel * forwardDir, ForceMode.Acceleration);
        playerRb.AddForce(desiredRightFrictionAccel * rightDir, ForceMode.Acceleration);
    }


    private void Jump()
    {
        if(isJumping)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = false;
        }
    }


    private void SpringSystem(Vector3 grabPoint)
    {
        float springVelocity = 0;
        Vector3 suspensionForce = Vector3.zero;

        // Damper için yayın hızını hesapla
        float currentSpringLength = Vector3.Distance(transform.position, grabPoint);

        if(lastLength != 0)
        {
            springVelocity = (lastLength - currentSpringLength) / Time.fixedDeltaTime;
        }

        lastLength = currentSpringLength;

        // Yay ve damper kuvvetini hesapla
        float springForce = springStiffness * (restLength - currentSpringLength);
        float damperForce = damperStiffness * springVelocity;
        

        Vector3 springDir = transform.position - grabPoint;
        springDir.Normalize();
            
        if(springForce < 0)
        {
            suspensionForce = (springForce + damperForce) * springDir;
        }

        playerRb.AddForce(suspensionForce);

        if(grabbedRb != null && !grabbedRb.isKinematic)
        {
            grabbedRb.AddForceAtPosition(-suspensionForce, grabPoint);
        }
    }


    internal void ReleaseGrab()
    {
        if(isGrabbing)
        {
            if(grabbedObject.CompareTag("Enemy"))
            {
                grabbedObject.GetComponent<Enemy>().isGrabbed = false;
            }

            if(throwHandCoroutine != null)
            {
                StopCoroutine(throwHandCoroutine);
            }

            pullHandCoroutine = StartCoroutine(PullHand(Vector3.zero));

            StopCoroutine(grabCoroutine);

            grabbedObject = null;
            grabbedRb = null;
            grabLastPosition = Vector3.zero;
            lastLength = 0;

            isGrabbing = false;
            grapInput = false;
        }
    }


    private IEnumerator PullGrab(float currentRestLength, float targetLength)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration; // Zamana bağlı bir interpolasyon faktörü
            float newLength;
            newLength = Mathf.Lerp(currentRestLength, targetLength, t);
            restLength = newLength;

            yield return null; // Bir sonraki kareyi bekle
        }

        restLength = targetLength;
    }


    private IEnumerator ThrowHand(Vector3 targetPos)
    {
        hand.transform.SetParent(grabbedObject.transform);
        targetPos = grabbedObject.transform.InverseTransformPoint(targetPos);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration; // Zamana bağlı bir interpolasyon faktörü
            Vector3 newPosition;
            newPosition = Vector3.Lerp(hand.transform.localPosition, targetPos, t);
            hand.transform.localPosition = newPosition;

            yield return null; // Bir sonraki kareyi bekle
        }

        hand.transform.localPosition = targetPos;
    }

    private IEnumerator PullHand(Vector3 targetPos)
    {
        hand.transform.SetParent(handStartPos);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration; // Zamana bağlı bir interpolasyon faktörü
            Vector3 newPosition;
            newPosition = Vector3.Lerp(hand.transform.position, targetPos, t); // local alirsan duzgun oluyo
            hand.transform.localPosition = newPosition;

            yield return null; // Bir sonraki kareyi bekle
        }

        hand.transform.localPosition = targetPos;
    }


    private void Float(RaycastHit rayHit)
    {
        Vector3 velocity = playerRb.velocity;
        Vector3 rayDir = transform.TransformDirection(downDir);

        Vector3 otherVel = Vector3.zero;
        Rigidbody hitRb = rayHit.rigidbody;

        if(hitRb != null)
        {
            otherVel = hitRb.velocity;
        }

        float velocityY = Vector3.Dot(velocity, rayDir);
        float otherVelocityY = Vector3.Dot(otherVel, rayDir);

        float relVelo = velocityY - otherVelocityY;

        float x = rayHit.distance - stepDistance;

        float springForce = (x * springStrength) - (relVelo * springDamper);

        springForce = Mathf.Clamp(springForce, Mathf.NegativeInfinity, 0);

        playerRb.AddForce(rayDir * springForce);
        
        if(hitRb != null && !hitRb.isKinematic)
        {
            hitRb.AddForceAtPosition(rayDir * -springForce, rayHit.point);
        }
    }


    private void DrawRayLine()
    {
        // Define the start and end points of the line
        Vector3 startPoint = handStartPos.position;
        Vector3 endPoint = grabPoint;

        // Update the LineRenderer positions
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
    

    private IEnumerator healthDecreaseCountDown()
    {
        float time = healtDecreaseCooldown;
        
        while(time > 0)
        {
            time -= Time.deltaTime;

            yield return null;
        }

        canLoseHealth = true;
    }


    private IEnumerator GrabCountDown()
    {
        float time = grabCoolDown;

        canGrab = false;
        
        while(time > 0)
        {
            time -= Time.deltaTime;
            cooldownSc.UpdateCooldownExternally(0, time, grabCoolDown);

            yield return null;
        }

        cooldownSc.UpdateCooldownExternally(0, 0, grabCoolDown);
        canGrab = true;
    }


    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy") && canLoseHealth)
        {
            GameObject currentHearth = hearthUI[3 - health];
            currentHearth.SetActive(false);
            
            health--;

            isDead = health <= 0; // bu neymis la

            if(!isDead)
            {
                SetStats();
                canLoseHealth = false;
                StartCoroutine(healthDecreaseCountDown());
            }else
            {
                Time.timeScale = 0f;
                dedCanva.SetActive(true);
            }
        }

        if(other.gameObject.CompareTag("Ground"))
        {
            if(powerLandingSFX != null)
            {
                powerLandingSFX.PlayOneShot(powerLandingSFX.clip);
            }

            PowerLanding();
        }
    }


    private void PowerLanding()
    {
        shockVFX.Play();
        float velocity = playerRb.velocity.magnitude;

        if(isGrabbing && velocity > minSpeedToLand)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, damageRange, enemyLayer);
            StartCoroutine(fpsCameraScriptSc.Shake(landShakeDuration, landShakeMagnitude));

            // Bulunan Collider'ların GameObject'lerini listeye ekle
            foreach (Collider collider in colliders)
            {
                Enemy enemySc = collider.GetComponent<Enemy>();
                
                scytheSc.SetCombo();
                enemySc.Die(pointFactor);
            }
        }
    }


    private void SetStats()
    {
        // sondan basladigi icin 3ten cikar
        maxSpeed = maxVeloList[3 - health];
        grabCoolDown = grabCoolDownList[3 - health];
        scytheSc.duration = scytheAttackDuration[3 - health];
    }
}
