using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour {

    public float shakeDuration = 0f;
    public float shakeAmount = 0f;
    public float decreaseFactor = 1f;

	private void FixedUpdate () {
        if (shakeDuration > 0)
        {
            transform.localPosition = Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = Vector3.zero;
        }
    }
}
