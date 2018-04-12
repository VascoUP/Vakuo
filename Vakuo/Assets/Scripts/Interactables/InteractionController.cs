using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private KeyStateMachine _keyState;

    [SerializeField]
    private Transform _feet;
    [SerializeField]
    private float _feetToGround;
    [SerializeField]
    private float _frontDistance;

    private Transform _interactableObject;
    private bool _isOnTop = true;

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
            OnArea();
        }
        else if(_isOnTop)
        {
            _isOnTop = false;
            OnExitArea();
        }
    }

    private bool CheckInteractable()
    {
        RaycastHit feetHit;
        bool feetHasHit = Physics.Raycast(_feet.position, Vector3.down, out feetHit, _feetToGround);
        RaycastHit frontHit;
        bool frontHasHit = Physics.Raycast(transform.position, transform.forward, out frontHit, _frontDistance);
        
        if (feetHasHit)
        {
            if(feetHit.transform.gameObject.tag == "Interactable")
            {
                if(_interactableObject != null && 
                    feetHit.transform.GetInstanceID() != _interactableObject.GetInstanceID() && 
                    ((frontHasHit && frontHit.transform.GetInstanceID() != _interactableObject.GetInstanceID()) || !frontHasHit))
                {
                    StopChanneling();
                }
                _interactableObject = feetHit.transform;
                return true;
            }
        }

        if (frontHasHit)
        {
            if (frontHit.transform.gameObject.tag == "Interactable" )
            {
                if (_interactableObject != null && frontHit.transform.GetInstanceID() != _interactableObject.GetInstanceID())
                {
                    StopChanneling();
                }
                _interactableObject = frontHit.transform;
                return true;
            }
        }

        return false;
    }

    private void OnExitArea()
    {
        // Reset state machine if not on top
        _keyState.status = KeyStateMachine.InputStatus.IGNORE;
        // Stop channeling coroutine
        StopChanneling();
    }

    private void OnArea()
    {
        _keyState.Update();

        if(_keyState.status == KeyStateMachine.InputStatus.PRESSING && !_channelingRunning)
        {
            InteractionAction action = _interactableObject.gameObject.GetComponent<InteractionAction>();
            if(action == null)
            {
                _interactableObject = null;
                return;
            }

            _channelingCoroutine = WaitChanneling(action.channelingTime);
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

    private void OnEndChanneling()
    {
        DeactivateText();
        InteractionAction action = _interactableObject.gameObject.GetComponent<InteractionAction>();
        if(action != null)
        {
            action.OnInteraction();
        }
        else
        {
            Debug.Log("Action is null");
        }
    }

    private IEnumerator WaitChanneling(float channelingTime)
    {
        _channelingRunning = true;
        _text.text = "Channeling";
        yield return new WaitForSeconds(channelingTime);
        _channelingRunning = false;
        OnEndChanneling();
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
