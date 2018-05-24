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
    private float __headToCamRatio;
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


    private IEnumerator zoomEffect;

    private void Start()
    {
        _astronautController = target.GetComponent<AstronautController>();
        if(_astronautController != null)
        {
            float maxDeltaHead = _astronautController.headMaxRotation - _astronautController.headMinRotation;
            float maxDeltaCam = _maxHeight - _minHeight;
            __headToCamRatio = maxDeltaCam / maxDeltaHead;
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
            rotation = _astronautController.currentHeadRotation * __headToCamRatio + __headToCamRatio + _minHeight;
        }

        Vector3 wantedPosition = target.TransformPoint(0, rotation, -_distance);
        Vector3 lookAtPosition = target.transform.position;

        // check to see if there is anything behind the target
        RaycastHit hit;
        Vector3 back = target.transform.TransformDirection(-1 * Vector3.forward);

        // cast the bumper ray out from rear and check to see if there is anything behind
        if (Physics.Raycast(target.TransformPoint(_bumperRayOffset), back, out hit, _bumperDistanceCheck, LayerMask.NameToLayer("Trigger"))
            && hit.transform != target) // ignore ray-casts that hit the user. DR
        {
            // clamp wanted position to hit position
            wantedPosition.x = hit.point.x;
            wantedPosition.z = hit.point.z;
            wantedPosition.y = Mathf.Lerp(hit.point.y + _bumperCameraHeight, wantedPosition.y, _positionDamping * Time.deltaTime);
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
        float targetDistance = _distance + deltaDistance;
        float targetMinHeight = _minHeight + deltaHeight;
        float targetMaxHeight = _maxHeight + deltaHeight;

        while(!EndZoomEffect(targetDistance, _distance) ||
            !EndZoomEffect(targetMinHeight, _minHeight) ||
            !EndZoomEffect(targetMaxHeight, _maxHeight))
        {
            _distance = Mathf.Lerp(_distance, targetDistance, clamp * Time.unscaledDeltaTime);
            _minHeight = Mathf.Lerp(_minHeight, targetMinHeight, clamp * Time.unscaledDeltaTime);
            _maxHeight = Mathf.Lerp(_maxHeight, targetMaxHeight, clamp * Time.unscaledDeltaTime);
            yield return null;
        }

        _distance = targetDistance;
        _minHeight = targetMinHeight;
        _maxHeight = targetMaxHeight;
    }

    public void ZoomIn(/*float deltaDistance, float deltaHeight*/)
    {
        if(zoomEffect != null)
        {
            StopCoroutine(zoomEffect);
        }

        zoomEffect = ZoomEffect(-2f, -2f, 2f);
        StartCoroutine(zoomEffect);
    }

    public void ZoomOut(/*float deltaDistance, float deltaHeight*/)
    {
        if (zoomEffect != null)
        {
            StopCoroutine(zoomEffect);
        }

        zoomEffect = ZoomEffect(2f, 2f, 2f);
        StartCoroutine(zoomEffect);
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
