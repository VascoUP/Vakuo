using System.Collections;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private KeyStateMachine _keyState;

    [SerializeField]
    private float _channelingTime;
    [SerializeField]
    private InteractionAction _action;

    [SerializeField]
    private float _maxRasycastLength = 5f;

    private IEnumerator _channelingCoroutine;
    private bool _channelingRunning = false;

    private Transform _target;

    [SerializeField]
    private TextMesh _text;
    private bool _isActive = false;

    private bool _isFacingMe = false;

    public bool drawLineRendered = false;
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;

    private void Start () {
        _keyState = new KeyStateMachine("Interact");

        // Draw line
        Debug.Log(laserLineRenderer);
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
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

        // Draw line
        if (_target != null && drawLineRendered)
        {
            ShootLaserFromTargetPosition(_target.position, _target.TransformDirection(Vector3.forward), _maxRasycastLength);
            laserLineRenderer.enabled = true;
        }
        else
        {
            laserLineRenderer.enabled = false;
        }
    }

    private bool IsTargetFacingMe()
    {
        Ray ray = new Ray(_target.position, _target.TransformDirection(Vector3.forward));
        RaycastHit[] frontHits = Physics.RaycastAll(ray, _maxRasycastLength);
        Debug.Log(frontHits.Length);
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

    #region DrawLine

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Vector3 endPosition = targetPosition + (length * direction);
        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }
    
    #endregion
}
