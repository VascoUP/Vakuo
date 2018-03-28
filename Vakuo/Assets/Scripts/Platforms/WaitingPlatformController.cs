using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPlatformController : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _wait;
    [SerializeField]
    private Vector3 _startPosition;
    [SerializeField]
    private Vector3 _endPosition;
    
    private float _startTime;
    private float _length;
    private float _lengthInTime;
    private float _totalTime;
    private float _fracMoveTime;

    void Start()
    {
        _startTime = Time.time;
        _length = Vector3.Distance(_startPosition, _endPosition);
        _lengthInTime = _length / _speed;
        _totalTime = _wait + _lengthInTime;
        _fracMoveTime = _lengthInTime / _totalTime;
    }

    private void LoopMovement()
    {
        if (_lengthInTime == 0)
            return;

        float timeSpent = (Time.time - _startTime);
        float fracJourney = timeSpent / _totalTime;

        float floor = Mathf.Floor(fracJourney);
        float actualFrac = fracJourney - floor;

        Vector3 posInit;
        Vector3 posEnd;

        if (floor % 2 == 0)
        {
            posInit = _startPosition;
            posEnd = _endPosition;
        }
        else
        {
            posInit = _endPosition;
            posEnd = _startPosition;
        }

        bool isWaitTime = actualFrac >= _fracMoveTime;
        if (isWaitTime)
            transform.position = posEnd;

        float moveFrac = (timeSpent - (floor * _totalTime)) / _lengthInTime;
        transform.position = Vector3.Lerp(posInit, posEnd, moveFrac);
    }

    void Update()
    {
        LoopMovement();
    }
}

