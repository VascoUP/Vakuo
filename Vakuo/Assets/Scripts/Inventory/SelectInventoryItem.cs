using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectInventoryItem : MonoBehaviour {
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
    private GameObject _spawnedAnimal;
    private bool _animalSpawned = false;

    private void Start()
    {
        _eventManager.onEnterInventoryEnabler += EnteredInventoryEnabler;
        _eventManager.onExitInventoryEnabler += EnteredInventoryEnabler;

        _keySM = new KeyStateMachine("Inventory");
    }

	private void EnableSelection(bool enable) {
		if (enable) {
			// Start checking for input
			onUpdate += CheckInput;
		} else {
			// Start checking for input
			onUpdate -= CheckInput;
		}
	}
		

    private void EnteredInventoryEnabler()
    {
        if (_iEnabler.isPlayerInside) 
        {
			// Enable inventory selection
			EnableSelection (true);
        }
    }

    private void AnimalSelected(string animal)
    {
        if(animal == _animalName)
		{
            // Spawn player on the spot
            _spawnedAnimal = Instantiate(_prefab, transform);
            // Disable inventory enabler for now
            _iEnabler.Disable();
            // Enable animal spawned
            _animalSpawned = true;
			// Disable inventory selection
			ExitedInventoryEnabler ();
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
		// Disable inventory selection
		EnableSelection (false);
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

    public void Remove()
    {
        if(_animalSpawned)
		{
			// Animal is no longer spawned
			_animalSpawned = false;
			// Destroy object
			Destroy(_spawnedAnimal);
			// Disable inventory enabler for now
			_iEnabler.Enable();
        }
    }
}
