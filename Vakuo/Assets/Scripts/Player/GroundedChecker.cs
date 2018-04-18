using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedChecker : MonoBehaviour {

    private EventManager _events;
    private CharacterController _cc;
    private AstronautController _ac;

    private bool _previousFrameGrounded = true;
    private float _highestPoint;

    [SerializeField]
    private float _minDistanceForEvent;

	void Start ()
    {
        _events = Utils.GetComponentOnGameObject<EventManager>("Game Manager");
        _cc = GetComponent<CharacterController>();
        _ac = GetComponent<AstronautController>();
    }

	void Update ()
    { 
		if(_previousFrameGrounded)
        {
            if(!_cc.isGrounded)
            {
                _previousFrameGrounded = false;
                _highestPoint = transform.position.y;
            }
        } else
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
