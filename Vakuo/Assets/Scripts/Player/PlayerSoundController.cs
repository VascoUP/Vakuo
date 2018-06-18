using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour {
	[SerializeField]
	private CharacterController cc;
	[SerializeField]
	private GameObject groundSound0;
	[SerializeField]
	private GameObject groundSound1;
	[SerializeField]
	private GameObject mountainSound0;
	[SerializeField]
	private GameObject mountainSound1;
	[SerializeField]
	private GameObject jumpSound0;
	[SerializeField]
	private GameObject jumpSound1;
	[SerializeField]
	private GroundedChecker grounded;
	
	private void OnEnable() {
		groundSound0.SetActive(false);
		groundSound1.SetActive(false);
		mountainSound0.SetActive(false);
		mountainSound1.SetActive(false);
	}

	private void Update() {
		if(grounded.IsGrounded()) {
			jumpSound0.SetActive(true);
			jumpSound0.SetActive(false);
			jumpSound1.SetActive(true);
			jumpSound1.SetActive(false);
		} else if(!cc.isGrounded) {
			groundSound0.SetActive(false);
			groundSound1.SetActive(false);
			mountainSound0.SetActive(false);
			mountainSound1.SetActive(false);
		}
	}

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
		if(hit.gameObject.tag == "Ground") {
			groundSound0.SetActive(true);
			groundSound1.SetActive(true);
			mountainSound0.SetActive(false);
			mountainSound1.SetActive(false);
		} else if(hit.gameObject.tag == "Mountain") {
			groundSound0.SetActive(false);
			groundSound1.SetActive(false);
			mountainSound0.SetActive(true);
			mountainSound1.SetActive(true);
		}
	}
}
