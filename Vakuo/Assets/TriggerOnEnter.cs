using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnEnter : MonoBehaviour {
    [SerializeField]
    private List<GameObject> _checkObjects;
	[SerializeField]
	private UnityEvent _onEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (_checkObjects.Contains(other.gameObject))
        {
            _onEnter.Invoke();
        }
    }
}
