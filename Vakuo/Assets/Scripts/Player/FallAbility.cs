using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAbility : MonoBehaviour {
    [SerializeField]
    private EventManager _events;
    [SerializeField]
    private AstronautController _astronautController;
    [SerializeField]
    private CharacterController _characterController;

    [SerializeField]
    private float _fallForce;
    private bool _isActive = false;
    private float _originalGravity;
    
    private void Start()
    {
        _events.onPlayerGrounded += PlayerGrounded;
        _originalGravity = _astronautController.gravity;
    }

    private void ResetGravity()
    {
        _astronautController.gravity = _originalGravity;
        _isActive = false;
    }

    private void PlayerGrounded()
    {
        ResetGravity();
    }

    private bool IsAirborn()
    {
        //if (_previousFrameGrounded)
        //{
            if (!_characterController.isGrounded)
            {
                //_previousFrameGrounded = false;
                //_highestPoint = transform.position.y;

                //if(_highestPoint > 0.5f)
                    return true;
            }
        //}
        return false;
    }

    private void Ability()
    {
        if(Input.GetButtonDown("Jump"))
        {
            _astronautController.gravity = _fallForce;
            _isActive = true;
        }
    }

    private void FixedUpdate ()
    {
		if(IsAirborn() && !_isActive)
        {
            Ability();
        }
        else if(_isActive && _characterController.isGrounded)
        {
            ResetGravity();
        }
	}
}
