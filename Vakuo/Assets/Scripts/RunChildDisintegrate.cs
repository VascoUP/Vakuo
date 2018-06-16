using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunChildDisintegrate : MonoBehaviour {
    
	private void OnEnable()
    {
        RunDisintegrate[] scripts = GetComponentsInChildren<RunDisintegrate>();
        foreach(RunDisintegrate rd in scripts)
        {
            rd.Enable(true);
        }
    }
}
