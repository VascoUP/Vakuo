using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnExit : MonoBehaviour {
    [SerializeField]
    private List<GameObject> _checkObjects;
	[SerializeField]
	private UnityEvent _onExit;

    private void OnTriggerExit(Collider other)
    {
        if(_checkObjects.Contains(other.gameObject))
        {
            _onExit.Invoke();
        }
    }
}
