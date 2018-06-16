using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MimicController : MonoBehaviour {
    private enum MimicStatus { ANIMAL_TURN, PLAYER_TURN, END };
    private MimicStatus _state;

    public UnityEvent onStart;
    public UnityEvent onEnd;

    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private EntityInventory _entityInventory;

    [SerializeField]
    private string[]  _buttonNames;
    [SerializeField]
    private GameObject[] _visualSoundPrefabs;
    [SerializeField]
    private float _timeBetweenSounds;

    public AnimalAction _mimicEmitter;
    public Vector3 _spawnOffset = new Vector3(2f, 1f, 0f); //Vector3.left;

    [Range(1,5)]
    public int numberOfLives;

    private int _lives;

    [Range(1, 10)]
    public int numberOfTurns;
    [Range(1, 10)]
    public int initialNumberOfSounds;
    [Range(1, 10)]
    public int numberOfSoundsIncreaseAmount;
    private int _numberOfSounds;

    private int _turn;

    private List<int> _mimicSounds;
    private List<int> _playerSounds;

    #region UIFields

    [SerializeField]
    private GameObject _mimicPanel;

    [SerializeField]
    private Text _turnsText;

    [SerializeField]
    private Text _announcerText;

    #endregion

    private void Start()
    {
        _state = MimicStatus.END;
    }
    
    private void OnEnable()
    {
        _turn = 0;
        _lives = numberOfLives;
        _state = MimicStatus.ANIMAL_TURN;
        _numberOfSounds = initialNumberOfSounds;

        InteractionController ic = _mimicEmitter.GetComponent<InteractionController>();
        if (ic != null)
            ic.enabled = false;

        SetActiveUI(true);

        onStart.Invoke();
        OnAnimalTurn();
    }

    private void OnDisable()
    {
        SetActiveUI(false);
        StopAllCoroutines();
    }
    
    private void Update ()
    {
		if(_state == MimicStatus.PLAYER_TURN)
        {
            CheckPlayerKeys();
        }
	}
    
    private void OnAnimalTurn()
    {
        _state = MimicStatus.ANIMAL_TURN;

        _turn++;
        if (_turn > 1)
        {
            _numberOfSounds += numberOfSoundsIncreaseAmount;
        }
        if(_turn > numberOfTurns)
        {
            MimicSuccess();
            return;
        }

        if (_playerSounds == null)
        {
            _playerSounds = new List<int>();
        }
        else
        {
            _playerSounds.Clear();
        }

        GenerateTurnSound(_numberOfSounds);

        StartCoroutine(PlayAnimalSounds());
    }

    private void OnPlayerTurn()
    {
        _state = MimicStatus.PLAYER_TURN;
    }
    
    private IEnumerator EndMimic(bool success)
    {
        SetText(_announcerText, success ? "Success" : "Fail");
        AnnounceActive(true);

        yield return new WaitForSeconds(2f);
        AnnounceActive(false);

        End();
    }

    private IEnumerator WarningTurnFailure()
    {
        SetText(_announcerText, "Turn failed");
        AnnounceActive(true);

        yield return new WaitForSeconds(2f);
        AnnounceActive(false);

        OnAnimalTurn();
    }

    private IEnumerator PlayAnimalSounds()
    {
        AnnouncerText(MimicStatus.ANIMAL_TURN);
        AnnounceActive(true);

        yield return new WaitForSeconds(_timeBetweenSounds * 2);
        AnnounceActive(false);
        TurnsText();

        foreach (int entry in _mimicSounds)
        {
            SpawnVisualClue(entry);
            yield return new WaitForSeconds(_timeBetweenSounds);
        }

        AnnouncerText(MimicStatus.PLAYER_TURN);
        AnnounceActive(true);

        yield return new WaitForSeconds(_timeBetweenSounds * 2);
        AnnounceActive(false);

        OnPlayerTurn();
    }

    private void FailedTurn()
    {
        if (--_lives <= 0)
        {
            MimicFailure();
        }
        else
        {
            _turn = 0;
            _state = MimicStatus.ANIMAL_TURN;
            _numberOfSounds = initialNumberOfSounds;

            StartCoroutine(WarningTurnFailure());
        }
    }

    private void MimicSuccess()
    {
        _entityInventory.AddAnimal(_mimicEmitter.GetAnimalBasicItem());
        _mimicEmitter.MimicSuccess();
        _state = MimicStatus.END;
        StartCoroutine(EndMimic(true));
    }

    private void MimicFailure()
    {
        _mimicEmitter.MimicFailure();
        _state = MimicStatus.END;
        InteractionController ic = _mimicEmitter.GetComponent<InteractionController>();
        if (ic != null)
            ic.enabled = true;
        StartCoroutine(EndMimic(false));
    }

    private void End()
    {
        if (_gameManager != null)
        {
            onEnd.Invoke();
            _gameManager.ChangeState(GameStatus.RUNNING);
        }
    }

    private void CheckPlayerKeys()
    {
        int i = 0;
        foreach(string entry in _buttonNames)
        {
            if(Input.GetButtonDown(entry))
            {
                OnPlayerSoundPlayed(i);
                if (_state != MimicStatus.PLAYER_TURN)
                    return;
                else if (_playerSounds.Count == _mimicSounds.Count)
                {
                    OnAnimalTurn();
                    return;
                }
            }

            i++;
        }
    }

    private void GenerateTurnSound(int numberOfKeys)
    {
        if (_mimicSounds == null)
        {
            _mimicSounds = new List<int>();
        }
        else
        {
            _mimicSounds.Clear();
        }

        for(int i = 0; i < numberOfKeys; i++)
        {
            _mimicSounds.Add(GetRandomSound());
        }
    }

    private void OnPlayerSoundPlayed(int sound)
    {
        int addedIndex = _playerSounds.Count;
        _playerSounds.Add(sound);

        SpawnVisualClue(sound);
        if (addedIndex < _mimicSounds.Count && _playerSounds[addedIndex] == _mimicSounds[addedIndex])
        {
            return;
        }

        FailedTurn();
    }

    private int GetRandomSound()
    {
        return Random.Range(0, _buttonNames.Length);
    }

    private void SpawnVisualClue(int index)
    {
        GameObject spawnedObj = Instantiate(_visualSoundPrefabs[index], _mimicEmitter.transform);
        spawnedObj.transform.Translate(_spawnOffset);
    }
    
    #region UI

    private void SetActiveUI(bool active)
    {
        if (_mimicPanel != null)
        {
            _mimicPanel.SetActive(active);
            if (active)
            {
                TurnsText();
            }
        }
    }

    private void AnnounceActive(bool active)
    {
        _announcerText.gameObject.SetActive(active);
    }

    private void SetText(Text text, string message)
    {
        text.text = message;
    }

    private void AnnouncerText(MimicStatus state)
    {
        string message;
        switch(state)
        {
            case MimicStatus.ANIMAL_TURN:
                message = "Animal";
                break;
            case MimicStatus.PLAYER_TURN:
                message = "Player";
                break;
            default:
                return;
        }

        message += " turn";
        SetText(_announcerText, message);
    }

    private void TurnsText()
    {
        SetText(_turnsText, _turn + " out of " + numberOfTurns);
    }

    #endregion
}
