using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;

public class AnimalAction : InteractionAction
{
    public UnityEvent onSuccess;
    public UnityEvent onFailure;

    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private MimicController _mimicController;
    [SerializeField]
    private Vector3 _soundOffset = new Vector3(2f,1f,0f);
    [SerializeField]
    private string _animalName;
    [SerializeField]
    private string _animalIconName;

    public override void OnInteraction()
    {
        _mimicController._spawnOffset = _soundOffset;
        _gameManager.StartMimic(this);
    }

    public BasicItem GetAnimalBasicItem()
    {
        return new BasicItem(_animalName ,_animalIconName);
    }

    public void MimicSuccess()
    {
        onSuccess.Invoke();
    }

    public void MimicFailure()
    {
        onFailure.Invoke();
    }
}
