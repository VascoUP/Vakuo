using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRigidbody : MonoBehaviour {

    public Rigidbody _rb;
    public float speed;
    public float jump;

	void Update () {
        _rb.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed * Time.deltaTime);
        _rb.AddForce(Vector3.up * (Input.GetButton("Jump") ? 1f : 0f) * jump * Time.deltaTime, ForceMode.Acceleration);
    }
}
