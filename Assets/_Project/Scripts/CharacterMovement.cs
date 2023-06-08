using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Cinemachine;

// Class used to handle single character movement
public class CharacterMovement : NetworkBehaviour
{
    // Stores character animator component
    Animator animator;

    // Velocity variables from the Animator. Stores optimized setter/getter parameters IDs
    private int xVelocityHash;
    private int yVelocityHash;

    // Speed at which the animations blend between each other
    [SerializeField] private float animBlendSpeed = 9f;

    [SerializeField] private Rigidbody playerRigidbody;

    // Object which will contain the Main Camera
    private GameObject characterFPSCamera;
    private CinemachineVirtualCamera _cinemachineVirtualCamera;


    // Stores reference to the instance of PlayerInput
    PlayerInput input;

    // Storing Player Input values
    Vector2 currentMovement;

    // Speed values
    [SerializeField] private const float walkSpeed = 2f;
    [SerializeField] private const float runSpeed = 4f;
    private Vector2 targetVelocity; // Represents the player desired velocity
    bool runPressed;

    void Awake(){
        if(characterFPSCamera == null){
            characterFPSCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        if(_cinemachineVirtualCamera == null){
            _cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        input = new PlayerInput();

        input.Movement.Movement.performed += onMove;
        input.Movement.Movement.canceled += onMoveCancelled;
       
        input.Movement.Run.performed += onRun;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the animator reference
        animator = GetComponent<Animator>();

        playerRigidbody = GetComponent<Rigidbody>();

        // Set the ID references
        xVelocityHash = Animator.StringToHash("xVelocity");
        yVelocityHash = Animator.StringToHash("yVelocity");
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {
        handleMovement();
        handleRotation();
    }

    //Checks if player wants to move, assigns input value to CurrentMovement
    void onMove(InputAction.CallbackContext ctx){
        currentMovement = ctx.ReadValue<Vector2>();
    }

    // Reset currentMovement when movement input is canceled
    // Small workaround that makes sure to reset to zero our movement
    // the ".performed" code should also handle setting the movement to zero, as it reads the vector2 value of the controller analog stick
    // But it keeps its previous value instead. I don't know why. It's as if it doesn't register the movement when its moved to neutral position unless i do this
    void onMoveCancelled(InputAction.CallbackContext ctx){
        currentMovement = Vector2.zero;
    }
    
    // Accessing "Run" action in PlayerInput, adding callback function to enable running by storing the button press in runPressed
    void onRun(InputAction.CallbackContext ctx){
        runPressed = ctx.ReadValueAsButton();
    }

    void handleMovement(){
        // if run is pressed we assign run speed, otherwise walk speed
        float targetSpeed = runPressed ? runSpeed : walkSpeed;
        // If nothing is pressed at all, then our speed should be 0 (or close to it?)
        if(currentMovement == Vector2.zero) targetSpeed = 0.1f;

        // Lerping our speed for smoother animation transition
        targetVelocity.x = Mathf.Lerp(targetVelocity.x, currentMovement.x * targetSpeed, animBlendSpeed * Time.fixedDeltaTime);
        targetVelocity.y = Mathf.Lerp(targetVelocity.y, currentMovement.y * targetSpeed, animBlendSpeed * Time.fixedDeltaTime);


        // We need the difference between the current velocity and out target velocity
        // Otherwise our rigidbody will forever accellerate
        float xVelocityDiff = targetVelocity.x - playerRigidbody.velocity.x;
        float yVelocityDiff = targetVelocity.y - playerRigidbody.velocity.y;

        // Applying velocity to our rigidbody through a force.
        playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelocityDiff, 0f, yVelocityDiff)), ForceMode.VelocityChange);

        // Set our animator velocity param to be the same as the velocity desired to update animations
        Debug.Log("x velocity target: " + xVelocityDiff + " y velocity target: " + yVelocityDiff);
        animator.SetFloat(xVelocityHash, targetVelocity.x);
        animator.SetFloat(yVelocityHash, targetVelocity.y);

    }

    // Sets character rotation to be the same as the main camera
    void handleRotation(){
        // Weird euler rotation dark magic
        transform.rotation = Quaternion.Euler( transform.eulerAngles.x, characterFPSCamera.transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void OnEnable(){
        // Enables "Movement" Action map inside PlayerInput
        input.Movement.Enable();
        input.Look.Enable();
    }

    void OnDisable(){
        // Disables "Movement" Action map inside PlayerInput
        input.Movement.Disable();
        input.Look.Disable();
    }

    // This is fired once a character spawns in the game from a connection
    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        OnEnable();

        // This code makes sure that if the object is a client and is the owner of the instance, we take control of the camera
        if(IsClient && IsOwner){
            _cinemachineVirtualCamera.Follow = this.transform.GetChild(0).transform;    // Gets the "Camera Ref" gameobject's transform
            _cinemachineVirtualCamera.LookAt = this.transform.GetChild(1).transform;    // Gets the "Look At" gameobject's transform
            _cinemachineVirtualCamera.GetComponent<CinemachineInputProvider>().XYAxis.action.Enable(); // Enabling cinemachine controls
        } 

        // To avoid controlling other people's characters
        // We disable Inputs on them
        // IsOwner is a Network Behaviour property. true if the object is the one assigned to the network client
        // False, if its another player object
        if(!IsOwner) input.Disable();

    }

}
