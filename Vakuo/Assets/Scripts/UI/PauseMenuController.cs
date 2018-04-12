using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour {

    public GameObject inventory;
    public Button resumeButton;
    public Button inventoryButton;
    public Button exitButton;

    void Start ()
    {
        Time.timeScale = 0f;
        resumeButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        });
        inventoryButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            inventory.SetActive(true);
        });
        exitButton.onClick.AddListener(() =>
        {
        });

    }
	
	void Update () {
		
	}
}
