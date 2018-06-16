using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthpotController : MonoBehaviour {
    [SerializeField]
    private GameObject _particlesPrefab;
    [SerializeField]
    private float _speed;
    private bool destroy = false;
	
	// Update is called once per frame
	private void FixedUpdate () {
        if(destroy)
        {
            GameObject instance = Instantiate(_particlesPrefab, transform.parent);
            instance.transform.position = transform.position;
            Destroy(gameObject);
        }
        transform.RotateAround(transform.position, Vector3.up, _speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.tag == "Player")
        {
            PlayerLife astronautLifes = collider.gameObject.GetComponent<PlayerLife>();
            astronautLifes.AddLife();
            destroy = true;
        }
    }
}
