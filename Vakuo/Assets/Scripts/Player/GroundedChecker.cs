using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChecker : MonoBehaviour {

	[SerializeField]
    private EventManager _events;
    private CharacterController _cc;

    private bool _previousFrameGrounded = true;
    private float _highestPoint;

    [SerializeField]
    private float _minDistanceForEvent;

	private void Start ()
    {
        _cc = GetComponent<CharacterController>();
    }

	private void Update ()
    { 
		if(_previousFrameGrounded)
        {
            if(!_cc.isGrounded)
            {
                _previousFrameGrounded = false;
                _highestPoint = transform.position.y;
            }
        }
        else
        {
            if(_cc.isGrounded)
            {
                _previousFrameGrounded = true;

                float fallDistance = _highestPoint - transform.position.y;
                if(fallDistance >= _minDistanceForEvent)
                {
                    _events.onPlayerGrounded();
                }
            }
            else if(_highestPoint < transform.position.y)
            {
                _highestPoint = transform.position.y;
            }
        }
	}
}
