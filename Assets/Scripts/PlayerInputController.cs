using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using SmoothShakeFree;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using static UnityEngine.InputSystem.InputAction;
using System.Security.Cryptography;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEditor;

[RequireComponent(typeof(Rigidbody))]
public class PlayerInputController : MonoBehaviour
{
    private DefaultPlayerActions _defaultPlayerActions;

    private InputAction _moveAction, _lookAction, _jumpAction
        , _runAction, _crouchAction, _dragAction, _leftClickAction, _rightClickAction, 
        _scaleScrollUpAction, _scaleScrollDownAction, _scaleScrollButtonAction,_escapeButtonAction;

    //private InputAction _shootAction;

    [HideInInspector] public Rigidbody _rb;
    [Header("FOV Settings")]
    public CinemachineVirtualCamera mainCamera; // Assign this to the main camera in the inspector
    public float normalFOV = 60f; // Default FOV
    public float runningFOV = 70f;
    public float crouchFOV = 60f;// FOV when running
    public float fovTransitionSpeed = 2f; // Speed of FOV transition


    private float targetFOV;

    float speed;
    [Header("Movement Settings")]
    public float _moveSpeed = 5f;
    public float _runSpeedMultiplier = 5f; // Multiplier for run speed
    public float defautSpeedDropOff = 1f;
    public float _jumpForce = 5f; // Jump force
    public float _jumpForceOnSlope = 2.5f;
    public float jumpRaycastLenght = 1.02f;
    public float maxVelocity = 10f;
    Vector3 maxVelVector;

    [Header("Wallrun Settings")]
    public LayerMask whatIsWall;
    public float wallCheckDistance = 1f, wallrunForce, maxWallrunSpeed, minJumpHeight = 2.2f;
    RaycastHit rightWallHit, leftWallHit;
    bool isWallRight, isWallLeft;
    [HideInInspector] public bool isWallrunning;
    public float cameraTiltSpeed = 5f; // Speed of the camera tilt
    public float maxCameraTilt = 15f; // Maximum tilt angle
    private float currentCameraTilt = 0f; // Current tilt angle
    public float wallJumpCooldown = 0.5f; // Cooldown time after a wall jump
    public float stopWallrunBelow = 1.05f;
    public float wallrunStopJumpBoostMultipler = 1f;
    public float downForceAmount = 0.2f;
    private bool canWallJump = true;


    [Header("Crouch Settings")]
    public float _crouchSpeedMultiplier = 0.5f;
    public float crouchYscale;
    private float startYscale;

    [Header("Slope Settings")]
    public float maxSlopeAngle = 0.5f;
    private RaycastHit slopeHit;

    bool isWalking, isRunning, isCrouching;
    [Header("Camera Settings")]
    [Range(0f, 1f)]
    public float _lookTreshold = 0.25f;
    public float _lookSensitivityVertical = 5f;
    public float _lookSensitivityHorizontal = 5f;

    private float verticalRotation = 0;
    public float rotationDeadAngle = 75f;


    [HideInInspector] public CapsuleCollider playerCollider = null;
    GameObject playerModel;



    [Header("Cam Wobble Settings")]
    public bool camWobbleOn = true;
    public float camWobbleNormal, camWobbleWalking, camWobbleRunning, camWobbleCrouching;
    public float camWobbleChangeSpeed = 1f;


    public Vector3 smoothShakeFrequency;
    private SmoothShake smoothShake;




    [Header("Air Speed Settings")]

    private float airTime = 0f;
    public float airSpeedMultiplier = 1f;
    public float maxAirSpeedMultiplier = 10f; // Maximum speed multiplier while in the air
    public float airTimeIncrement = 0.1f; // Rate at which the multiplier increases per second in the air
    public float speedDecreaseRate = 1f; // Rate at which the multiplier decreases after landing

    private bool wasGrounded = true;

    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Item Rotating Settings")]
    public float rotateSpeed = 1f;
    private Vector2 rotation;


    RaycastHit hit;
    [HideInInspector] public Vector2 cumulativeMouseDelta;
    private DragAndDrop dragAndDrop;
    private ChooseBox chooseBox;
    float _jumpForceAtStart;
    private void Awake()
    {

        targetFOV = normalFOV;
        mainCamera.m_Lens.FieldOfView = normalFOV;

        _defaultPlayerActions = new DefaultPlayerActions();
        _rb = GetComponent<Rigidbody>();
        smoothShake = GetComponentInChildren<SmoothShake>();

        dragAndDrop = GetComponent<DragAndDrop>();
        chooseBox = GetComponent<ChooseBox>();
        _jumpForceAtStart = _jumpForce;





    }

    private void Start()
    {
        smoothShake.enabled = camWobbleOn;
        smoothShakeFrequency = smoothShake.positionShake.frequency;
        playerCollider = gameObject.GetComponentInChildren<CapsuleCollider>();
        playerModel = gameObject.transform.GetChild(0).gameObject;
        startYscale = playerModel.transform.localScale.y;
    }


    private void OnEnable()
    {
        _moveAction = _defaultPlayerActions.Player.Move;
        _moveAction.Enable();
        _lookAction = _defaultPlayerActions.Player.Look;
        _lookAction.Enable();
        _jumpAction = _defaultPlayerActions.Player.Jump;
        _jumpAction.Enable();
        _runAction = _defaultPlayerActions.Player.Run;
        _runAction.Enable();
        _crouchAction = _defaultPlayerActions.Player.Crouch;
        _crouchAction.Enable();
        _dragAction = _defaultPlayerActions.Player.Drag;
        _dragAction.Enable();
        _leftClickAction = _defaultPlayerActions.Player.LeftClick;
        _leftClickAction.Enable();
        _rightClickAction = _defaultPlayerActions.Player.RightClick;
        _rightClickAction.Enable();
        _scaleScrollButtonAction = _defaultPlayerActions.Player.AxisChange;
        _scaleScrollButtonAction.Enable();
        _scaleScrollUpAction = _defaultPlayerActions.Player.AxisUp;
        _scaleScrollUpAction.Enable();
        _scaleScrollDownAction = _defaultPlayerActions.Player.AxisDown;
        _scaleScrollDownAction.Enable();

        _escapeButtonAction = _defaultPlayerActions.Player.Pause;
        _escapeButtonAction.Enable();
        //_shootAction = _defaultPlayerActions.Player.Fire;
        //_shootAction.Enable();

        _moveAction.performed += OnWalk;
        _moveAction.canceled += OnWalkCancel;
        _jumpAction.performed += OnJump; // Subscribe to jump action
        _jumpAction.canceled += OnJumpCancel;
        _runAction.performed += OnRun;
        _runAction.canceled += OnRunCancel;
        _crouchAction.performed += OnCrouch;
        _crouchAction.canceled += OnCrouchCancel;
        _dragAction.performed += OnObjectDrag;
        _dragAction.canceled += OnObjectDragCancel;
        _leftClickAction.performed += OnLeftClick;
        _leftClickAction.canceled += OnLeftClickCancel;
        _rightClickAction.performed += OnRightClick;
        _rightClickAction.canceled += OnRightClickCancel;

        _scaleScrollButtonAction.performed += OnScaleButton;
        _scaleScrollUpAction.performed += OnScaleUp;
        _scaleScrollDownAction.performed += OnScaleDown;

        _escapeButtonAction.performed += OnPauseButton;
        //_shootAction.performed += OnShoot;
        //_shootAction.canceled += OnShootCancel;
        InputSystem.onDeviceChange += OnDeviceChange; // Subscribe to device change events


    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _jumpAction.Disable();
        _runAction.Disable();
        _crouchAction.Disable();
        _dragAction.Disable();
        _leftClickAction.Disable();
        _rightClickAction.Disable();
        _scaleScrollButtonAction.Disable();
        _scaleScrollUpAction.Disable();
        _scaleScrollDownAction.Disable();

        _moveAction.performed -= OnWalk;
        _moveAction.canceled -= OnWalkCancel;
        _jumpAction.performed -= OnJump;
        _jumpAction.canceled -= OnJumpCancel;
        _runAction.performed -= OnRun;
        _runAction.canceled -= OnRunCancel;

        _crouchAction.performed -= OnCrouch;
        _crouchAction.canceled -= OnCrouchCancel;

        _dragAction.performed -= OnObjectDrag;
        _dragAction.canceled -= OnObjectDragCancel;

        _leftClickAction.performed -= OnLeftClick;
        _leftClickAction.canceled -= OnLeftClickCancel;
        _rightClickAction.performed -= OnRightClick;
        _rightClickAction.canceled -= OnRightClickCancel;

        _scaleScrollButtonAction.performed -= OnScaleButton;
        _scaleScrollUpAction.performed -= OnScaleUp;
        _scaleScrollDownAction.performed -= OnScaleDown;

        InputSystem.onDeviceChange -= OnDeviceChange; // Unsubscribe from device change events
    }
    DirectionalScalableCube dsc;
    private void OnScaleButton(InputAction.CallbackContext context)
    {

        GameObject objectToChangeAxis = chooseBox.lookingAt;
        if (chooseBox.lookingAt != null && objectToChangeAxis.TryGetComponent<DirectionalScalableCube>(out dsc))
        {
            dsc.ChangeAxis(true);
            chooseBox.UiTexts[1].text = "CHANGE SCALE IN:" + dsc.GetAxisString() + System.Environment.NewLine + "(M.SCROLL/R)";
            chooseBox.UiTexts[1].color = dsc.ChangeTextColor();
            dsc = null;
        }
        else {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, chooseBox.interactRange, chooseBox.hitWhat))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("BlueBox") && objectToChangeAxis.TryGetComponent<DirectionalScalableCube>(out dsc))
                {
                    if (hit.transform.TryGetComponent<Animator>(out Animator anim))
                    {
                        anim.SetTrigger("LookingAt");
                        dsc.ChangeAxis(true);
                        chooseBox.UiTexts[1].text = "CHANGE SCALE IN:" + dsc.GetAxisString() + System.Environment.NewLine + "(M.SCROLL/R)";
                        chooseBox.UiTexts[1].color = dsc.ChangeTextColor();
                        chooseBox.lookingAt = hit.transform.gameObject;
                        dsc = null;
                    }

                }


            }

        }
    }
    private void OnPauseButton(InputAction.CallbackContext context)
    {
       
    }

    private void OnScaleUp(InputAction.CallbackContext context)
    {
        GameObject objectToChangeAxis = chooseBox.lookingAt;
        if (chooseBox.lookingAt != null && objectToChangeAxis.TryGetComponent<DirectionalScalableCube>(out dsc))
        {
            dsc.ChangeAxis(true);
            chooseBox.UiTexts[1].text = "CHANGE SCALE IN:" + dsc.GetAxisString() + System.Environment.NewLine + "(M.SCROLL/R)";
            chooseBox.UiTexts[1].color = dsc.ChangeTextColor();
            dsc = null;
        }
        else
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, chooseBox.interactRange, chooseBox.hitWhat))
            {
                if (hit.transform.gameObject.layer==LayerMask.NameToLayer("BlueBox")  && objectToChangeAxis.TryGetComponent<DirectionalScalableCube>(out dsc))
                {
                    if (hit.transform.TryGetComponent<Animator>(out Animator anim))
                    {
                        anim.SetTrigger("LookingAt");
                        dsc.ChangeAxis(true);
                        chooseBox.UiTexts[1].text = "CHANGE SCALE IN:" + dsc.GetAxisString() + System.Environment.NewLine + "(M.SCROLL/R)";
                        chooseBox.UiTexts[1].color = dsc.ChangeTextColor();
                        chooseBox.lookingAt = hit.transform.gameObject;
                        dsc = null;
                    }

                }


            }

        }

    }

    private void OnScaleDown(InputAction.CallbackContext context)
    {
        GameObject objectToChangeAxis = chooseBox.lookingAt;
        if (chooseBox.lookingAt != null && objectToChangeAxis.TryGetComponent<DirectionalScalableCube>(out dsc))
        {
            dsc.ChangeAxis(false);
            chooseBox.UiTexts[1].text = "CHANGE SCALE IN:" + dsc.GetAxisString() + System.Environment.NewLine + "(M.SCROLL/R)";
            chooseBox.UiTexts[1].color = dsc.ChangeTextColor();
            dsc = null;
        }
        else
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, chooseBox.interactRange, chooseBox.hitWhat))
            {
                if (hit.transform.gameObject.layer==LayerMask.NameToLayer("BlueBox")&& objectToChangeAxis.TryGetComponent<DirectionalScalableCube>(out dsc))
                {
                    if (hit.transform.TryGetComponent<Animator>(out Animator anim)&&dsc!=null)
                    {
                        anim.SetTrigger("LookingAt");
                        dsc.ChangeAxis(false);
                        chooseBox.UiTexts[1].text = "CHANGE SCALE IN:" + dsc.GetAxisString() + System.Environment.NewLine + "(M.SCROLL/R)";
                        chooseBox.UiTexts[1].color = dsc.ChangeTextColor();
                        chooseBox.lookingAt = hit.transform.gameObject;
                        dsc = null;
                    }

                }


            }

        }

    }



    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log($"Device added: {device.displayName}");
                break;
            case InputDeviceChange.Removed:
                Debug.Log($"Device removed: {device.displayName}");
                break;
            case InputDeviceChange.Disconnected:
                Debug.Log($"Device disconnected: {device.displayName}");
                break;
            case InputDeviceChange.Reconnected:
                Debug.Log($"Device reconnected: {device.displayName}");
                break;
        }
    }
    Coroutine airCoroutine, reduceCoroutine;

    bool jumping, canJump = true;


    private void OnJump(InputAction.CallbackContext context)
    {

        jumping = true;
        if (coyoteTimeCounter > 0 && wasGrounded && canJump)
        {
            if (_rb.useGravity == false) { _rb.useGravity = true; }

            coyoteTimeCounter = 0f;
            wasGrounded = false;
            Jump();
        }

        // Reset coyote time counter after a jump


    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        if (airCoroutine == null)

        {
            airCoroutine = StartCoroutine(InAir());
        }



    }


    private void OnJumpCancel(InputAction.CallbackContext context)
    {
        jumping = false;

        if (isWallrunning /*&& canWallJump*/)
        {
            //if (waitCoroutine == null)
            //{
            //    waitCoroutine = StartCoroutine(WaitFor(4f, w8));
            //}
            //if (IsGrounded() && waitCoroutine != null)
            //{
            //    w8 = true;
            //    waitCoroutine = null;
            //}
            Debug.Log("Wall Jump Attempted");

            if (reduceCoroutine != null) { StopCoroutine(reduceCoroutine); }
            if (airCoroutine == null) { airCoroutine = StartCoroutine(InAir()); }




            // Perform the wall jump

            StopWallrun();
            // Start the cooldown
            StartCoroutine(WallJumpCooldown());


        }
    }
    private IEnumerator WallJumpCooldown()
    {
        canWallJump = false;
        isWallrunning = false;
        yield return new WaitForSeconds(wallJumpCooldown);
        canWallJump = true;
    }

    [HideInInspector] public bool inAir = false;
    IEnumerator InAir()
    {

        inAir = true;
        bool grounded = false;
        airTime = 0f;


        while (!grounded)
        {

            airTime += Time.deltaTime;



            //Debug.Log("In Air - Air Time: " + airTime + " - Speed Multiplier: " + airSpeedMultiplier);
            yield return new WaitForSeconds(0.1f);

            if (jumping) { Debug.Log("Checkin for wall"); CheckForWall(); }

            if (jumping && (isWallRight || isWallLeft) && AboveGround())
            {
                // if (0<= wallrunTimeCounter) {

                if (reduceCoroutine == null) { StartCoroutine(ReduceSpeedMultiplier()); }
                StartWallrun();
                continue;
                //}

            }

            if (IsGrounded())
            {
                wasGrounded = true;
                grounded = true;

                StopWallrun();


                break;

            }

            if ((isWallRight || isWallLeft))
            {
                airSpeedMultiplier = 1;
                _rb.velocity = Vector3.zero;
            }
            else
            {
                airSpeedMultiplier = Mathf.Min(airSpeedMultiplier + airTime * airTimeIncrement, maxAirSpeedMultiplier);
            }


            //_rb.AddForce(transform.forward* airSpeedMultiplier*_rb.velocity.magnitude, ForceMode.Acceleration);
            _rb.velocity = new Vector3(maxVelVector.x, _rb.velocity.y, maxVelVector.z);

        }

        if (tiltCoroutine == null) {

            tiltCoroutine = StartCoroutine(TiltCamera(0f));
        }
        Debug.Log("Landed");
        // After landing, slowly decrease the speed multiplier back to 1
        reduceCoroutine = StartCoroutine(ReduceSpeedMultiplier());
        airCoroutine = null;
        inAir = false;
        //StopCoroutine( airCoroutine );
        yield return null;
    }

    IEnumerator ReduceSpeedMultiplier()
    {
        while (airSpeedMultiplier > 1f)
        {
            //airCoroutine = null;
            airSpeedMultiplier -= speedDecreaseRate * Time.deltaTime;
            airSpeedMultiplier = Mathf.Max(airSpeedMultiplier, 1f);
            yield return null;

        }

    }

    private bool IsGrounded()
    {
        // Perform a simple raycast to check if the player is grounded
        return Physics.SphereCast(transform.position, playerCollider.radius, Vector3.down, out RaycastHit hitInfo, jumpRaycastLenght);
    }

    //WALLRUN
    private void CheckForWall()
    {


        isWallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDistance, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDistance, whatIsWall);


        if (!isWallRight && !isWallLeft)
        {
            StopWallrun();
            return;
        }



    }

    Coroutine tiltCoroutine;
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    private void StartWallrun()
    {
        if (!canWallJump) return;

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
    Vector3 wallNormal;
    private void WallrunningMovement()
    {
        if (Gamepad.current != null)
        {

            Gamepad.current.SetMotorSpeeds(0, 0.2f);

        }
        _rb.useGravity = false;
        _rb.velocity = new Vector3(_rb.velocity.x, -downForceAmount, _rb.velocity.z);

        wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        _rb.AddForce(wallForward * wallrunForce, ForceMode.Force);
        if (Physics.Raycast(transform.position, Vector3.down, stopWallrunBelow)) StopWallrun();

    }
    private void StopWallrun()
    {
        if (isWallrunning) {

            if (Gamepad.current != null)
            {

                Gamepad.current.SetMotorSpeeds(0, 0);


            }


            //_rb.AddForce(transform.forward * _jumpForce * 1.5f, ForceMode.Impulse);

            float temp = _rb.velocity.magnitude / 100;
            if (isWallRight)
            {
                _rb.AddForce(-transform.right * _jumpForce * Mathf.Max(wallrunStopJumpBoostMultipler, temp), ForceMode.Impulse);
            }
            else if (isWallLeft)
            {
                _rb.AddForce(transform.right * _jumpForce * Mathf.Max(wallrunStopJumpBoostMultipler, temp), ForceMode.Impulse);
            }

            _rb.AddForce(transform.forward * _jumpForce * Mathf.Max(wallrunStopJumpBoostMultipler, temp), ForceMode.Impulse);
            _rb.AddForce(transform.up * _jumpForce * Mathf.Max(wallrunStopJumpBoostMultipler, temp)/*Mathf.Min(_jumpForce / 10, _rb.velocity.magnitude / 100)*/, ForceMode.Impulse);
            //_rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);

            isWallrunning = false;
            //_rb.AddForce(Vector3.up * _jumpForce);
            //_rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y /*Mathf.Min(_rb.velocity.y, _jumpForce*2)*/, _rb.velocity.z);
            isWallRight = false;
            isWallLeft = false;
            _rb.useGravity = true;

            if (tiltCoroutine != null) StopCoroutine(tiltCoroutine);
            tiltCoroutine = StartCoroutine(TiltCamera(0f));



        }








    }

    private IEnumerator TiltCamera(float targetTilt)
    {
        while (Mathf.Abs(currentCameraTilt - targetTilt) > 0.01f)
        {
            currentCameraTilt = Mathf.Lerp(currentCameraTilt, targetTilt, Time.deltaTime * cameraTiltSpeed);
            mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation + recoilX, recoilY, currentCameraTilt);
            yield return null;
        }

        currentCameraTilt = targetTilt;

        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation + recoilX, recoilY, currentCameraTilt);
        tiltCoroutine = null;
        yield return null;
    }




    Vector3 vel;
    private void HandleMovement()
    {
        Vector2 moveDir = _moveAction.ReadValue<Vector2>();


        if (moveDir.magnitude > _lookTreshold)
        {
            isWalking = true;

            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = (forward * moveDir.y + right * moveDir.x).normalized;

            speed = _moveSpeed * airSpeedMultiplier;



            if (isCrouching)
            {
                Debug.Log("crouching ");
                speed *= _crouchSpeedMultiplier;

                targetFOV = crouchFOV;
                isRunning = false;
            }

            if (isRunning)
            {
                speed *= _runSpeedMultiplier;
                targetFOV = runningFOV;

            }

            vel = new Vector3(desiredMoveDirection.x * speed, _rb.velocity.y, desiredMoveDirection.z * speed);

            if (isWallrunning /*&& airCoroutine != null/*_rb.velocity.magnitude >= vel.magnitude*/)
            {

                Debug.Log("move1");
                if ((moveDir.x > _lookTreshold && isWallRight) || moveDir.x < -_lookTreshold && isWallLeft)
                {
                    // _rb.AddForce(-wallNormal * 100, ForceMode.Force);
                }




            }
            else if (!isWallrunning && airCoroutine != null /*&& (_rb.velocity.magnitude >= vel.magnitude)*/)
            {
                Debug.Log("move2");
                _rb.AddForce(new Vector3(vel.x, 0, vel.z), ForceMode.Force);


            }
            else
            {

                if (OnSlope())
                {
                    _rb.velocity = GetSlopeMoveDirection() * vel.magnitude;
                    Debug.Log("move4");
                    Debug.Log("Onslope");
                }
                else
                {
                    Debug.Log("move3");
                    _rb.velocity = vel;
                }



            }

        }
        else
        {

            //if (waitCoroutine == null) StartCoroutine(WaitFor(2f, w8));


            if (!isWallrunning && airCoroutine == null /*&& w8*/ /*&& (_rb.velocity.magnitude >= vel.magnitude)*/)
            {
                _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            }

        }
    }



    [HideInInspector] public Quaternion recoil;

    [HideInInspector] public float recoilX, recoilY;
    bool isMouse = true;
    private void HandleLook()
    {
        if (rotateCoroutine != null) { return; }
        Vector2 lookDir = _lookAction.ReadValue<Vector2>();

        if (lookDir.magnitude > _lookTreshold)
        {


            if (Gamepad.current != null)
            {
                isMouse = false;
            }
            else if (Mouse.current != null)
            {
                isMouse = true;
            }


            float mouseX = lookDir.x * (isMouse ? _lookSensitivityHorizontal * 0.1f : _lookSensitivityHorizontal) * Time.deltaTime;
            float mouseY = lookDir.y * (isMouse ? _lookSensitivityVertical * 0.1f : _lookSensitivityVertical) * Time.deltaTime;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -rotationDeadAngle, rotationDeadAngle);

            // Apply horizontal rotation to the player
            transform.Rotate(Vector3.up * mouseX);


            mainCamera.transform.localRotation = Quaternion.Euler(Mathf.Clamp(verticalRotation + recoilX, -70f, 70f), recoilY, currentCameraTilt);


        }
        else if (lookDir.magnitude <= _lookTreshold)
        {
            mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation + recoilX, recoilY, currentCameraTilt);

            //mainCamera.transform.localRotation = Quaternion.Euler(mainCamera.transform.localRotation.eulerAngles.x + recoilX, mainCamera.transform.localRotation.eulerAngles.y+ recoilY, mainCamera.transform.localRotation.eulerAngles.z);
            //mainCamera.transform.localRotation = recoil;
        }

    }



    private void OnWalk(InputAction.CallbackContext context)
    {
        if (!isWalking)
        {
            isWalking = true;
            smoothShakeFrequency *= camWobbleWalking;
            //smoothShake.positionShake.frequency = smoothShake.positionShake.frequency * camWobbleWalking;

        }


    }

    private void OnWalkCancel(InputAction.CallbackContext context)
    {
        isWalking = false;
        smoothShakeFrequency /= camWobbleWalking;
        //smoothShake.positionShake.frequency = smoothShake.positionShake.frequency / camWobbleWalking;




    }

    private void OnRun(InputAction.CallbackContext context)
    {
        isRunning = true;
        smoothShakeFrequency *= camWobbleRunning;
        //smoothShake.positionShake.frequency = smoothShake.positionShake.frequency * camWobbleRunning;

    }

    private void OnRunCancel(InputAction.CallbackContext context)
    {
        isRunning = false;
        targetFOV = normalFOV;
        smoothShakeFrequency /= camWobbleRunning;
        //smoothShake.positionShake.frequency = smoothShake.positionShake.frequency / camWobbleRunning;



    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = true;
        isRunning = false;
        targetFOV = crouchFOV;
        smoothShakeFrequency *= camWobbleCrouching;
        //smoothShake.positionShake.frequency = smoothShake.positionShake.frequency * camWobbleCrouching;


        playerModel.transform.localScale = new Vector3(playerModel.transform.localScale.x, crouchYscale, playerModel.transform.localScale.z);
        _rb.AddForce(Vector3.down * 5, ForceMode.Impulse);


    }
    Coroutine checkUpCoroutine;
    private void OnCrouchCancel(InputAction.CallbackContext context)
    {
        if (checkUpCoroutine == null)
        {
            checkUpCoroutine = StartCoroutine(CheckUp());
        }
        else
        {
            StopCoroutine(checkUpCoroutine);
            checkUpCoroutine = null;
            checkUpCoroutine = StartCoroutine(CheckUp());
        }



    }
    IEnumerator CheckUp()
    {
        while (Physics.Raycast(playerModel.transform.position, playerModel.transform.up, transform.localScale.y * 2f))
        {
            canJump = false;
            yield return new WaitForSeconds(.5f);
        }
        canJump = true;
        isCrouching = false;
        smoothShakeFrequency /= camWobbleCrouching;


        targetFOV = normalFOV;
        playerModel.transform.localScale = new Vector3(playerModel.transform.localScale.x, startYscale, playerModel.transform.localScale.z);
        yield return null;


    }

    private bool OnSlope()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerCollider.height * 0.5f + (playerCollider.height * transform.localScale.y) * 0.4f))
        {
            //Debug.Log("ONslope");
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(vel, slopeHit.normal).normalized;
    }

    [HideInInspector] public bool isDragging = false;
    private void OnObjectDrag(InputAction.CallbackContext context)
    {



        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitt, dragAndDrop.dragDistance, dragAndDrop.draggableLayer)
            && !isDragging && !dragAndDrop.isDragging)
        {
            isDragging = dragAndDrop.TryStartDrag(hitt);
        } else if (isDragging)
        {
            dragAndDrop.StopDrag();

        }
    }
    private void OnObjectDragCancel(InputAction.CallbackContext context)
    {
        //isDragging = false;

    }

    //float maxY=0, prevMaxY;

    [HideInInspector] public bool isLeftClick,isRightClick;
    Coroutine weightCoroutine,scaleCoroutine, dimensionCoroutine;


    private void OnLeftClick(InputAction.CallbackContext context)
    {
        isLeftClick = true;
        if (isDragging&& dragAndDrop.draggedObject != null) {

            if (rotateCoroutine == null) 
            {
                rotateCoroutine = StartCoroutine(RotateObject());
            }
            

            return;
        }
        
        
        GameObject effectedObject=null;
        if (chooseBox.lookingAt != null)
        {

            effectedObject = chooseBox.lookingAt;
        }
        else
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, chooseBox.interactRange, chooseBox.hitWhat))
            {
                if (hit.transform.tag == "Box"||(hit.transform.gameObject.layer == LayerMask.NameToLayer("BlueBox")|| hit.transform.gameObject.layer == LayerMask.NameToLayer("YellowBox")|| hit.transform.gameObject.layer == LayerMask.NameToLayer("GreenBox")))
                {
                    if (hit.transform.TryGetComponent<Animator>(out Animator anim))
                    {
                        anim.SetTrigger("LookingAt");//////
                       
                    }

                    effectedObject = hit.transform.gameObject;
                    
                }
                else
                {

                    return;
                }

            }
        }

        
        if (effectedObject != null) 
        {
            if (effectedObject.TryGetComponent<ScalableCube>(out ScalableCube scalableCube))
            {
                if (scaleCoroutine == null)
                    scaleCoroutine= StartCoroutine(ChangeSize(scalableCube,true)); 
            }
            else if (effectedObject.TryGetComponent<HeavyCube>(out HeavyCube heavyCube))
            {
                if (weightCoroutine == null)
                    weightCoroutine = StartCoroutine(IncreaseWeight(heavyCube));
            }
            else if (effectedObject.TryGetComponent<DirectionalScalableCube>(out DirectionalScalableCube dsc)) 
            {
                if (dimensionCoroutine == null)
                    dimensionCoroutine = StartCoroutine(ChangeDimensions(dsc, true));
            }
        }
        effectedObject = null;
    }
    
    
    IEnumerator RotateObject ()
    {
        float temp = dragAndDrop.rotationBreakMultiplier;
        while (isLeftClick) {
            rotation = _lookAction.ReadValue<Vector2>();
            rotation *= rotateSpeed;
            
                dragAndDrop.draggedObject.transform.Rotate(Vector3.up, rotation.x, Space.World);
                dragAndDrop.draggedObject.transform.Rotate(Vector3.right, rotation.y, Space.World);
            
           
            //dragAndDrop.rb.velocity = Vector3.zero;
            //dragAndDrop.rb.angularDrag = 5f;
            dragAndDrop.rotationBreakMultiplier = temp* 0.75f;

            //dragAndDrop.rb.constraints = RigidbodyConstraints.None;
            yield return null;

        }
        if (dragAndDrop.rb != null) 
        {
            dragAndDrop.rb.angularDrag = 0.05f;
        }
        dragAndDrop.rotationBreakMultiplier = temp;

        rotateCoroutine = null;
    }

    Coroutine rotateCoroutine;
    private void OnLeftClickCancel(InputAction.CallbackContext context)
    {
        isLeftClick = false;
        if (isDragging )
        {
            

            return;
        }

        if (weightCoroutine != null) 
        {
            StopCoroutine(weightCoroutine);
            weightCoroutine=null;
        }
        if (scaleCoroutine != null) 
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }
            

    }
    private void OnRightClick(InputAction.CallbackContext context)
    {
        if (isDragging) return;
        isRightClick = true;
        GameObject effectedObject = null;
        if (chooseBox.lookingAt != null)
        {

            effectedObject = chooseBox.lookingAt;
        }
        else
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, chooseBox.interactRange, chooseBox.hitWhat))
            {
                if (hit.transform.tag == "Box"||(hit.transform.gameObject.layer == LayerMask.NameToLayer("BlueBox")|| hit.transform.gameObject.layer == LayerMask.NameToLayer("YellowBox")|| hit.transform.gameObject.layer == LayerMask.NameToLayer("GreenBox")))
                {
                    if (hit.transform.TryGetComponent<Animator>(out Animator anim))
                    {
                        anim.SetTrigger("LookingAt");//////

                    }

                    effectedObject = hit.transform.gameObject;

                }
                else
                {

                    return;
                }

            }
        }
        if (effectedObject != null)
        {
            if (effectedObject.TryGetComponent<ScalableCube>(out ScalableCube scalableCube))
            {
                if (scaleCoroutine == null)
                    scaleCoroutine = StartCoroutine(ChangeSize(scalableCube, false)); ;
            }
            else if (effectedObject.TryGetComponent<HeavyCube>(out HeavyCube heavyCube))
            {
                if (weightCoroutine == null)
                    weightCoroutine = StartCoroutine(DecreaseWeight(heavyCube));
            }
            else if (effectedObject.TryGetComponent<DirectionalScalableCube>(out DirectionalScalableCube dsc))
            {
                if (dimensionCoroutine == null)
                    dimensionCoroutine = StartCoroutine(ChangeDimensions(dsc,false));
                
            }
        }
        effectedObject = null;
    }
    private void OnRightClickCancel(InputAction.CallbackContext context)
    {
        if (isDragging) return;
        isRightClick = false;
        if (weightCoroutine != null)
        {
            StopCoroutine(weightCoroutine);
            weightCoroutine = null;
        }
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }
    }

    IEnumerator ChangeSize(ScalableCube scalableCube,bool increase)
    {
        while (isLeftClick||isRightClick)
        {
            if (increase) { scalableCube.Interact(); }
            else { scalableCube.InteractAlt(); }
            
            yield return new WaitForSeconds(.1f);
            yield return null;
        }
        dimensionCoroutine = null;
        yield return null;


    }

    IEnumerator IncreaseWeight(HeavyCube heavyCube)
    {
        while (isLeftClick)
        {
            heavyCube.Interact();
            yield return new WaitForSeconds(.1f);
            yield return null;
        }
        weightCoroutine = null;
        yield return null;


    }



    IEnumerator DecreaseWeight(HeavyCube heavyCube)
    {
        while (isRightClick)
        {
            heavyCube.InteractAlt();
            yield return new WaitForSeconds(.1f);
            yield return null;
        }
        weightCoroutine = null;
        yield return null;
    }


    IEnumerator ChangeDimensions(DirectionalScalableCube dsc,bool increase)
    {
        while (isLeftClick || isRightClick)
        {
            if (increase) {dsc.Interact(); }
            if (!increase){ dsc.InteractAlt(); }
            
            yield return new WaitForSeconds(.1f);
            yield return null;
        }
        dimensionCoroutine = null;
        yield return null;
    }

    [HideInInspector]public bool onSlope;
    private void Update()
    {// Manage coyote time
        

        mainCamera.m_Lens.FieldOfView = Mathf.Lerp(mainCamera.m_Lens.FieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
        //mainCamera.m_Lens.FieldOfView = Mathf.Lerp(mainCamera.m_Lens.FieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
        smoothShake.positionShake.frequency = Vector3.MoveTowards(smoothShake.positionShake.frequency, smoothShakeFrequency, camWobbleChangeSpeed * Time.deltaTime);
        onSlope = OnSlope();
        _rb.useGravity = !onSlope;

        if (onSlope)
        {
            _rb.drag = 4f;
            jumpRaycastLenght = 1.12f * transform.localScale.y;
            _jumpForce=_jumpForceOnSlope;
            _rb.AddForce(-slopeHit.normal * 6f, ForceMode.Force);
        }
        else 
        {
            _rb.drag = 1f;
            _jumpForce = _jumpForceAtStart;
            jumpRaycastLenght = 1.02f * transform.localScale.y;
        }

        if (IsGrounded())
        {
            

            coyoteTimeCounter = coyoteTime;
            wasGrounded = true;
            StopWallrun();
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //prevMaxY = transform.position.y;
        //if (prevMaxY >= maxY) {
        //    maxY = prevMaxY;
        //    Debug.Log(maxY); }


        HandleMovement();
        maxVelVector = Vector3.ClampMagnitude(_rb.velocity, maxVelocity);
        if (inAir)
            Debug.DrawRay(transform.position, -transform.up * jumpRaycastLenght, Color.cyan, 1f);
        // HandleJump();
        //Debug.Log("veloc:"+_rb.velocity);
    }

    private void FixedUpdate()
    {
       


        HandleLook();
        if (isWallrunning)
        {
            WallrunningMovement();
        }

    }

    
    

}
