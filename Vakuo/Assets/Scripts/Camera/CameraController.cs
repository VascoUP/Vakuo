using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    private AstronautController _astronautController;

    [SerializeField]
    private float _distance;
    // Used if no astronaut controller is found
    [SerializeField]
    private float _height; 
    [SerializeField]
    private float _minHeight;
    [SerializeField]
    private float _maxHeight;
    private float __headToCamRatio;
    [SerializeField]
    private float _damping;

    [SerializeField]
    private Vector3 _targetLookAtOffset;

    [SerializeField]
    private float _bumperDistanceCheck;
    [SerializeField]
    private float _bumperCameraHeight;
    [SerializeField]
    private Vector3 _bumperRayOffset;

    private void Start()
    {
        _astronautController = _target.GetComponent<AstronautController>();
        if(_astronautController != null)
        {
            float maxDeltaHead = _astronautController.headMaxRotation - _astronautController.headMinRotation;
            float maxDeltaCam = _maxHeight - _minHeight;
            __headToCamRatio = maxDeltaCam / maxDeltaHead;
        }
    }

    private void FollowPlayer()
    {
        if(_astronautController != null)
        {
            _height = _astronautController.currentHeadRotation * __headToCamRatio + 4;
        }

        Vector3 wantedPosition = _target.TransformPoint(0, _height, -_distance);

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
        transform.Rotate(_targetLookAtOffset);
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }
}
