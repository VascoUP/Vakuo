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
        ChangeState(GameStatus.RUNNING);
    }

    private void OnPlayerPushed(GameObject enemy)
    {
        ChangeState(GameStatus.CAMERA_SEQUENCE);
        StartCoroutine(WaitEndCameraSequence());
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
                Time.timeScale = 0.5f;
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
                Time.timeScale = 1f;
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
