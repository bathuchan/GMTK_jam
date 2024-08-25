using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Wallrun : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;


    [Header("Wallrun Settings")]
    public LayerMask whatIsWall;
    public float wallCheckDistance = 1f, wallrunForce = 20f, maxWallrunSpeed = 10f;
    
    RaycastHit rightWallHit, leftWallHit;
    public bool isWallRight, isWallLeft;
    public bool isWallrunning;
    public float cameraTiltSpeed = 5f; // Speed of the camera tilt
    public float maxCameraTilt = 15f; // Maximum tilt angle
    [HideInInspector]public float currentCameraTilt = 0f; // Current tilt angle
    public float wallJumpCooldown = 0.5f; // Cooldown time after a wall jump
    //public float stopWallrunBelow = 1.05f;
    public float wallrunStopJumpBoostMultipler = 1f;
    public float downForceAmount = 0.2f;
    public bool canWallRide = true;
    Rigidbody rb;
    Vector3 wallNormal;



    private delegate void CheckWallDelegate();
    private void Start()
    {
        mainCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        rb = GetComponent<Rigidbody>();

        //CheckWallDelegate checkForWall = CheckForWall;
        ////StartCoroutine(WaitForFrames(20, checkForWall));
        //    // Example: Start the coroutine with a specific function
        //StartCoroutine(RunEveryXFrames(20,MyFunction));
    }


    public void CheckForWall() 
    {
        if (!canWallRide&&isWallrunning) 
        {

            StopWallrun();
            return;
        } 
        //Debug.Log("Checking for wall");
        isWallRight = Physics.Raycast(transform.position + new Vector3(0,-transform.localScale.y*0.8f,0) , transform.right, out rightWallHit, wallCheckDistance, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position + new Vector3(0, -transform.localScale.y * 0.8f, 0), -transform.right, out leftWallHit, wallCheckDistance, whatIsWall);


        if (!isWallRight && !isWallLeft)
        {
            StopWallrun();
            return;
        }
    }

    public int frameInterval = 22; // Number of frames to wait

    

    // Coroutine that accepts a delegate as a parameter
    IEnumerator RunEveryXFrames(float frameToWait,Action functionToRun)
    {
        while (true) 
        {
            
            functionToRun?.Invoke();

            
            for (int i = 0; i < frameToWait; i++)
            {
                yield return null; 
            }
        }
    }

    // Example function to be passed as a delegate
    void MyFunction()
    {
        // Example function logic
        Debug.Log("MyFunction executed at frame: " + Time.frameCount);
    }


    public Coroutine tiltCoroutine;

    public void StartWallrun()
    {
        if (!canWallRide) return;

        isWallrunning = true;
        // Start tilting the camera
        if (tiltCoroutine != null)
        {

            StopCoroutine(tiltCoroutine);
            tiltCoroutine = null;
        }

        if (isWallRight)
        {
            tiltCoroutine = StartCoroutine(TiltCamera(maxCameraTilt));
        }
        else if (isWallLeft)
        {
            tiltCoroutine = StartCoroutine(TiltCamera(-maxCameraTilt));
        }
    }
    public void WallrunningMovement()
    {
        if (Gamepad.current != null)
        {

            Gamepad.current.SetMotorSpeeds(0, 0.2f);

        }
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, -downForceAmount, rb.velocity.z);

        wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        rb.AddForce(-wallNormal * 100, ForceMode.Force);
        rb.AddForce(wallForward * wallrunForce, ForceMode.Force);
        //if (StopWallrunBelow()) StopWallrun();

    }

    //public void CalculateMaxJumpHeight(float jumpForce)
    //{
    //    float yStart = transform.position.y;
    //    // Using the equation: s = -(u^2) / (2a)
    //    minJumpHeight=((-((Mathf.Pow(jumpForce, 2)) / (2 * Physics.gravity.y))) - yStart) * 0.25f;
    //}

    //public bool StopWallrunBelow()
    //{
    //    return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    //}

    public IEnumerator TiltCamera(float targetTilt)
    {
        while (Mathf.Abs(currentCameraTilt - targetTilt) > 0.01f)
        {
            currentCameraTilt = Mathf.Lerp(currentCameraTilt, targetTilt, Time.deltaTime * cameraTiltSpeed);
            mainCamera.transform.localRotation = Quaternion.Euler(mainCamera.transform.localRotation.eulerAngles.x, mainCamera.transform.localRotation.y, currentCameraTilt);
            yield return null;
        }

        currentCameraTilt = targetTilt;

        mainCamera.transform.localRotation = Quaternion.Euler(mainCamera.transform.localRotation.eulerAngles.x, mainCamera.transform.localRotation.eulerAngles.y, currentCameraTilt);
        tiltCoroutine = null;
        yield return null;
    }

    [HideInInspector]public Coroutine wallrideCooldown;
    public void StopWallrun()
    {
        if (isWallrunning)
        {

            if (Gamepad.current != null)
            {

                Gamepad.current.SetMotorSpeeds(0, 0);


            }


            

            float temp = rb.velocity.magnitude / 100;
            if (isWallRight)
            {
                rb.AddForce(-transform.right * wallrunStopJumpBoostMultipler * Mathf.Max(wallrunStopJumpBoostMultipler, temp), ForceMode.Impulse);
            }
            else if (isWallLeft)
            {
                rb.AddForce(transform.right * wallrunStopJumpBoostMultipler * Mathf.Max(wallrunStopJumpBoostMultipler, temp), ForceMode.Impulse);
            }

            rb.AddForce(transform.forward * wallrunStopJumpBoostMultipler * Mathf.Max(wallrunStopJumpBoostMultipler, temp), ForceMode.Impulse);
            rb.AddForce(transform.up * wallrunStopJumpBoostMultipler *4* Mathf.Max(wallrunStopJumpBoostMultipler, temp)/*Mathf.Min(_jumpForce / 10, _rb.velocity.magnitude / 100)*/, ForceMode.Impulse);


            isWallrunning = false;

            isWallRight = false;
            isWallLeft = false;
            rb.useGravity = true;

            if (tiltCoroutine != null) StopCoroutine(tiltCoroutine);
            tiltCoroutine = StartCoroutine(TiltCamera(0f));


            //if (wallrideCooldown == null) wallrideCooldown = StartCoroutine(WallJumpCooldown());
        }
    }


    public  IEnumerator WallJumpCooldown()
    {
        canWallRide = false;
        isWallrunning = false;
        yield return new WaitForSeconds(wallJumpCooldown);
        canWallRide = true;
        wallrideCooldown = null;
        yield return null;
    }

}
