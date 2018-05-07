using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// States of the game
public enum GameStatus { RUNNING, PAUSED, MIMIC, CAMERA_SEQUENCE, INVENTORY_SELECTION };

public class GameManager : MonoBehaviour
{
    private GameStatus _state = GameStatus.RUNNING;

    [SerializeField]
    private EventManager _events;
    [SerializeField]
    private MimicController _mimic;

    // Delegate funtion that calls right update function according to the current state
    private UpdateMonoBehavior onUpdate;

    [SerializeField]
    private float _cameraSequenceDuration;
    [SerializeField]
    [Range(0, 1)]
    private float _cameraSequenceSlowScale;
    [SerializeField]
    private float _returnToTimeDuration;
    [SerializeField]
    private GameObject _gameUI;
    [SerializeField]
    private GameObject _pauseUI;
    [SerializeField]
    private GameObject _inventoryUI;

    private void Start()
    {
        _events.onEnterState += OnEnterState;
        _events.onExitState += OnExitState;
        _events.onPlayerPushed += OnPlayerPushed;

        _mimic.enabled = false;

        onUpdate += FirstFrame;
    }

    private void Update()
    {
        if (onUpdate != null)
            onUpdate();
    }


    private void FirstFrame()
    {
        _events.onEnterState(GameStatus.RUNNING);
        onUpdate -= FirstFrame;
    }

    private void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    #region UpdateMethods

    private void UpdateRunning()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState(GameStatus.PAUSED);
        }
    }

    private void UpdateGenericMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeState(GameStatus.RUNNING);
        }
    }

    #endregion

    #region Camera

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

    #endregion

    #region Events

    private void OnPlayerPushed(GameObject player, GameObject enemy)
    {
        ChangeState(GameStatus.CAMERA_SEQUENCE);
        StartCoroutine(WaitEndCameraSequence());
    }

    #endregion
    
    #region State

    private void OnEnterState(GameStatus state)
    {
        switch (state)
        {
            case GameStatus.PAUSED:
                onUpdate += UpdateGenericMenu;

                Time.timeScale = 0f;
                _pauseUI.SetActive(true);

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
            case GameStatus.MIMIC:
                _mimic.enabled = true;
                break;
            case GameStatus.INVENTORY_SELECTION:
                onUpdate += UpdateGenericMenu;

                Time.timeScale = 0f;
                _inventoryUI.SetActive(true);

                Cursor.visible = true;
                break;
        }
    }

    private void OnExitState(GameStatus state)
    {
        switch (state)
        {
            case GameStatus.PAUSED:
                onUpdate -= UpdateGenericMenu;

                Time.timeScale = 1f;
                _pauseUI.SetActive(false);
                break;
            case GameStatus.RUNNING:
                onUpdate -= UpdateRunning;
                break;
            case GameStatus.CAMERA_SEQUENCE:
                onUpdate -= UpdateRunning;
                SetTimeScale(1f);
                break;
            case GameStatus.MIMIC:
                _mimic.enabled = false;
                break;
            case GameStatus.INVENTORY_SELECTION:
                onUpdate -= UpdateGenericMenu;

                Time.timeScale = 1f;
                _inventoryUI.SetActive(false);
                break;
        }
    }

    public void ChangeState(GameStatus nSate)
    {
        _events.onExitState(_state);
        _events.onEnterState(nSate);
        _state = nSate;
    }

    public void StartMimic(Transform mimicEmitter)
    {
        _mimic._mimicEmitter = mimicEmitter;
        ChangeState(GameStatus.MIMIC);
    }

    #endregion

    #region Quit

    public void Quit()
    {
        OnExitState(_state);
        Application.Quit();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #endregion
}
