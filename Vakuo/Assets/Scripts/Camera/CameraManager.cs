using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    // Instance of event manager
    private EventManager _events;
    private FollowPlayerCamera _followPlayer;
    private FocusPointCamera _focusPoint;

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

    void Start () {
        _followPlayer = GetComponent<FollowPlayerCamera>();
        _focusPoint = GetComponent<FocusPointCamera>();

        _events = Utils.GetComponentOnGameObject<EventManager>("Game Manager");
        _events.onEnterState += OnEnterState;
        _events.onExitState += OnExitState;
        _events.onPlayerGrounded += OnPlayerGrounded;
        _events.onPlayerPushed += OnPush;
        _events.onAttack += OnAttack;
        _events.onEnemyDeath += OnEnemyDeath;

        _focusPoint.enabled = false;
        _followPlayer.enabled = true;
    }
	
    private void OnPlayerGrounded()
    {
        _followPlayer.CameraShake(_shakeForceGrounded, _shakeDurationGrounded);
    }

    private void OnPush(GameObject player, GameObject enemy)
    {
        _focusPoint.originObject = player.transform;
        _focusPoint.destObject = enemy.transform;
        _focusPoint.ignoreTimeScale = true;
        /*_focusPoint.height = 2f;
        _focusPoint.distance = 3f;*/

        _focusPoint.enabled = true;
        _followPlayer.enabled = false;
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
        switch (state)
        {
            case GameStatus.CAMERA_SEQUENCE:
                _focusPoint.enabled = false;
                _followPlayer.enabled = true;
                break;
        }
    }
}
