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

    public float shakeAmount;//The amount to shake this frame.
    public float shakeDuration;//The duration this frame.

    private float _shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    private float _startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    private float _startDuration;//The initial shake duration, set when ShakeCamera is called.
    
    public bool smooth;//Smooth rotation?
    public float smoothAmount = 5f;//Amount to smooth

    public bool drawLineRendered = false;
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;

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

    private IEnumerator Shake()
    {
        _isShaking = true;

        Vector2 startOffset = new Vector3(targetLookOffset.x, targetLookOffset.y);

        while (shakeDuration > 0.01f)
        {
            targetLookOffset = startOffset;

            Vector2 rotationAmount = Random.insideUnitCircle * shakeAmount;//A Vector3 to add to the Local Rotation

            _shakePercentage = shakeDuration / _startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = _startAmount * _shakePercentage;//Set the amount of shake (% * startAmount).
            shakeDuration -= Time.deltaTime;

            if (smooth)
                targetLookOffset = Vector3.Lerp(targetLookOffset, rotationAmount, Time.deltaTime * smoothAmount);
            else
                targetLookOffset += rotationAmount;//Set the local rotation the be the rotation amount.

            yield return null;
        }
        //transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        targetLookOffset = startOffset;

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
                //wantedPosition.y = Mathf.Lerp(hit.point.y + _bumperCameraHeight, wantedPosition.y, _positionDamping * Time.deltaTime);
                //wantedPosition = target.TransformPoint(0, rotation, -dist);
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

        _zoomEffect = ZoomEffect(-4f, -4f, 2f);
        StartCoroutine(_zoomEffect);
    }

    public void CaveZoomOut()
    {
        if (_zoomEffect != null)
        {
            StopCoroutine(_zoomEffect);
        }

        _zoomEffect = ZoomEffect(4f, 4f, 2f);
        StartCoroutine(_zoomEffect);
    }
    #endregion

    #region DrawLine

    void ShootLaserFromTargetPosition()
    {
        Vector3 direction = Vector3.Normalize(transform.position - target.transform.position);
        Vector3 endPosition = target.transform.position + (_bumperDistanceCheck * direction);
        laserLineRenderer.SetPosition(0, target.TransformPoint(_bumperRayOffset));
        laserLineRenderer.SetPosition(1, endPosition);
    }

    #endregion

    private void LateUpdate()
    {
        FollowPlayer();        
        
        // Draw line
        if (drawLineRendered)
        {
            ShootLaserFromTargetPosition();
            laserLineRenderer.enabled = true;
        }
        else
        {
            laserLineRenderer.enabled = false;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
