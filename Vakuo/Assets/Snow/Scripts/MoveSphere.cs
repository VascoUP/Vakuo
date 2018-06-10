using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour {

    public float height;
    public float width;
    private float time = 0;
    private Vector3 position;

    private void Start()
    {
        position = transform.position;
    }

	void Update () {
        time += Time.deltaTime;
        float sinTime = Mathf.Sin(time);
        transform.position = new Vector3(sinTime * width + position.x, Mathf.Abs(sinTime) * height + position.y, position.z);
	}
}
