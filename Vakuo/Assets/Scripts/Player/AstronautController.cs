using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Controller for the player's game object
public class AstronautController : MonoBehaviour {
	// Instance of event manager
	[SerializeField]
    private EventManager _events;
    
    // Delegate funtion that calls right update function according to the current state
    private UpdateMonoBehavior onUpdate;

    // Character controller component of the astronaut game object
    private CharacterController _cc;
    
	// Feet of the astronaut
    [SerializeField]
    private Transform _feet;
    // Check distance
    [SerializeField]
    private float _feetRadius = 0.05f;
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
    private bool _isJumpFrame = false;

    // Wait for a push effect to be over
    private IEnumerator waitPush;
    // Indicates if a push effect is taking place
    private bool _isPushed = false;
    [SerializeField]
    private float _pushMinTime = 0.5f;
    [SerializeField]
    private float _clampPushMagnitude = 10f;

    // Acceleration downward of the astronaut while in the air
    public float gravity = 15.0f;

    // Speed at which the astronaut turns 
    [SerializeField]
    private float _aimSpeed;
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

    private void Start () {
        _events.onEnterState += OnEnterState;
        _events.onExitState += OnExitState;

        _cc = GetComponent<CharacterController>();

        GlobalControl.SaveEvent += Save;
        GlobalControl.LoadEvent += Load;
	}

    private bool IsEndPushed()
    {
        return (_cc.isGrounded || _ridingPlatform != null);
    }

    private IEnumerator WaitEndPush(float pushTime)
    {
        _isPushed = true;
        // Wait for minimim cooldown
        yield return new WaitForSeconds(pushTime);
        // Wait for player to be grounded
        yield return new WaitUntil(IsEndPushed);

        Vector2 clampVector = new Vector2(_velocity.x, _velocity.z);
        float magnitude = clampVector.magnitude;
        while (clampVector.magnitude != 0f)
        {
            // Calculate clamped vector
            magnitude -= _clampPushMagnitude * Time.deltaTime;
            if (magnitude <= 0)
                magnitude = 0f;
            clampVector = Vector2.ClampMagnitude(clampVector, magnitude);

            // Set velocity
            _velocity.x = clampVector.x;
            _velocity.z = clampVector.y;

            yield return null;
        }

        _isPushed = false;
    }

    private float DistanceToGround() {
        RaycastHit hit;
        if(Physics.Raycast(_feet.position, Vector3.down, out hit, 2f, ~(1 << gameObject.layer))) {
            Vector3 diff = _feet.position - hit.point;
            return diff.magnitude;
        }
        return float.PositiveInfinity;
    }


    // Checks if the astronaut is touching a platform or not using Raycast
    private void CheckPlatform()
    {
        float sphereRadius = 0.4f;
        RaycastHit hit;
        if (Physics.SphereCast(_feet.position + Vector3.up * sphereRadius, sphereRadius, Vector3.down, out hit, _feetRadius, _platformLayer))
        {
            if (_ridingPlatform == null || hit.collider.transform.GetInstanceID() != _ridingPlatform.GetInstanceID())
            {
                _ridingPlatform = hit.collider.transform;
                _ridingOffset = transform.position - _ridingPlatform.position;
            }
        }
        else if(_ridingPlatform != null)
        {
            // Let the player continue with the momentum from the platform
            Vector3 desiredPosition = _ridingOffset + _ridingPlatform.position;
            Vector3 delta = desiredPosition - transform.position;
            
            delta = delta / Time.deltaTime;
            _velocity += delta;

            _ridingPlatform = null;
        }
    }
    
    public void Push(float pushSpeed, float ySpeed, Vector3 direction)
    {
        _isJumpFrame = true;
        _velocity = new Vector3(pushSpeed * direction.x, ySpeed * direction.y, pushSpeed * direction.z);

        if(_isPushed)
        {
            StopCoroutine(waitPush);
        }

        waitPush = WaitEndPush(_pushMinTime);
        StartCoroutine(waitPush);
    }

    // Jumps the astronaut up in the air with a given force.
    // Public so it can be called by other scripts
    public void Jump(float speed)
    {
        _isJumpFrame = true;
        _velocity.y = speed;
    }

    // Updates the boolean isGrounded by checking if the feet are colliding with an object that is ground
    private void UpdateYVelocity()
    {
        if (!_isJumpFrame && (_cc.isGrounded || _ridingPlatform != null))
        {
            _velocity.y = 0f;
        }
        else
            _velocity.y -= gravity * Time.deltaTime;
    }

    // Rotates the player according to mouse input
    private void Aim()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseInput *= _aimSpeed;
        
        transform.Rotate(Vector3.up * mouseInput.x * Time.deltaTime);

        currentHeadRotation =
            Mathf.Clamp(currentHeadRotation - (mouseInput.y * Time.deltaTime), headMinRotation, headMaxRotation);
		/*
        _head.localRotation = Quaternion.identity;
        _head.Rotate(Vector3.right * currentHeadRotation);
        */
    }

    // Checks for input and isGrounded and decides if character should jump or not
    private void Jump()
    {
        if (Input.GetButton("Jump") && (_cc.isGrounded || _ridingPlatform != null) && !_isJumpFrame)
        {
            Jump(_jumpSpeed);
        }
    }
    
    // Moves the astronaut in the world according to input
    private void Move()
    {
        Vector3 moveVelocity = Vector3.zero;
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
        if(!_isPushed)
        {
            if (_cc.isGrounded || _ridingPlatform != null)
            {
                moveVelocity = transform.TransformDirection(input * _walkSpeed);
            }
            else if(_ridingPlatform == null)
            {
                // Player will be able to change his velocity mid air but on a smaller scale
                moveVelocity = transform.TransformDirection(input * _walkSpeed * _midAirControlRatio) + new Vector3(_velocity.x, 0, _velocity.z);
                moveVelocity = Vector3.ClampMagnitude(moveVelocity, _jumpMaxSpeed);
            }

            _velocity = moveVelocity + Vector3.up * _velocity.y;
        }
        
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

        _isJumpFrame = false;
    }

    private void Update()
    {
        if(onUpdate != null)
            onUpdate();
    }

    private void UpdateRunning()
    {
        CheckPlatform();
        UpdateYVelocity();
        Aim();
        Jump();
        Move();
    }

    private void OnEnterState(GameStatus state)
    {
        switch (state)
        {
            case GameStatus.RUNNING:
            case GameStatus.CAMERA_SEQUENCE:
                onUpdate += UpdateRunning;
                break;
        }
    }

    private void OnExitState(GameStatus state)
    {
        switch (state)
        {
            case GameStatus.RUNNING:
            case GameStatus.CAMERA_SEQUENCE:
                onUpdate -= UpdateRunning;
                break;
        }
    }

    private void OnDestroy()
    {
        _events.onEnterState -= OnEnterState;
        _events.onExitState -= OnExitState;

        StopAllCoroutines();
    }

    private void Save(object sender, EventArgs args)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.positionX = gameObject.transform.position.x;
        data.positionY = gameObject.transform.position.y;
        data.positionZ = gameObject.transform.position.z;

        bf.Serialize(file, data);
        file.Close();
    }

    private void Load(object sender, EventArgs args)
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            gameObject.transform.position = new Vector3(data.positionX, data.positionY, data.positionZ);
        }
    }
}

[Serializable]
public class PlayerData
{
    public float positionX;
    public float positionY;
    public float positionZ;
}
