using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{    
    // Instance of event manager
    private EventManager _events;

    [SerializeField]
    private Transform _target;
    private AstronautController _astronautController;

    [SerializeField]
    private float _distance;
    // Used if no astronaut controller is found
    [SerializeField]
    private float _rotation; 
    [SerializeField]
    private float _minHeight;
    [SerializeField]
    private float _maxHeight;
    private float __headToCamRatio;
    [SerializeField]
    private float _damping;

    [SerializeField]
    private Vector2 _targetLookOffset;

    [SerializeField]
    private float _bumperDistanceCheck;
    [SerializeField]
    private float _bumperCameraHeight;
    [SerializeField]
    private Vector3 _bumperRayOffset;

    [SerializeField]
    private float _shakeDurationGrounded;
    [SerializeField]
    private float _shakeForceGrounded;
    [SerializeField]
    private float _shakeDurationPush;
    [SerializeField]
    private float _shakeForcePush;
    [SerializeField]
    private float _shakeDurationAttack;
    [SerializeField]
    private float _shakeForceAttack;
    [SerializeField]
    private float _shakeDurationEnemyDeath;
    [SerializeField]
    private float _shakeForceEnemyDeath;

    private bool _isShaking = false;

    public float shakeAmount;//The amount to shake this frame.
    public float shakeDuration;//The duration this frame.

    private float _shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    private float _startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    private float _startDuration;//The initial shake duration, set when ShakeCamera is called.
    
    public bool smooth;//Smooth rotation?
    public float smoothAmount = 5f;//Amount to smooth

    private void Start()
    {
        _events = Utils.GetComponentOnGameObject<EventManager>("Game Manager");
        //_events.onEnterState += OnEnterState;
        //_events.onExitState += OnExitState;
        _events.onPlayerGrounded += OnPlayerGrounded;
        _events.onPlayerPushed += OnPush;
        _events.onAttack += OnAttack;
        _events.onEnemyDeath += OnEnemyDeath;

        _astronautController = _target.GetComponent<AstronautController>();
        if(_astronautController != null)
        {
            float maxDeltaHead = _astronautController.headMaxRotation - _astronautController.headMinRotation;
            float maxDeltaCam = _maxHeight - _minHeight;
            __headToCamRatio = maxDeltaCam / maxDeltaHead;
        }
    }

    private void OnPlayerGrounded()
    {
        CameraShake(_shakeForceGrounded, _shakeDurationGrounded);
    }

    private void OnPush(GameObject enemy)
    {
        //CameraShake(_shakeForcePush, _shakeDurationPush);
    }

    private void OnAttack()
    {
        CameraShake(_shakeForceAttack, _shakeDurationAttack);
    }

    private void OnEnemyDeath()
    {
        CameraShake(_shakeForceEnemyDeath, _shakeDurationEnemyDeath);
    }

    private void OnEnterState(GameStatus state)
    {
        switch (state)
        {
            case GameStatus.CAMERA_SEQUENCE:
                break;
        }
    }

    private void OnExitState(GameStatus state)
    {
        switch (state)
        {
            case GameStatus.CAMERA_SEQUENCE:
                break;
        }
    }

    IEnumerator Shake()
    {
        _isShaking = true;

        Vector2 startOffset = new Vector3(_targetLookOffset.x, _targetLookOffset.y);

        while (shakeDuration > 0.01f)
        {
            _targetLookOffset = startOffset;

            Vector2 rotationAmount = Random.insideUnitCircle * shakeAmount;//A Vector3 to add to the Local Rotation

            _shakePercentage = shakeDuration / _startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = _startAmount * _shakePercentage;//Set the amount of shake (% * startAmount).
            shakeDuration -= Time.deltaTime;

            if (smooth)
                _targetLookOffset = Vector3.Lerp(_targetLookOffset, rotationAmount, Time.deltaTime * smoothAmount);
            else
                _targetLookOffset += rotationAmount;//Set the local rotation the be the rotation amount.

            yield return null;
        }
        //transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        _targetLookOffset = startOffset;

        _isShaking = false;
    }

    public void CameraShake(float force, float duration)
    {
        shakeAmount += force;//Add to the current amount.
        _startAmount = shakeAmount;//Reset the start amount, to determine percentage.
        shakeDuration += duration;//Add to the current time.
        _startDuration = shakeDuration;//Reset the start time.

        if (!_isShaking) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }

    private void FollowPlayer()
    {
        if(_astronautController != null)
        {
            _rotation = -_astronautController.currentHeadRotation * __headToCamRatio + __headToCamRatio + _minHeight;
        }

        Vector3 wantedPosition = _target.TransformPoint(0, _rotation, -_distance);

        // check to see if there is anything behind the target
        RaycastHit hit;
        Vector3 back = _target.transform.TransformDirection(-1 * Vector3.forward);

        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(_target.TransformPoint(_bumperRayOffset), back, out hit, _bumperDistanceCheck)
            && hit.transform != _target) // ignore ray-casts that hit the user. DR
        {
            // clamp wanted position to hit position
            wantedPosition.x = hit.point.x;
            wantedPosition.z = hit.point.z;
            wantedPosition.y = Mathf.Lerp(hit.point.y + _bumperCameraHeight, wantedPosition.y, Time.deltaTime * _damping);
        }

        transform.Translate(-_targetLookOffset.x, -_targetLookOffset.y, 0);
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * _damping);
        //transform.position = wantedPosition;
        
        transform.LookAt(_target.transform);
        transform.Translate(_targetLookOffset.x, _targetLookOffset.y, 0);
        transform.Rotate(Vector3.up, _rotation);
    }
    

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
