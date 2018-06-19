using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Camera camera = GetComponent<Camera>();
        float[] distances = new float[32];
        distances[13] = 100;
        camera.layerCullDistances = distances;
	}
}
