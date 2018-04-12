using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private KeyStateMachine _keyState;

    [SerializeField]
    private Transform _feet;
    [SerializeField]
    private float _feetToGround;

    private Transform _interactableObject;
    private bool _isOnTop = true;

    [SerializeField]
    private float _channelingTime;
    private IEnumerator _channelingCoroutine;
    private bool _channelingRunning = false;

    [SerializeField]
    private TextMesh _text;
    private bool _isActive = false;

    private void Start () {
        _keyState = new KeyStateMachine("Interact");
	}

    private void FixedUpdate () {
        if (CheckInteractable())
        {
            _isOnTop = true;
            OnTop();
        }
        else if(_isOnTop)
        {
            _isOnTop = false;
            OnExitTop();
        }
    }

    private bool CheckInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(_feet.position, Vector3.down, out hit, _feetToGround))
        {
            if(hit.transform.gameObject.tag == "Interactable")
            {
                if(_interactableObject != null && hit.transform.GetInstanceID() != _interactableObject.GetInstanceID())
                {
                    StopChanneling();
                    _interactableObject = hit.transform;
                }
                return true;
            }
        }

        return false;
    }

    private void OnExitTop()
    {
        // Reset state machine if not on top
        _keyState.status = KeyStateMachine.InputStatus.IGNORE;
        // Stop channeling coroutine
        StopChanneling();
    }

    private void OnTop()
    {
        _keyState.Update();

        if(_keyState.status == KeyStateMachine.InputStatus.PRESSING && !_channelingRunning)
        {
            _channelingCoroutine = WaitChanneling();
            StartCoroutine(_channelingCoroutine);
            return;
        }

        if (_keyState.status != KeyStateMachine.InputStatus.PRESSING)
        {
            // Activate text
            if(!_isActive)
            {
                ActivateText("Interact");
            }

            // Stop coroutine
            if(_channelingRunning)
            {
                StopChanneling();
            }
        }
    }

    private IEnumerator WaitChanneling()
    {
        _channelingRunning = true;
        _text.text = "Channeling";
        yield return new WaitForSeconds(_channelingTime);
        DeactivateText();
        _channelingRunning = false;
    }

    private void StopChanneling()
    {
        if (_channelingRunning)
        {
            // Stop channeling coroutine
            StopCoroutine(_channelingCoroutine);
            _channelingRunning = false;
        }

        // Hide text
        DeactivateText();
    }

    private void DeactivateText()
    {
        _text.gameObject.SetActive(false);
        _isActive = false;
    }

    private void ActivateText(string text)
    {
        _isActive = true;
        _text.gameObject.SetActive(true);
        _text.text = "Interact";
    }
}
