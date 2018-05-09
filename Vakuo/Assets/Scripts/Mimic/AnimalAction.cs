using UnityEngine;

public class AnimalAction : InteractionAction
{
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private MimicController _mimicController;

    public override void OnInteraction()
    {
        SubscribeMimicEvents();
        _gameManager.StartMimic(transform);
    }

    private void SubscribeMimicEvents()
    {
        _mimicController.onSuccess += MimicSuccess;
        _mimicController.onFailure += MimicFailure;
    }

    private void UnsubscribeMimicEvents()
    {
        _mimicController.onSuccess -= MimicSuccess;
        _mimicController.onFailure -= MimicFailure;
    }

    private void MimicSuccess()
    {
        UnsubscribeMimicEvents();
        Destroy(gameObject);
    }

    private void MimicFailure()
    {
        UnsubscribeMimicEvents();
    }
}
