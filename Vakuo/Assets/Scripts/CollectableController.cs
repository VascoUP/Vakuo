using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour {

    [SerializeField]
    private float _rotateSpeed;

    private void OnTriggerEnter(Collider other)
    {
        CollectorController collector = other.GetComponent<CollectorController>();
        if (collector != null)
        {
            collector.Collect();
            Destroy(gameObject);
        }
    }
}
