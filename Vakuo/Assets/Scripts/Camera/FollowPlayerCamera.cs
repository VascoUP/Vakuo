using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{    
    // Instance of event manager
    private EventManager _events;
    
    public Transform target;
    private AstronautController _astronautController;

    [SerializeField]
    private float _distance;
    [SerializeField]
    private float _minHeight;
    [SerializeField]
    private float _maxHeight;
    private float _headToCamRatio;
    [SerializeField]
    private float _positionDamping;
    
    public float rotation;

    public Vector2 targetLookOffset;

    [SerializeField]
    private float _bumperDistanceCheck;
    [SerializeField]
    private float _bumperCameraHeight;
    [SerializeField]
    private Vector3 _bumperRayOffset;

    private bool _isShaking = false;

    private IEnumerator _zoomEffect;
    // (x: Distance, y: Min Height, z: Max Height)
    private Vector3 _targetZoom;

    private void Start()
    {
        _targetZoom = new Vector3(_distance, _minHeight, _maxHeight);

        _astronautController = target.GetComponent<AstronautController>();
        if(_astronautController != null)
        {
            float maxDeltaHead = _astronautController.headMaxRotation - _astronautController.headMinRotation;
            float maxDeltaCam = _maxHeight - _minHeight;
            _headToCamRatio = maxDeltaCam / maxDeltaHead;
        }
    }

    private void FollowPlayer()
    {
        if (_astronautController != null)
        {
            rotation = _astronautController.currentHeadRotation * _headToCamRatio + _headToCamRatio + _minHeight;
        }

        Vector3 wantedPosition = target.TransformPoint(0, rotation, -_distance);
        Vector3 lookAtPosition = target.transform.position;

        // check to see if there is anything behind the target
        Vector3 direction = wantedPosition - target.TransformPoint(_bumperRayOffset);
        RaycastHit hit;

        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(
            target.TransformPoint(_bumperRayOffset), direction, out hit, 
            _bumperDistanceCheck, ~(1 << LayerMask.NameToLayer("Trigger")))
            && hit.transform != target) // ignore ray-casts that hit the user. DR
        {
            float dist = Vector3.Distance(target.TransformPoint(_bumperRayOffset), hit.point);
            float wantedDist = Vector3.Distance(target.TransformPoint(_bumperRayOffset), wantedPosition);
            if (dist < wantedDist)
            {
                // clamp wanted position to hit position
                wantedPosition.x = hit.point.x;
                wantedPosition.z = hit.point.z;
            }

        }

        transform.Translate(-targetLookOffset.x, -targetLookOffset.y, 0);
        transform.position = Vector3.Lerp(transform.position, wantedPosition, _positionDamping * Time.deltaTime);

        transform.LookAt(lookAtPosition);
        
        transform.Translate(targetLookOffset.x, targetLookOffset.y, 0);
    }


    #region Effects
    private bool EndZoomEffect(float value, float target)
    {
        return Mathf.Approximately(value, target);
    }

    private IEnumerator ZoomEffect(float deltaDistance, float deltaHeight, float clamp)
    {
        _targetZoom = new Vector3(
            _targetZoom.x + deltaDistance,
            _targetZoom.y + deltaHeight,
            _targetZoom.z + deltaHeight);

        while (!EndZoomEffect(_targetZoom.x, _distance) ||
            !EndZoomEffect(_targetZoom.y, _minHeight) ||
            !EndZoomEffect(_targetZoom.z, _maxHeight))
        {
            _distance = Mathf.Lerp(_distance, _targetZoom.x, clamp * Time.unscaledDeltaTime);
            _minHeight = Mathf.Lerp(_minHeight, _targetZoom.y, clamp * Time.unscaledDeltaTime);
            _maxHeight = Mathf.Lerp(_maxHeight, _targetZoom.z, clamp * Time.unscaledDeltaTime);
            yield return null;
        }

        _distance = _targetZoom.x;
        _minHeight = _targetZoom.y;
        _maxHeight = _targetZoom.z;
    }

    public void MimicZoomIn()
    {
        if(_zoomEffect != null)
        {
            StopCoroutine(_zoomEffect);
        }

        _zoomEffect = ZoomEffect(-2f, -2f, 2f);
        StartCoroutine(_zoomEffect);
    }

    public void MimicZoomOut()
    {
        if (_zoomEffect != null)
        {
            StopCoroutine(_zoomEffect);
        }

        _zoomEffect = ZoomEffect(2f, 2f, 2f);
        StartCoroutine(_zoomEffect);
    }

    public void CaveZoomIn()
    {
        if (_zoomEffect != null)
        {
            StopCoroutine(_zoomEffect);
        }

        _zoomEffect = ZoomEffect(-4f, -2f, 2f);
        StartCoroutine(_zoomEffect);
    }

    public void CaveZoomOut()
    {
        if (_zoomEffect != null)
        {
            StopCoroutine(_zoomEffect);
        }

        _zoomEffect = ZoomEffect(4f, 2f, 2f);
        StartCoroutine(_zoomEffect);
    }
    #endregion

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
