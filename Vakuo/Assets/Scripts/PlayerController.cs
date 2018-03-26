using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private enum MovementState { IDLE, AIRBORN };
    private MovementState _status = MovementState.AIRBORN;

    private Rigidbody _rigidbody;

    private Rigidbody _ridingRigidbody;
    private Vector3 _ridingObjectOffset;

    private PlayerControls _controls;
    
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private float _walkForce;
    [SerializeField]
    [Range(0, 2)]
    private float _airbornModifier;
    [SerializeField]
    [Range(1, 10)]
    private float _jumpModifier;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _controls = new PlayerControls();
    }

    private void Jump(float additionalYForce)
    {
        float radAng = transform.eulerAngles.y * Mathf.Deg2Rad;

        float cosAng = Mathf.Cos(radAng);
        float sinAng = Mathf.Sin(radAng);

        float xdirection = (sinAng * _controls.straightAxis + cosAng * _controls.sideAxis) * _walkForce;
        float ydirection = _jumpForce;
        float zdirection = (cosAng * _controls.straightAxis - sinAng * _controls.sideAxis) * _walkForce;

        if (_ridingRigidbody != null)
        {
            xdirection += _ridingRigidbody.velocity.x * 0.8f;
            ydirection += _ridingRigidbody.velocity.y * 0.5f;
            zdirection += _ridingRigidbody.velocity.z * 0.8f;
        }

        _rigidbody.velocity = new Vector3(xdirection, ydirection, zdirection);

        //_status = MovementState.AIRBORN;
    }

    private void Walk()
    {
        float radAng = transform.eulerAngles.y * Mathf.Deg2Rad;

        float cosAng = Mathf.Cos(radAng);
        float sinAng = Mathf.Sin(radAng);

        float xdirection = (sinAng * _controls.straightAxis + cosAng * _controls.sideAxis) * _walkForce;
        float ydirection = _walkForce;
        float zdirection = (cosAng * _controls.straightAxis - sinAng * _controls.sideAxis) * _walkForce;

        if (_ridingRigidbody != null)
        {
            xdirection += _ridingRigidbody.velocity.x * 0.8f;
            ydirection += _ridingRigidbody.velocity.y * 0.8f;
            Debug.Log(ydirection);
            zdirection += _ridingRigidbody.velocity.z * 0.8f;
        }

        _rigidbody.velocity = new Vector3(xdirection, ydirection, zdirection);

        //_status = MovementState.AIRBORN;
    }
    
    private void Update () {
        _controls.UpdateValues();
        
        if(_status == MovementState.IDLE)
        {
            if (_controls.jump)
            {
                Jump(1f);
            }
            else if (_controls.sideAxis != 0f || _controls.straightAxis != 0f)
            {
                Walk();
            }
            else if (_ridingRigidbody != null)
            {
                //_rigidbody.MovePosition(_ridingRigidbody.transform.position + _ridingObjectOffset);
                transform.position = _ridingRigidbody.transform.position + _ridingObjectOffset;
            }
        }
        else if(_status == MovementState.AIRBORN)
        {
            float radAng = transform.eulerAngles.y * Mathf.Deg2Rad;

            float cosAng = Mathf.Cos(radAng);
            float sinAng = Mathf.Sin(radAng);

            float xdirection = sinAng * _controls.straightAxis + cosAng * _controls.sideAxis;
            float zdirection = cosAng * _controls.straightAxis - sinAng * _controls.sideAxis;
            _rigidbody.AddForce(new Vector3(xdirection * _walkForce * _airbornModifier, 0, zdirection * _walkForce * _airbornModifier));
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        _status = MovementState.IDLE;

        if(collision.gameObject.tag == "Planet")
        {
            Vector3 direction = transform.position - collision.transform.position;
            transform.rotation = Quaternion.FromToRotation (Vector3.up, direction);
        }

        if (collision.gameObject.tag == "Plataform")
        {
            _ridingRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            _ridingObjectOffset = -collision.transform.position + transform.position;
        }

        if (_controls.jump)
        {
            Jump(0);
        }
        else if (_controls.sideAxis != 0f || _controls.straightAxis != 0f)
        {
            Walk();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Plataform")
        {
            _ridingRigidbody = null;
        }

        _status = MovementState.AIRBORN;
    }
}

public class PlayerControls
{
    public bool jump = false;
    public float straightAxis = 0f;
    public float sideAxis = 0f;

    public void UpdateValues()
    {
        jump = Input.GetButton("Jump");
        straightAxis = Input.GetAxis("Vertical");
        sideAxis = Input.GetAxis("Horizontal");
    }
}

