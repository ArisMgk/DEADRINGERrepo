using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCharacter : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool isSprinting => Input.GetKey(SprintKey) && canSprint && currentInput != Vector2.zero && !isCrouching && !isSliding;
    private bool ShouldJump => Input.GetKeyDown(JumpKey) && charactercontroller.isGrounded && !isSliding;
    private bool ShouldCrouch => Input.GetKeyDown(CrouchKey) && !duringCrouchAnimation && charactercontroller.isGrounded;
    private bool CanPause => Input.GetKeyDown(PauseKey);

    private bool CanPickup = true;


    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool CanHeadbob = true;
    [SerializeField] private bool SlideOnSlope = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useFootsteps = true;
    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isChanged;
    [SerializeField] private bool useStamina = true;


    [Header("Controls")]
    [SerializeField] private KeyCode SprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode JumpKey = KeyCode.Space;
    [SerializeField] private KeyCode CrouchKey = KeyCode.C;
    [SerializeField] private KeyCode InteractKey = KeyCode.E;
    [SerializeField] private KeyCode PauseKey = KeyCode.Escape;


    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchspeed = 1.5f;
    [SerializeField] private float slopespeed = 8f;


    [Header("FOV Parameters")]
    [SerializeField] private float changespeedfov = 0.02f;
    [SerializeField] private float changebackspeedfov = 0.05f;
    [SerializeField] private float maxFOV = 90f;
    private float startingFOV = 60f;


    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;


    [Header("Stamina Parameters")]
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float StaminaUseMultiplier = 5;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 3;
    [SerializeField] private float StaminaValueIncrement = 1f;
    [SerializeField] private float staminaTimeIncrement = 0.1f;
    private float currentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;


    [Header("Jumping Parameters")]
    [SerializeField] private float JumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;


    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float TimetoCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;


    [Header("Headbob Parameters")]
    [SerializeField] private float walkbobspeed = 14f;
    [SerializeField] private float walkbobamount = 0.05f;
    [SerializeField] private float sprintbobspeed = 18f;
    [SerializeField] private float sprintbobamount = 0.11f;
    [SerializeField] private float crouchbobspeed = 8f;
    [SerializeField] private float crouchbobamount = 0.025f;
    private float defaultYPos = 0;
    private float timer;


    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    [SerializeField] private AudioSource m_audiosource;
    [SerializeField] private AudioClip[] SandClips;
    [SerializeField] private AudioClip[] RockClips;
    [SerializeField] private AudioClip[] PlatformClips;
    private float footsteptimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : isSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed; // get actual footstep time while crouching, sprinting

    [Header("Player Sound Parameters")]
    [SerializeField] private AudioSource _playeraudiosource;
     

    [Header("Pick up objects Parameters")]
    


    //SLIDING PARAMETERS
    private Vector3 hitPointNormal;
    private bool isSliding
    {
        get //check if raycast is hitting the slope when it is above the degrees we want it to be, then slide down
        {
            if (charactercontroller.isGrounded && Physics.Raycast(transform.position, Vector3.down , out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > charactercontroller.slopeLimit;
            }

            else
            {
                return false;
            }
        }
    }
    ////////

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interaction currentInteractable;

    ////////
    private Camera PlayerCamera;
    private CharacterController charactercontroller;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;
    public static FPSCharacter instance;


    /////////////METHODS////////////////////

    void Awake()
    {
        //Cache variables 
        PlayerCamera = GetComponentInChildren<Camera>();
        charactercontroller = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        //position variable for returning camera to default position when not moving [headbob]
        defaultYPos = PlayerCamera.transform.localPosition.y;

        //lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        instance = this;
    }

    void Update()
    {
        if (CanPause)
        {
           isPaused = !isPaused;
           Time.timeScale = isPaused ? 0 : 1;

            if(isPaused) HUD.GetInstance().ShowControls();
            if (!isPaused) HUD.GetInstance().HideControls();
        }

        ///movement methods
        if (CanMove)
        {
            HandleMovement();
            HandleMouseLook();

            if (canJump)
            {
                HandleJump();
            }

            if (canCrouch)
            {
                HandleCrouch();
            }

            if (CanHeadbob)
            {
                HandleHeadbob();
            }

            if (useFootsteps)
            {
                HandleFootsteps();
            } 

            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            ApplyFinalMovements();
        }
    }

    /// <summary>
    /// Check when interacting with an object and gaining focus, calculated with raycast 
    /// </summary>
    /// 
    void HandleInteractionCheck()
    {
        //looking at an object that is interactable for the first time and gaining focus
        if (Physics.Raycast(PlayerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 9 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {

                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }

        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    /// <summary>
    /// pressing E, if the currentinteractable is not null and if the raycast is hitting something in the interactable layer - intracting with it is possible
    /// </summary>
    void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null && Physics.Raycast(PlayerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
    }

    /// <summary>
    /// Code for vertical and horizontal movement of the player
    /// </summary>
    void HandleMovement()
    {
        currentInput = new Vector2((isCrouching ? crouchspeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"),
        //vertical movement, turnary operators check for sprint and crouch

        (isCrouching ? crouchspeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        //horizontal  movement

        float movedirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = movedirectionY;
    }

    void HandleJump()
    {
        if (ShouldJump)
        {
            moveDirection.y = JumpForce;
        }
    }

    void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }


    /// <summary>
    /// Footstep handling, depending on the surface you walk on, and on the speed you step on (crouch, walk, sprint).
    /// Using tags and audioclips the sounds are played randomly. 
    /// </summary>

    void HandleFootsteps()
    {
        if (!charactercontroller.isGrounded) return;
        if (currentInput == Vector2.zero) return;

        footsteptimer -= Time.deltaTime;

        if(footsteptimer <= 0)
        {
            if (Physics.Raycast(charactercontroller.transform.position, Vector3.down, out RaycastHit hit, 4))
            {
                switch (hit.collider.tag) //play footstep sound depending on the area you're stepping on - TAGS TO DEFINE THE GROUND
                {
                    case "Footsteps/Sand":
                        m_audiosource.PlayOneShot(SandClips[UnityEngine.Random.Range(0, SandClips.Length - 1)]);
                        break;

                    case "Footsteps/Rocks":
                        m_audiosource.PlayOneShot(RockClips[UnityEngine.Random.Range(0, RockClips.Length - 1)]);
                        break;
                    default:
                        break;
                }
            }
            footsteptimer = GetCurrentOffset;
        }        
    }


    /// <summary>
    /// headbob, by transforming the position of the camera, the values are different for sprinting, walking and crouching. 
    /// Can be changed in the inspector. Camera resets to the default y position in order to not stay at a different y value while walking, sprinting 
    /// or crouching.
    /// </summary>
    void HandleHeadbob()
    {
        if (!charactercontroller.isGrounded)
        {
            return;
        }

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchbobspeed : isSprinting ? sprintbobspeed : walkbobspeed);

            PlayerCamera.transform.localPosition =
                new Vector3(PlayerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchbobamount : isSprinting ? sprintbobamount : walkbobamount),
                PlayerCamera.transform.localPosition.z);
        }

        else if (defaultYPos != PlayerCamera.transform.localPosition.y)
        {
            timer += Time.deltaTime * (isCrouching ? crouchbobspeed : isSprinting ? sprintbobspeed : walkbobspeed);
            PlayerCamera.transform.localPosition = new Vector3(PlayerCamera.transform.localPosition.x, defaultYPos, PlayerCamera.transform.localPosition.z);
        }
    }


    /// <summary>
    /// handles the mouse movement, locks the player at a 80 degree angle upwards and downwards in order to prevent the player from doing a 360 degrees
    /// circle using the camera.
    /// </summary>
    void HandleMouseLook()
    {
        if (!isPaused) 
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit); //clamp camera to look at 80 degrees up and down
            PlayerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
        }
    }

    /// <summary>
    /// Includes code to not build speedup during falling, gravity and slope functionality for when falling from a slope and trying to get up a slope
    /// </summary>
    void ApplyFinalMovements()
    {

        if(charactercontroller.velocity.y < -1 && charactercontroller.isGrounded) //reset movedirection.y value on landing to avoid falling speed buildup
        {
            moveDirection.y = 0;
        }


        if (!charactercontroller.isGrounded)
        { 
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if(SlideOnSlope && isSliding) //slope functionality
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopespeed;
        }

        if (charactercontroller.enabled) //for errors when disabling the character controller during cutscenes
        {
            charactercontroller.Move(moveDirection * Time.deltaTime);
        }
    }


   /// <summary>
   /// checks for when the player can stand up from crouching using raycast, if something is above the players head do not allow to stand up.
   /// Changes the values of the character controllers height and center, in order to shrink the model correctly to fit the crouching animation.
   /// </summary>
   
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(PlayerCamera.transform.position, Vector3.up, 1f)) //using raycast check if anything is above the player 
        {
            yield break; //if there is something above the player, do not allow to stand in order to not clip
        }

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = charactercontroller.height;
        Vector3 targetcenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentcenter = charactercontroller.center;

        while (timeElapsed < TimetoCrouch)
        {
            charactercontroller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / TimetoCrouch);
            charactercontroller.center = Vector3.Lerp(currentcenter, targetcenter, timeElapsed / TimetoCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        charactercontroller.height = targetHeight;
        charactercontroller.center = targetcenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }


    /// <summary>
    /// Code for stamina
    /// </summary>

    /*
    private void HandleStamina()
    {
        if (isSprinting)
        {
            if(regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }

            currentStamina -= StaminaUseMultiplier * Time.deltaTime;

            if(currentStamina < 0)
            { 
                currentStamina = 0;
            }

            OnStaminaChange?.Invoke(currentStamina);

            if (currentStamina <= 0)
            {
                canSprint = false;
            }
        }

        if(!isSprinting && currentStamina < maxStamina && regeneratingStamina == null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }


    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds TimeToWait = new WaitForSeconds(staminaTimeIncrement);

        while(currentStamina < maxStamina)
        {
            if (currentStamina > 0)
                canSprint = true;

            currentStamina += StaminaValueIncrement;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;

            OnStaminaChange?.Invoke(currentStamina);

            yield return TimeToWait;
        }

        regeneratingStamina = null;
    }
    */
}
