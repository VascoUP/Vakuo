using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// States of the game
public enum GameStatus { RUNNING, PAUSED, MIMIC, CAMERA_SEQUENCE };

public class GameManager : MonoBehaviour {
    private GameStatus _state = GameStatus.RUNNING;

    private EventManager _events;

    // Delegate funtion that calls right update function according to the current state
    private UpdateMonoBehavior onUpdate;

    [SerializeField]
    private float _cameraSequenceDuration;
    [SerializeField]
    [Range(0,1)]
    private float _cameraSequenceSlowScale;
    [SerializeField]
    private float _returnToTimeDuration;
    [SerializeField]
    private GameObject _pauseMenu;

    private void Start()
    {
        _events = GetComponent<EventManager>();

        _events.onEnterState += OnEnterState;
        _events.onExitState += OnExitState;
        _events.onPlayerPushed += OnPlayerPushed;

        onUpdate += FirstFrame;
    }

    private void Update () {
        if(onUpdate != null)
            onUpdate();
	}

    private void FirstFrame()
    {
        _events.onEnterState(GameStatus.RUNNING);
        onUpdate -= FirstFrame;
    }

    private void UpdateRunning()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState(GameStatus.PAUSED);
        }
    }

    private void UpdatePaused()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState(GameStatus.RUNNING);
        }
    }

    IEnumerator WaitEndCameraSequence()
    {
        yield return new WaitForSecondsRealtime(_cameraSequenceDuration);
        float startScale = Time.timeScale;
        float duration = 0f;
        float lerpScale = 1f / _returnToTimeDuration;
        while (Time.timeScale < 1f)
        {
            duration += Time.unscaledDeltaTime;
            float nextTimeScale = Mathf.Lerp(startScale, 1f, duration * lerpScale);
            SetTimeScale(nextTimeScale);
            yield return null;
        }
        ChangeState(GameStatus.RUNNING);
    }

    private void OnPlayerPushed(GameObject player, GameObject enemy)
    {
        ChangeState(GameStatus.CAMERA_SEQUENCE);
        StartCoroutine(WaitEndCameraSequence());
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    private void OnEnterState(GameStatus state)
    {
        switch(state)
        {
            case GameStatus.PAUSED:
                onUpdate += UpdatePaused;

                Time.timeScale = 0f;
                _pauseMenu.SetActive(true);

                Cursor.visible = true;
                break;
            case GameStatus.RUNNING:
                onUpdate += UpdateRunning;

                Cursor.visible = false;
                break;
            case GameStatus.CAMERA_SEQUENCE:
                onUpdate += UpdateRunning;
                SetTimeScale(_cameraSequenceSlowScale);
                break;
        }
    }

    private void OnExitState(GameStatus state)
    {
        switch (state)
        {
            case GameStatus.PAUSED:
                onUpdate -= UpdatePaused;

                Time.timeScale = 1f;
                _pauseMenu.SetActive(false);
                break;
            case GameStatus.RUNNING:
                onUpdate -= UpdateRunning;
                break;
            case GameStatus.CAMERA_SEQUENCE:
                onUpdate -= UpdateRunning;
                SetTimeScale(1f);
                break;
        }
    }

    public void ChangeState(GameStatus nSate)
    {
        _events.onExitState(_state);
        _events.onEnterState(nSate);
        _state = nSate;
    }

    public void Quit()
    {
        OnExitState(_state);
        Application.Quit();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
