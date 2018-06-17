using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthpotSpawner : MonoBehaviour {
    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    private Transform _parent;
    [SerializeField]
    private float _probability;
    public void SpawnHealth()
    {
        if(Random.Range(0f, 100f) < _probability)
        {
            GameObject instance = Instantiate(_prefab, _parent);
            instance.transform.position = transform.position + Vector3.up;
        }
    }
}
