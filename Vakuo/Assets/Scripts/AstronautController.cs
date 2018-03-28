using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for the player's game object
public class AstronautController : MonoBehaviour {
    // Character controller component of the astronaut game object
    private CharacterController _cc;
    
    // Head of the astronaut
    [SerializeField]
    private Transform _head;
    // Feet of the astronaut
    [SerializeField]
    private Transform _feet;
    // Check distance
    private float _feetRadius = 0.2f;
    // Layer for the platforms
    [SerializeField]
    private LayerMask _platformLayer;

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

    // Rigidbody of the platform that the player is currently riding
    private Transform _ridingPlatform;
    // Offset from the astronaut to the platform
    private Vector3 _ridingOffset;

    void Start () {
        Cursor.visible = false;
        _cc = GetComponent<CharacterController>();
	}

    // Checks if the astronaut is touching a platform or not using Raycast
    private void CheckPlatform()
    {
        RaycastHit hit;
        if (Physics.Raycast(_feet.position, Vector3.down, out hit, _feetRadius, _platformLayer))
        {
            if (_ridingPlatform == null || hit.collider.transform.GetInstanceID() != _ridingPlatform.GetInstanceID())
            {
                _ridingPlatform = hit.collider.transform;
                _ridingOffset = transform.position - _ridingPlatform.position;
            }
        }
        else
        {
            if(_ridingPlatform != null)
            {
                // Let the player continue with the momentum from the platform
                Vector3 desiredPosition = _ridingOffset + _ridingPlatform.position;
                Vector3 delta = desiredPosition - transform.position;
                
                delta = delta / Time.deltaTime;
                _velocity += delta;
            }

            _ridingPlatform = null;
        }
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
        if (!_isJump && (_cc.isGrounded || _ridingPlatform != null))
        {
            _velocity.y = 0f;
        }
        else
            _velocity.y -= _gravity * Time.deltaTime;
    }

    // Rotates the player according to mouse input
    private void Aim()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseInput *= _aimSpeed;

        mouseInput.x = Mathf.Clamp(mouseInput.x, _aimMinRotation, _aimMaxRotation);
        transform.Rotate(0, mouseInput.x, 0);

        currentHeadRotation =
            Mathf.Clamp(currentHeadRotation + mouseInput.y, headMinRotation, headMaxRotation);

        _head.localRotation = Quaternion.identity;
        _head.Rotate(Vector3.right, currentHeadRotation);
    }

    // Checks for input and isGrounded and decides if character should jump or not
    private void Jump()
    {
        if (Input.GetButton("Jump") && (_cc.isGrounded || _ridingPlatform != null) && !_isJump)
        {
            Jump(_jumpSpeed);
        }
    }
    
    // Moves the astronaut in the world according to input
    private void Move()
    {
        Vector3 moveVelocity = Vector3.zero;
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
        if (_cc.isGrounded || _ridingPlatform != null)
        {
            moveVelocity = transform.TransformDirection(input * _walkSpeed);
        }
        else if(_ridingPlatform == null)
        {
            // Player will be able to change his velocity mid air but on a smaller scale
            moveVelocity = transform.TransformDirection(input * _walkSpeed * _midAirControlRatio) + new Vector3(_velocity.x, 0, _velocity.z);
            // Keep move velocity on a grounded magnitude
            moveVelocity = Vector3.ClampMagnitude(moveVelocity, _jumpMaxSpeed);
        }
        
        _velocity = moveVelocity + Vector3.up * _velocity.y;
        
        if(_ridingPlatform != null)
        {
            _ridingOffset += (_velocity * Time.deltaTime);

            Vector3 desiredPosition = _ridingOffset + _ridingPlatform.position;
            Vector3 delta = desiredPosition - transform.position;

            if (_velocity.y != 0)
            {
                _velocity.x += delta.x;
                _velocity.y += delta.z;
            }

            _cc.Move(delta);
        }
        else
        {
            _cc.Move(_velocity * Time.deltaTime);
        }

        _isJump = false;
    }

    private void Update()
    {
        CheckPlatform();
        UpdateYVelocity();
        Aim();
        Jump();
        Move();
    }
}