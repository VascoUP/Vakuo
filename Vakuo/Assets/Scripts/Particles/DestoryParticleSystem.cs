using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryParticleSystem : MonoBehaviour
{
    private void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }
}
