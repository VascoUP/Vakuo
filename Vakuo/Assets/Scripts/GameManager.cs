using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// States of the game
public enum GameStatus { RUNNING, PAUSED, MIMIC };

// Delegate funtion for when the games enters a new state
public delegate void StateEnter(GameStatus enterState);
// Delegate funtion for when the games exits a state
public delegate void StateExit(GameStatus exitState);
// Delegate funtion that used to call different update funtions
public delegate void UpdateMonoBehavior();

public class GameManager : MonoBehaviour {
    private GameStatus _state = GameStatus.RUNNING;

    public StateEnter onEnterState;
    public StateExit onExitState;

    // Delegate funtion that calls right update function according to the current state
    private UpdateMonoBehavior onUpdate;

    [SerializeField]
    public GameObject _pauseMenu;

    private void Start()
    {
        onEnterState += OnEnterState;
        onExitState += OnExitState;

        onUpdate += FirstFrame;
    }

    private void Update () {
        onUpdate();
	}

    private void FirstFrame()
    {
        onEnterState(GameStatus.RUNNING);
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
        }
    }

    public void ChangeState(GameStatus nSate)
    {
        onExitState(_state);
        onEnterState(nSate);
        _state = nSate;
    }

    public void Quit()
    {
        OnExitState(_state);
        Application.Quit();
    }
}
