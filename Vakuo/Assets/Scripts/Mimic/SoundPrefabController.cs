using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPrefabController : MonoBehaviour {

    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _time;

    private void Start()
    {
        StartCoroutine(WaitForEndOfTime());
    }

    private void Update ()
    {
        transform.Translate(transform.worldToLocalMatrix * (Vector3.up * _speed * Time.deltaTime));
	}

    private IEnumerator WaitForEndOfTime()
    {
        yield return new WaitForSeconds(_time);
        
        Destroy(gameObject);
    }
}
