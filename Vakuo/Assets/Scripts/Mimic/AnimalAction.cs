using Assets.Scripts.UI;
using UnityEngine;

public class AnimalAction : InteractionAction
{
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private MimicController _mimicController;
    [SerializeField]
    private string _animalName;
    [SerializeField]
    private string _animalIconName;

    public override void OnInteraction()
    {
        _gameManager.StartMimic(this);
    }

    public BasicItem GetAnimalBasicItem()
    {
        return new BasicItem(_animalName ,_animalIconName);
    }

    public void MimicSuccess()
    {
        Destroy(gameObject);
    }

    public void MimicFailure()
    {
    }
}
