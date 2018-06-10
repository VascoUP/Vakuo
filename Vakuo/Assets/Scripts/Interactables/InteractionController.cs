using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractionController : MonoBehaviour
{
    [SerializeField]
    private GameObject _interactObject;

    public UnityEvent onInteract;
    
    [SerializeField]
    private float _maxRasycastLength = 5f;

    private Transform _target;
    private bool _isFacingMe = false;

    [SerializeField]
    private TextMesh _text;
    private bool _isTextActive = false;

    
    public bool drawLineRendered = false;
    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;


    private void Start () {        // Draw line
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.startWidth = laserWidth;
        laserLineRenderer.endWidth = laserWidth;
    }

    private void Update()
    {
        if (_target != null &&
            IsTargetFacingMe())
        {
            _isFacingMe = true;
            IsTargetInteracting();

            if(!_isTextActive)
            {
                ActivateText("E");
            }
        }
        else if(_isFacingMe)
        {
            DeactivateText();
            _isFacingMe = false;
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
        foreach (RaycastHit hit in frontHits)
        {
            if (hit.transform.gameObject.GetInstanceID() == _interactObject.GetInstanceID())
            {
                return true;
            }
        }
        return false;
    }

    private void IsTargetInteracting()
    {
        if (Input.GetButton("Interact"))
        {
            onInteract.Invoke();
        }
    }

    private void DeactivateText()
    {
        _text.gameObject.SetActive(false);
        _isTextActive = false;
    }

    private void ActivateText(string text)
    {
        _isTextActive = true;
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
        _target = null;
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
