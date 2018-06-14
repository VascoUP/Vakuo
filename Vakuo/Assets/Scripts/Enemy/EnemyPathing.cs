using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour {
    [SerializeField]
    private BezierCurve _defaultPath;

    private BezierPathing _pathing;

	private void OnEnable () {
		if(_defaultPath != null)
        {
            _pathing = new BezierPathing();
        } 
	}
	
	private void Update () {
		
	}
}
