using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearPlataformController : MonoBehaviour {
    private enum MovementType { Invert, Repeat, Destroy };
    [SerializeField]
    private MovementType _type;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Vector3 _startPosition;
    [SerializeField]
    private Vector3 _endPosition;

    private Rigidbody _rigidbody;

    private float _length;
    private float _startTime;

    // Use this for initialization
    void Start () {
        _rigidbody = GetComponent<Rigidbody>();

        _startTime = Time.time;
        _length = Vector3.Distance(_startPosition, _endPosition);
    }

    private void InvertMovement()
    {
        float distanceCovered = (Time.time - _startTime) * _speed;
        float fracJourney = distanceCovered / _length;

        float floor = Mathf.Floor(fracJourney);
        float actualFrac = fracJourney - floor;
        if (floor % 2 == 0)
            _rigidbody.MovePosition(Vector3.Lerp(_startPosition, _endPosition, actualFrac));
        else
            _rigidbody.MovePosition(Vector3.Lerp(_endPosition, _startPosition, actualFrac));
    }

    private void RepeatMovement()
    {
        float distanceCovered = (Time.time - _startTime) * _speed;

        if(distanceCovered > _length)
        {
            _startTime = Time.time - (distanceCovered - _length) / _speed;
            distanceCovered = (Time.time - _startTime) * _speed;
        }

        float fracJourney = distanceCovered / _length;
        float floor = Mathf.Floor(fracJourney);
        float actualFrac = fracJourney - floor;
        _rigidbody.MovePosition(Vector3.Lerp(_startPosition, _endPosition, actualFrac));
    }

    private void DestroyMovement()
    {
        float distanceCovered = (Time.time - _startTime) * _speed;

        if (distanceCovered > _length)
        {
            Destroy(gameObject);
        }

        float fracJourney = distanceCovered / _length;
        _rigidbody.MovePosition(Vector3.Lerp(_startPosition, _endPosition, fracJourney));
    }

    // Update is called once per frame
    void FixedUpdate () {
        switch(_type) {
            case MovementType.Invert:
                InvertMovement();
                break;
            case MovementType.Repeat:
                RepeatMovement();
                break;
            case MovementType.Destroy:
                DestroyMovement();
                break;
        }
    }
}
