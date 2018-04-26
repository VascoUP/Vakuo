using UnityEngine;

public class AnimalAction : InteractionAction
{
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = Utils.GetComponentOnGameObject<GameManager>("Game Manager");
    }

    public override void OnInteraction()
    {
        _gameManager.StartMimic(transform);
    }
}
