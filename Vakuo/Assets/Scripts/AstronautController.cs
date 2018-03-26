using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for the player's game object
public class AstronautController : MonoBehaviour {
    // Character controller component of the astronaut game object
    private CharacterController _cc;
    // Responsible for handling player controls
    private AstonautControls _controls;
    
    // Head of the astronaut
    [SerializeField]
    private Transform _head;

    // Walk speed of the astronaut (per second)
    [SerializeField]
    private float _walkSpeed;
    // Jump speed of the astronaut (per second)
    [SerializeField]
    private float _jumpSpeed;
    // Max speed of the astronaut on the X and Z axis during jump (per second)
    [SerializeField]
    private float _jumpMaxSpeed;
    // Mid air control ratio compared to walkSpeed 
    [SerializeField]
    [Range(0, 0.1f)]
    private float _midAirControlRatio;
    // Current velocity of the astronaut
    private Vector3 _velocity = Vector3.zero;
    // Indicates if the astronaut is jumping in a frame
    private bool _isJump = false;

    // Acceleration downward of the astronaut while in the air
    [SerializeField]
    private float _gravity = 15.0f;

    // Speed at which the astronaut turns 
    [SerializeField]
    [Range(0, 1)]
    private float _aimSpeed;
    // Max rotation in a frame
    [SerializeField]
    private float _aimMaxRotation;
    [SerializeField]
    // Lower boudry of rotation in a frame 
    private float _aimMinRotation;
    // Max rotation of the head
    public float headMaxRotation;
    // Lower boudry of rotation of the head in a frame 
    public float headMinRotation;
    // Head rotation in a frame
    public float currentHeadRotation = 0;

    void Start () {
        //_rb = GetComponent<Rigidbody>();
        _cc = GetComponent<CharacterController>();
        _controls = new AstonautControls();
	}

    // Jumps the astronaut up in the air with a given force.
    // Public so it can be called by other scripts
    public void Jump(float speed)
    {
        _isJump = true;
        _velocity.y = speed;
    }

    // Updates the boolean isGrounded by checking if the feet are colliding with an object that is ground
    private void UpdateYVelocity()
    {
        if (!_isJump && _cc.isGrounded)
        {
            _velocity.y = 0f;
        }
        else
            _velocity.y -= _gravity * Time.deltaTime;
    }

    // Rotates the player according to mouse input
    private void Aim()
    {
        _controls.mouseInput *= _aimSpeed;

        _controls.mouseInput.x = Mathf.Clamp(_controls.mouseInput.x, _aimMinRotation, _aimMaxRotation);
        transform.Rotate(0, _controls.mouseInput.x, 0);

        currentHeadRotation =
            Mathf.Clamp(currentHeadRotation + _controls.mouseInput.y, headMinRotation, headMaxRotation);

        _head.localRotation = Quaternion.identity;
        _head.Rotate(Vector3.right, currentHeadRotation);
    }

    // Checks for input and isGrounded and decides if character should jump or not
    private void Jump()
    {
        if (_controls.jump && _cc.isGrounded && !_isJump)
        {
            Jump(_jumpSpeed);
        }
    }
    
    // Moves the astronaut in the world according to input
    private void Move()
    {
        Vector3 moveVelocity = Vector3.zero;
        Vector3 input = new Vector3(_controls.sideAxis, 0, _controls.straightAxis);

        if (_cc.isGrounded)
        {
            moveVelocity = transform.TransformDirection(input * _walkSpeed);
        }
        else
        {
            // Player will be able to change his velocity mid air but on a smaller scale
            moveVelocity = transform.TransformDirection(input * _walkSpeed * _midAirControlRatio) + new Vector3(_velocity.x, 0, _velocity.z);
            // Keep move velocity on a grounded magnitude
            moveVelocity = Vector3.ClampMagnitude(moveVelocity, _jumpMaxSpeed);
        }


        _velocity = moveVelocity + Vector3.up * _velocity.y;
        
        _cc.Move(_velocity * Time.deltaTime);

        _isJump = false;
    }

    private void Update()
    {
        _controls.UpdateValues();
        UpdateYVelocity();
        Aim();
        Jump();
        Move();
    }
}

// Class dedicated to storing importante values regarding player controls
public class AstonautControls
{
    public bool jump = false;
    public float straightAxis = 0f;
    public float sideAxis = 0f;
    public Vector2 mouseInput;

    // Updates input values for all the character's keys
    public void UpdateValues()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        jump = Input.GetButton("Jump");
        straightAxis = Input.GetAxisRaw("Vertical");
        sideAxis = Input.GetAxisRaw("Horizontal");
    }
}

