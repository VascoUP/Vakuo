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
    private bool isQuit = false;

    private void OnApplicationQuit()
    {
        isQuit = true;
    }

    private void OnDestroy()
    {
        if (isQuit)
            return;

        if(Random.Range(0f, 100f) < _probability)
        {
            GameObject instance = Instantiate(_prefab, _parent);
            instance.transform.position = transform.position + Vector3.up;
        }
    }
}
