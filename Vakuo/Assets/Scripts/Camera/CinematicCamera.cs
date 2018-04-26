using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCamera : MonoBehaviour {
    public Transform originalTargetLookAt;
    public Transform newTargetLookAt;
    public Transform targetPosition;

    // Tell if it should or not ignore the time scale
    public bool ignoreTimeScale;
    
    public Vector2 targetLookOffset;
    public float rotation;

    public float positionDamping;

    public float lookAtDamping;

    private void LookAtPoint()
    {
        // Delta time based on scaling or not
        float deltaTime = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

        Vector3 desiredPosition = targetPosition.position;
        Vector3 lookAtPosition = newTargetLookAt.position;

        transform.Rotate(Vector3.up, -rotation);
        transform.Translate(-targetLookOffset.x, -targetLookOffset.y, 0);

        // Smoothly move to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionDamping * deltaTime);

        // Smoothly rotate towards the target point
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookAtDamping * deltaTime);

        transform.Translate(targetLookOffset.x, targetLookOffset.y, 0);
        transform.Rotate(Vector3.up, rotation);
    }

    private void LateUpdate () {
        LookAtPoint();

    }
}
