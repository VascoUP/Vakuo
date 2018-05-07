using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEnabler : MonoBehaviour {
    [SerializeField]
    private EventManager _eventManager;

    private BoxCollider _bc;

    public bool isPlayerInside = false;

    private void Start()
    {
        _bc = GetComponent<BoxCollider>();
    }

    public void Disable()
    {
        _bc.enabled = false;
        RestrictAccess();
    }

    public void Enable()
    {
        _bc.enabled = true;
    }

    private void RestrictAccess()
    {
        isPlayerInside = false;
        _eventManager.onExitInventoryEnabler();
    }

    private void GiveAccess()
    {
        isPlayerInside = true;
        _eventManager.onEnterInventoryEnabler();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isPlayerInside)
        {
            GiveAccess();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            RestrictAccess();
        }
    }
}
