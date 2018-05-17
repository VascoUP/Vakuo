using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RemoveInventoryItem : MonoBehaviour {

    [SerializeField]
	private SelectInventoryItem _isc;
	public UnityEvent onExit;

	// When player goes through trigger exit
	// Invoke onExit event
	private void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			onExit.Invoke ();
		}
	}
}
