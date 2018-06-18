using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {
    public int outroSceneIndex;

	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Show mousse
            Cursor.visible = true;
            SceneManager.LoadScene(outroSceneIndex);
        }
    }
}
