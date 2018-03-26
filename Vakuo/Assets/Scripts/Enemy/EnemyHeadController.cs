using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadController : MonoBehaviour {
    private EnemyController _enemyController;

    private void Start()
    {
        _enemyController = transform.parent.GetComponent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            AstronautController ac = other.GetComponent<AstronautController>();
            _enemyController.OnHeadCollision(ac);
        }
    }
}
