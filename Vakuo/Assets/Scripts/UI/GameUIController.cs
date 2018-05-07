using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour {
    [SerializeField]
    private EventManager _eventManager;

    [SerializeField]
    private GameObject _inventoryNotifier;

    private void Start()
    {
        _eventManager.onEnterInventoryEnabler += EnableInventoryNotifier;
        _eventManager.onExitInventoryEnabler += DisableInventoryNotifier;
    }

    public void EnableInventoryNotifier()
    {
        _inventoryNotifier.SetActive(true);
    }

    public void DisableInventoryNotifier()
    {
        _inventoryNotifier.SetActive(false);
    }
}
