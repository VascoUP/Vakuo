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
    private float _shakeDuration;
    [SerializeField]
    private float _shakeForceGrounded;
    [SerializeField]
    private float _shakeForcePush;
    [SerializeField]
    private float _shakeForceAttack;
    [SerializeField]
    private float _shakeForceEnemyDeath;
    private float _shakeForce;
    private bool _isShaking = false;

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

    IEnumerator WaitCameraShake()
    {
        _isShaking = true;
        yield return new WaitForSeconds(_shakeDuration);
        _isShaking = false;
    }

    private void OnPlayerGrounded()
    {
        CameraShake(_shakeForceGrounded);
    }

    private void OnPush(GameObject enemy)
    {
        CameraShake(_shakeForcePush);
    }

    private void OnAttack()
    {
        CameraShake(_shakeForceAttack);
    }

    private void OnEnemyDeath()
    {
        CameraShake(_shakeForceEnemyDeath);
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

    public void CameraShake(float force)
    {
        if(_isShaking)
        {
            StopAllCoroutines();
        }
        _shakeForce = force;
        StartCoroutine(WaitCameraShake());
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

        //transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.fixedDeltaTime * _damping);
        transform.position = wantedPosition;
        
        transform.LookAt(_target.transform);
        transform.Translate(_targetLookOffset.x, _targetLookOffset.y, 0);
        transform.Rotate(Vector3.up, _rotation);
    }

    private void ShakeCamera()
    {
        transform.localPosition += Random.insideUnitSphere * _shakeForce;
    }

    private void LateUpdate()
    {
        FollowPlayer();
        if(_isShaking)
        {
            ShakeCamera();
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
