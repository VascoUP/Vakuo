using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    private GameManager _gameManager;

    public Button resumeButton;
    public Button exitButton;

    void Start()
    {
        _gameManager = Utils.GetComponentOnGameObject<GameManager>("Game Manager");
        resumeButton.onClick.AddListener(() =>
        {
            _gameManager.ChangeState(GameStatus.RUNNING);
        });
        exitButton.onClick.AddListener(() =>
        {
            _gameManager.Quit();
        });
    }
}
