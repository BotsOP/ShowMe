using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 100f, maxAirAcceleration = 50f;
    [SerializeField, Range(0f, 10f)]
    float jumpHeight = 2f;
    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;
    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f, maxStairsAngle = 50f;
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)]
    float probeDistance = 1f;
    [SerializeField]
    LayerMask probeMask = -1, stairsMask = -1;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    float mouseSensitivity;
    
    Vector3 velocity, desiredVelocity, connectionVelocity;
    Vector3 contactNormal, steepNormal;
    Vector3 connectionWorldPosition, connectionLocalPosition;
    bool desiredJump;
    int groundContactCount, steepContactCount;
    int stepsSinceLastGrounded, stepsSinceLastJump;
    bool OnGround
    {
        get {
            return groundContactCount > 0;
        }
    }
    bool OnSteep
    {
        get {
            return steepContactCount > 0;
        }
    }

    float xRotation = 0f;
    int jumpPhase;
    float minGroundDotProduct, minStairsDotProduct;
    Rigidbody body, connectedBody, previousConnectedBody;

    void OnValidate () 
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
    }
    
    void Awake () 
    {
        body = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        OnValidate();
    }

    void Update () 
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        Vector3 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = 0;
        playerInput.z = Input.GetAxis("Vertical");
        
        playerInput = (playerInput.x * right) + (playerInput.z * forward);
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);
        
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.z) * maxSpeed;
        
        desiredJump |= Input.GetButtonDown("Jump");

        MouseLook();
    }
    
    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    void FixedUpdate () 
    {
        UpdateState();
        AdjustVelocity();
        
        if (desiredJump) 
        {
            desiredJump = false;
            Jump();
        }
        
        body.velocity = velocity;
        ClearState();
        MouseLook();
    }

    void UpdateState () 
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (OnGround || SnapToGround() || CheckSteepContacts()) 
        {
            stepsSinceLastGrounded = 0;
            jumpPhase = 0;
            if (groundContactCount > 1) 
            {
                contactNormal.Normalize();
            }
        }
        else 
        {
            contactNormal = Vector3.up;
        }
        
        if (connectedBody) 
        {
            if (connectedBody.isKinematic || connectedBody.mass >= body.mass) 
            {
                UpdateConnectionState();
            }
        }
    }
    
    void UpdateConnectionState () 
    {
        if (connectedBody == previousConnectedBody) 
        {
            Vector3 connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        
        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
    }
    
    void ClearState () 
    {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = connectionVelocity = Vector3.zero;
        previousConnectedBody = connectedBody;
    }
    
    bool SnapToGround () 
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2) 
        {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed) 
        {
            return false;
        }
        if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask)) 
        {
            return false;
        }
        if (hit.normal.y < GetMinDot(hit.collider.gameObject.layer)) 
        {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        return true;
    }
    
    void Jump () {
        Vector3 jumpDirection;
        if (OnGround) 
        {
            jumpDirection = contactNormal;
        }
        else if (OnSteep) 
        {
            jumpDirection = steepNormal;
            jumpPhase = 0;
        }
        else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps) 
        {
            if (jumpPhase == 0) {
                jumpPhase = 1;
            }
            jumpDirection = contactNormal;
        }
        else {
            return;
        }

        if (stepsSinceLastJump > 1) 
        {
            jumpPhase = 0;
        }
        
        jumpPhase += 1;
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        jumpDirection = (jumpDirection + Vector3.up).normalized;
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        
        if (alignedSpeed > 0f) 
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        
        velocity += jumpDirection * jumpSpeed;
        
    }
    
    
    void OnCollisionEnter (Collision collision) 
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay (Collision collision) 
    {
        EvaluateCollision(collision);
    }
    
    void EvaluateCollision (Collision collision) 
    {
        float minDot = GetMinDot(collision.gameObject.layer);
        for (int i = 0; i < collision.contactCount; i++) 
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minDot) 
            {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
                
            }
            else if (normal.y > -0.01f) 
            {
                steepContactCount += 1;
                steepNormal += normal;
                if (groundContactCount == 0) {
                    connectedBody = collision.rigidbody;
                }
            }
        }
    }
    
    Vector3 ProjectOnContactPlane (Vector3 vector) 
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }
    
    void AdjustVelocity () 
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;
        
        Vector3 relativeVelocity = velocity - connectionVelocity;
        float currentX = Vector3.Dot(relativeVelocity, xAxis);
        float currentZ = Vector3.Dot(relativeVelocity, zAxis);
        
        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);
        
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }
    
    float GetMinDot (int layer) 
    {
       return (stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
    }
    
    bool CheckSteepContacts () 
    {
        if (steepContactCount > 1) 
        {
            steepNormal.Normalize();
            if (steepNormal.y >= minGroundDotProduct) 
            {
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }
}
