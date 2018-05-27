using UnityEngine;

public class LinearPlatformController : ScriptActivator
{
    private enum MovementType { Invert, Repeat, Destroy };
    [SerializeField]
    private MovementType _type;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Vector3 _startPosition;
    [SerializeField]
    private Vector3 _endPosition;

    [SerializeField]
    private Rigidbody _rigidbody;

    private float _length;
    private float _startTime;

    private void Start() { 
        _startTime = Time.time;
        _length = Vector3.Distance(_startPosition, _endPosition);
    }

    private void MovePosition(Vector3 localPosition)
    {
        // Calculate world position
        Vector3 worldPos = transform.TransformPoint(localPosition);

        // Use rigidbody if possible
        if (_rigidbody != null)
            _rigidbody.MovePosition(worldPos);
        else
            transform.position = worldPos;
    }

    private void InvertMovement()
    {
        float distanceCovered = (Time.time - _startTime) * _speed;
        float fracJourney = distanceCovered / _length;

        float floor = Mathf.Floor(fracJourney);
        float actualFrac = fracJourney - floor;
        Vector3 localPosition;

        if (floor % 2 == 0)
            localPosition = Vector3.Lerp(_startPosition, _endPosition, actualFrac);
        else
            localPosition = Vector3.Lerp(_endPosition, _startPosition, actualFrac);

        MovePosition(localPosition);
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

        MovePosition(Vector3.Lerp(_startPosition, _endPosition, actualFrac));
    }

    private void DestroyMovement()
    {
        float distanceCovered = (Time.time - _startTime) * _speed;

        if (distanceCovered > _length)
        {
            Destroy(gameObject);
        }

        float fracJourney = distanceCovered / _length;

        MovePosition(Vector3.Lerp(_startPosition, _endPosition, fracJourney));
    }

    protected override void Run()
    {
        switch (_type)
        {
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
