using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    private float _saveTime;

    private Vector3 _posInit;
    private Vector3 _posEnd;

    void Start()
    {
        _startTime = Time.time;
        _length = Vector3.Distance(_startPosition, _endPosition);
        _lengthInTime = _length / _speed;
        _totalTime = _wait + _lengthInTime;
        _fracMoveTime = _lengthInTime / _totalTime;
        _saveTime = 0;

        GlobalControl.SaveEvent += Save;
        GlobalControl.LoadEvent += Load;
    }

    private void LoopMovement()
    {
        if (_lengthInTime == 0)
            return;

        float timeSpent = (Time.time - _saveTime - _startTime);
        float fracJourney = timeSpent / _totalTime;

        float floor = Mathf.Floor(fracJourney);
        float actualFrac = fracJourney - floor;

        // Vector3 posInit;
        // Vector3 posEnd;

        if (floor % 2 == 0)
        {
            _posInit = _startPosition;
            _posEnd = _endPosition;
        }
        else
        {
            _posInit = _endPosition;
            _posEnd = _startPosition;
        }

        bool isWaitTime = actualFrac >= _fracMoveTime;
        if (isWaitTime)
            transform.position = _posEnd;

        float moveFrac = (timeSpent - (floor * _totalTime)) / _lengthInTime;
        transform.position = Vector3.Lerp(_posInit, _posEnd, moveFrac);
    }

    void Update()
    {
        LoopMovement();
    }

    private void Save(object sender, EventArgs args)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/waitingPlatformInfo.dat");

        WaitingPlatformData data = new WaitingPlatformData();
        data.time = Time.time;
        data.posInitX = _posInit.x;
        data.posInitY = _posInit.y;
        data.posInitZ = _posInit.z;

        data.posEndX = _posEnd.x;
        data.posEndY = _posEnd.y;
        data.posEndZ = _posEnd.z;

        bf.Serialize(file, data);
        file.Close();
    }

    private void Load(object sender, EventArgs args)
    {
        if(File.Exists(Application.persistentDataPath + "/waitingPlatformInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/waitingPlatformInfo.dat", FileMode.Open);
            WaitingPlatformData data = (WaitingPlatformData)bf.Deserialize(file);
            file.Close();

            _saveTime = data.time;
            _posEnd = new Vector3(data.posInitX, data.posInitY, data.posInitZ);
            _posInit = new Vector3(data.posEndX, data.posEndY, data.posEndZ);

        }
    }
}

[Serializable]
class WaitingPlatformData
{
    public float time;
    public float posInitX;
    public float posInitY;
    public float posInitZ;
    public float posEndX;
    public float posEndY;
    public float posEndZ;
}

