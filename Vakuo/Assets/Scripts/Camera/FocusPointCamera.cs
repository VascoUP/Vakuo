using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPointCamera : MonoBehaviour {

    public Transform originObject;
    public Transform destObject;

    public float distance;
    public float height;
    public float rotation;
    public bool ignoreTimeScale;
    public float speed;
    
    private Vector3 _targetLookPoint;
    private Vector3 _targetPosition;

    [SerializeField]
    private float _damping;
    [SerializeField]
    private float _bumperDistanceCheck;
    [SerializeField]
    private float _bumperCameraHeight;
    [SerializeField]
    private Vector3 _bumperRayOffset;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (originObject == null || destObject == null)
            return;

        _targetLookPoint = (originObject.position + destObject.position) / 2f;

        Vector3 objsVec = Vector3.Normalize(originObject.position - destObject.position);
        Vector3 hVector = Vector3.right;
        hVector.z = (-objsVec.x * hVector.x - objsVec.y * hVector.y) / objsVec.z;
        hVector.Normalize();

        float hSize = Mathf.Sqrt(Mathf.Pow(distance, 2f) - Mathf.Pow(height, 2f));
        hVector *= hSize;

        _targetPosition = hVector + Vector3.up * height + _targetLookPoint;

        Debug.Log(_targetLookPoint);
        Debug.Log(_targetPosition);
    }

    private void LookAtPoint()
    {
        float deltaTime = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        
        Vector3 wantedPosition = _targetPosition;
        Vector3 lookAtPosition = _targetLookPoint;

        transform.position = Vector3.Lerp(transform.position, wantedPosition, deltaTime * _damping);

        // Smoothly rotate towards the target point.
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * deltaTime);
    }

    private void LateUpdate ()
    {
        if (originObject == null || destObject == null)
            return;
        LookAtPoint();
    }
}
