using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySelectionController : MonoBehaviour {
    private UpdateMonoBehavior onUpdate;

    [SerializeField]
    private EventManager _eventManager;
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private InventoryController _inventoryController;
    [SerializeField]
    private InventoryEnabler _iEnabler;

    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    private string _animalName;

    private KeyStateMachine _keySM;

    private void Start()
    {
        _eventManager.onEnterInventoryEnabler += EnteredInventoryEnabler;
        _eventManager.onExitInventoryEnabler += EnteredInventoryEnabler;

        _keySM = new KeyStateMachine("Inventory");
    }

    private void EnteredInventoryEnabler()
    {
        if (_iEnabler.isPlayerInside) 
        {
            // Start checking for input
            onUpdate += CheckInput;
        }
    }

    private void AnimalSelected(string animal)
    {
        if(animal == _animalName)
        {
            // Spawn player on the spot
            Instantiate(_prefab, transform);
            // Disable inventory enabler for now
            _iEnabler.Disable();
        } else
        {
            // TODO Damage player
        }

        // Unsubscribe to receive double click event
        _inventoryController.onItemSelected -= AnimalSelected;

        _gameManager.ChangeState(GameStatus.RUNNING);
    }

    private void ExitedInventoryEnabler()
    {
        // Stop check for input
        onUpdate -= CheckInput;
    }

    private void CheckInput()
    {
        _keySM.Update();
        if(_keySM.status == KeyStateMachine.InputStatus.JUST_PRESSED)
        {
            // Subscribe to receive double click event
            _inventoryController.onItemSelected += AnimalSelected;

            // Initialize Inventory Selection
            _gameManager.ChangeState(GameStatus.INVENTORY_SELECTION);

            // Reset state machine
            _keySM.status = KeyStateMachine.InputStatus.IGNORE;
        }
    }

    private void Update () {
		if(onUpdate != null)
        {
            onUpdate();
        }
	}
}
