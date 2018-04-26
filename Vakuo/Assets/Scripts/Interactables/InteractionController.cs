using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private KeyStateMachine _keyState;

    [SerializeField]
    private float _channelingTime;
    [SerializeField]
    private InteractionAction _action;

    private IEnumerator _channelingCoroutine;
    private bool _channelingRunning = false;

    private Transform _target;

    [SerializeField]
    private TextMesh _text;
    private bool _isActive = false;

    private bool _isFacingMe = false;

    private void Start () {
        _keyState = new KeyStateMachine("Interact");
	}
    
    private void Update () {
        if (_target != null && IsTargetFacingMe())
        {
            _isFacingMe = true;
            IsTargetInteracting();
        }
        else if(_isFacingMe)
        {
            _isFacingMe = false;
            StopChanneling();
        }
    }

    private bool IsTargetFacingMe()
    {
        Ray ray = new Ray(_target.position, _target.TransformDirection(Vector3.forward));
        RaycastHit[] frontHits = Physics.RaycastAll(ray, 6f);
        foreach (RaycastHit hit in frontHits)
        {
            if(hit.transform.gameObject.GetInstanceID() == gameObject.GetInstanceID() || 
                (hit.transform.parent != null && 
                hit.transform.parent.gameObject.GetInstanceID() == gameObject.GetInstanceID()))
            {
                return true;
            }
        }
        return false;
    }

    private void TargetStoppedInteracting()
    {
        // Reset state machine if not on top
        _keyState.status = KeyStateMachine.InputStatus.IGNORE;
        // Stop channeling coroutine
        StopChanneling();
    }

    private void IsTargetInteracting()
    {
        _keyState.Update();

        if(_keyState.status == KeyStateMachine.InputStatus.PRESSING && !_channelingRunning)
        {
            _channelingCoroutine = WaitChanneling(_channelingTime);
            StartCoroutine(_channelingCoroutine);
            return;
        }

        if (_keyState.status != KeyStateMachine.InputStatus.PRESSING)
        {
            // Activate text
            if(!_isActive)
            {
                ActivateText("E");
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
        _action.OnInteraction();
    }

    private IEnumerator WaitChanneling(float channelingTime)
    {
        _channelingRunning = true;
        _text.text = "Pressing";
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
        _text.text = text;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && 
            (_target == null || 
            _target.GetInstanceID() != other.transform.GetInstanceID()))
        {
            _target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.tag == "Player")
        {
            StopChanneling();
            TargetStoppedInteracting();
            _target = null;
        }
    }
}
