using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	// Instance of event manager
	[SerializeField]
    private EventManager _events;
    private FollowPlayerCamera _followPlayer;
    private CinematicCamera _cinematicCamera;

    [SerializeField]
    // Distance to the desired point
    private float _distance;
    [SerializeField]
    // Height of the camera
    private float _height;
    [SerializeField]
    private float _cameraSequenceDuration;

    private GameObject _emptyLookAtObj;
    private GameObject _emptyDesiredPosition;
    [SerializeField]
    private Vector2 _cinematicOffset;
    [SerializeField]
    private float _cinematicRotation;

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

    private void Start () {
        _followPlayer = GetComponent<FollowPlayerCamera>();
        _cinematicCamera = GetComponent<CinematicCamera>();

        _events.onEnterState += OnEnterState;
        _events.onExitState += OnExitState;
        _events.onPlayerGrounded += OnPlayerGrounded;
        _events.onPlayerPushed += OnPush;
        _events.onAttack += OnAttack;
        _events.onEnemyDeath += OnEnemyDeath;

        _emptyDesiredPosition = new GameObject();
        _emptyLookAtObj = new GameObject();

        _cinematicCamera.enabled = false;
        _followPlayer.enabled = true;
    }

    private void OnDestroy()
    {
        if(_emptyDesiredPosition != null)
            Destroy(_emptyDesiredPosition);
        if (_emptyLookAtObj != null)
            Destroy(_emptyLookAtObj);
    }

    private IEnumerator WaitEndCameraSequence()
    {
        yield return new WaitForSecondsRealtime(_cameraSequenceDuration);

        _cinematicCamera.newTargetLookAt = _followPlayer.target;
        _cinematicCamera.lookAtDamping = 5f;
        _cinematicCamera.targetLookOffset = _followPlayer.targetLookOffset;
        _cinematicCamera.rotation = _followPlayer.rotation;

        yield return new WaitForSecondsRealtime(0.5f);

        _cinematicCamera.enabled = false;
        _followPlayer.enabled = true;
    }
	
    private void CalculateCinematicParams(Transform originObject, Transform destObject)
    {
        if (originObject == null || destObject == null)
            return;

        Vector3 targetLook = (originObject.position + destObject.position) / 2f;
        
        // Vector from player to enemy
        Vector3 objsVec = Vector3.Normalize(originObject.position - destObject.position);
        // Calculate the size of the horizontal vector
        float hSize = Mathf.Sqrt(Mathf.Pow(_distance, 2f) - Mathf.Pow(_height, 2f));
        float angle = Vector3.SignedAngle(objsVec, transform.position - originObject.position, Vector3.up);
        if (angle >= 0f)
            hSize *= -1f;

        _emptyLookAtObj.transform.position = targetLook;
        _emptyLookAtObj.transform.LookAt(destObject);
        _emptyDesiredPosition.transform.position = _emptyLookAtObj.transform.position;
        _emptyDesiredPosition.transform.rotation = _emptyLookAtObj.transform.rotation;
        _emptyDesiredPosition.transform.Translate(new Vector3(hSize, _height, 0));
        _emptyDesiredPosition.transform.position = Vector3.Slerp(transform.position, _emptyDesiredPosition.transform.position, 0.3f);
        _emptyDesiredPosition.transform.LookAt(_emptyLookAtObj.transform);

        _cinematicCamera.targetLookOffset = _cinematicOffset;
        _cinematicCamera.rotation = _cinematicRotation;

        _cinematicCamera.targetPosition = _emptyDesiredPosition.transform;
        _cinematicCamera.newTargetLookAt = _emptyLookAtObj.transform;
    }

    private void OnPlayerGrounded()
    {
        _followPlayer.CameraShake(_shakeForceGrounded, _shakeDurationGrounded);
    }

    private void OnPush(GameObject player, GameObject enemy)
    {
        if (_cinematicCamera.enabled)
            StopAllCoroutines();

        CalculateCinematicParams(player.transform, enemy.transform);

        /*
        _focusPoint.originObject = player.transform;
        _focusPoint.destObject = enemy.transform;
        _focusPoint.ignoreTimeScale = true;
        _focusPoint.height = 2f;
        _focusPoint.distance = 3f;
        */

        _cinematicCamera.enabled = true;
        _followPlayer.enabled = false;
        
        StartCoroutine(WaitEndCameraSequence());
    }

    private void OnAttack()
    {
        _followPlayer.CameraShake(_shakeForceAttack, _shakeDurationAttack);
    }

    private void OnEnemyDeath()
    {
        _followPlayer.CameraShake(_shakeForceEnemyDeath, _shakeDurationEnemyDeath);
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
        /*switch (state)
        {
            case GameStatus.CAMERA_SEQUENCE:
                StopAllCoroutines();
                _focusPoint.enabled = false;
                _followPlayer.enabled = true;
                break;
        }*/
    }
}
