using System;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class KartController : NetworkBehaviour
{
    [SerializeField] private Rigidbody sphere;

    private bool OnGround;
    float speed, currentSpeed;
    float rotate, currentRotate;
    int driftDirection;
    float driftPower;
    int driftMode = 0;
    private float steering;
    private float airSpin;
    private bool canDrift;
    private bool isDrifting;
    private bool startDrift;
    private driftDir currentDriftDir;
    private bool isBoosting;
    private bool isJumping;
    private bool hasJumped;
    private bool jumpPressed;
    private float currentSpinSpeed;
    private Vector3 camVelocity;

    enum driftDir
    {
        left,
        right,
        none
    }
    

    [Header("Controls")]
    
    public float maxSpeed = 30f;
    public float boostSpeed = 60f;
    public float boostImpulse = 60f;
    public float acceleration = 12f;
    
    public float groundSteering = 80f;
    public float airSteering = 80f;
    
    public float gravity = 10f;
    [SerializeField] private float jumpForce = 1;
    [SerializeField] private float spinSpeed = 70f;
    [SerializeField] private float raycastDistance;
    
    [Header("Drift")]
    
    [SerializeField] private float driftAngle;
    [SerializeField] private float driftBaseSteering;
    [SerializeField] private float driftMaxSteering;
    [SerializeField] private float driftMinSteering;
    
    
    [Header("visuals")]
    
    [SerializeField] private float wheelsRotationAmount;
    public float carOffset;
    public float wheelRotationMult = 1;
    [SerializeField] private float camLerpPos;
    [SerializeField] private float camLerpRot;

    [Header("references")]
    
    public Transform kartNormal;
    public Transform kartModel;
    public Transform camPivot;
    [SerializeField] private GameObject frontLeftWheel;
    [SerializeField] private GameObject frontLeftWheelPivot;
    [SerializeField] private GameObject frontRightWheel;
    [SerializeField] private GameObject frontRightWheelPivot;
    [SerializeField] private GameObject backLeftWheel;
    [SerializeField] private GameObject backRightWheel;
    [SerializeField] private TMP_Text speedCounter;

    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        CameraManager.INSTANCE.gameObject.transform.SetParent(camPivot);
        steering = groundSteering;
    }

    private void LateUpdate()
    {
        camPivot.transform.position = Vector3.SmoothDamp(
            camPivot.transform.position,
            kartModel.transform.position,
            ref camVelocity,
            camLerpPos);
        camPivot.transform.rotation = Quaternion.Lerp(
            camPivot.transform.rotation, 
            Quaternion.Euler(0, kartModel.eulerAngles.y, 0),
            camLerpRot);
        
        
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        transform.position = sphere.transform.position - new Vector3(0,carOffset,0);
        
        if (Input.GetKey(KeyCode.W) || Input.GetButton("Fire1"))
        {
            if (isBoosting)
            {
                speed = boostSpeed;
            }
            else
            {
                speed = maxSpeed;
                
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isBoosting = true;
            Boost();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isBoosting = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) || (Input.GetAxisRaw("TriggerRight") == 1 && !jumpPressed))
        {
            jumpPressed = true;
            if (OnGround)
            {
                hasJumped = true;
                float dir = Input.GetAxis("Horizontal");
                sphere.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                airSpin = 70f * dir;
                currentSpinSpeed = Mathf.Abs(dir);
                if (dir < -0.1f)
                {
                    currentDriftDir = driftDir.left;
                }
                else if (dir > 0.1f)
                {
                    currentDriftDir = driftDir.right;
                }
                else
                {
                    currentDriftDir = driftDir.none;
                }
            }
            canDrift = true;
        }

        if (Input.GetKeyUp(KeyCode.Space) || (Input.GetAxisRaw("TriggerRight") == 0 && jumpPressed))
        {
            jumpPressed = false;
            canDrift = false;
            if (isDrifting)
            {
                Boost();
            }

            currentDriftDir = driftDir.none;
            isDrifting = false;
        }
        
        Steering();
        
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * acceleration);
        speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;
        
        
    }

    private void FixedUpdate()
    {
        
        if (!IsOwner)
        {
            return;
        }
        
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out hitOn, 1.1f);

        //Normal Rotation
        OnGround = Physics.Raycast(transform.position + (transform.up * .1f), -kartModel.transform.up, out hitNear, raycastDistance);
        
        if (!OnGround && Mathf.Abs(airSpin) > 0.1f && hasJumped)
        {
            float spinStep = spinSpeed * currentSpinSpeed * Time.deltaTime;
            float spinAmount = Mathf.Min(spinStep, Mathf.Abs(airSpin)) * Mathf.Sign(airSpin);
            kartModel.Rotate(Vector3.up, spinAmount, Space.Self);
            airSpin -= spinAmount;
        }
        
        if (OnGround)
        {
            if (isJumping)
            {
                isJumping = false;
                hasJumped = false;
            }
            if (canDrift && !isDrifting && startDrift)
            {
                
                startDrift = false;
                canDrift = false;
                isDrifting = true;

                if (currentDriftDir == driftDir.none)
                {
                    float dir = Input.GetAxis("Horizontal");
                    if (dir < -0.1f)
                    {
                        currentDriftDir = driftDir.left;
                    }
                    else if (dir > 0.1f)
                    {
                        currentDriftDir = driftDir.right;
                    }
                    else
                    {
                        currentDriftDir = driftDir.none;
                    }
                }
                //Debug.Log(currentDriftDir);
            }
            steering = groundSteering;

            //Debug.Log(currentDriftDir);
            if (isDrifting && currentDriftDir != driftDir.none)
            {
                if (currentDriftDir == driftDir.left)
                {
                    Vector3 angle = Vector3.Lerp(kartModel.transform.forward, kartModel.transform.right, driftAngle).normalized;
                
                    sphere.AddForce(angle * currentSpeed, ForceMode.Acceleration);
                }
                else if (currentDriftDir == driftDir.right)
                {
                    Vector3 angle = Vector3.Lerp(kartModel.transform.forward, -kartModel.transform.right, driftAngle).normalized;
                
                    sphere.AddForce(angle * currentSpeed, ForceMode.Acceleration);
                }
                
            }
            else
            {
                sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);
            }
            
            
        }
        else
        {
            if (hasJumped) isJumping = true;
            if (canDrift) startDrift = true;
            steering = airSteering;
        }
        
        
        
        wheelRotation(frontLeftWheel, true, frontLeftWheelPivot);
        wheelRotation(frontRightWheel, true, frontRightWheelPivot);
        wheelRotation(backLeftWheel);
        wheelRotation(backRightWheel);

        
        
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        float grav = sphere.linearVelocity.y;

        Vector3 Velocity = new Vector3(sphere.linearVelocity.x, 0f, sphere.linearVelocity.z);
        if (OnGround)
        {
            if (isDrifting && currentDriftDir!=driftDir.none)
            {
                if (currentDriftDir == driftDir.left)
                {
                    Vector3 angle = Vector3.Lerp(kartModel.transform.forward, kartModel.transform.right, driftAngle).normalized;
                    Velocity = angle * Velocity.magnitude;
                }
                else
                {
                    Vector3 angle = Vector3.Lerp(kartModel.transform.forward, -kartModel.transform.right, driftAngle).normalized;
                    Velocity = angle * Velocity.magnitude;
                }
                
            }
            else
            {
                Velocity = kartModel.transform.forward.normalized * Velocity.magnitude;
                
            }
        }

        sphere.linearVelocity = new Vector3(Velocity.x, grav, Velocity.z);

        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
            new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);
        
        
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitOn.normal, Time.deltaTime * 8.0f);
        
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);

        //speedCounter.text = isDrifting.ToString() + sphere.linearVelocity.magnitude.ToString();
    }

    void wheelRotation(GameObject wheel, bool isFrontWheel = false, GameObject pivot = null)
    {
        if (isFrontWheel && pivot)
        {
            pivot.transform.localRotation = Quaternion.Lerp(pivot.transform.localRotation, Quaternion.Euler(wheel.transform.rotation.x,
                Input.GetAxis("Horizontal") * wheelsRotationAmount, pivot.transform.rotation.z), 0.2f); 
        }
        wheel.transform.Rotate(Vector3.right, sphere.linearVelocity.magnitude * wheelRotationMult * Time.deltaTime, Space.Self);
        //wheel.transform.localRotation = Quaternion.Euler(wheel.transform.localRotation.x + sphere.linearVelocity.magnitude * wheelRotationMult,wheel.transform.localRotation.y, wheel.transform.localRotation.z );
    }

    void Steering()
    {
        int dir = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
        float amount = Mathf.Abs(Input.GetAxis("Horizontal"));

        if (isDrifting && currentDriftDir != driftDir.none)
        {
            if (currentDriftDir == driftDir.left)
            {
                if (dir < 0)
                {
                    rotate = dir * driftMaxSteering * amount - driftBaseSteering;
                }
                else
                {
                    rotate = dir * driftMinSteering * amount - driftBaseSteering;
                }
                    
            }
            else if (currentDriftDir == driftDir.right)
            {
                if (dir > 0)
                {
                    rotate = dir * driftMaxSteering * amount + driftBaseSteering;
                }
                else
                {
                    rotate = dir * driftMinSteering * amount + driftBaseSteering;
                }
            }
        }
        else
        {
            rotate = (dir * steering) * amount;
        }
    }

    void Boost()
    {
        sphere.AddForce(kartModel.transform.forward * boostImpulse, ForceMode.Impulse);
    }


}
