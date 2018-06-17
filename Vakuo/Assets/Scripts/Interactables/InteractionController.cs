using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractionController : MonoBehaviour
{
    [SerializeField]
    private GameObject _interactObject;

    public UnityEvent onInteract;

    [SerializeField]
    private Vector3 _raycastOffset;
    [SerializeField]
    private float _maxRasycastLength = 5f;

    private Transform _target;
    private bool _isFacingMe = false;

    [SerializeField]
    private TextMesh _text;
    private bool _isTextActive = false;

    private void Update()
    {
        if (_target != null &&
            IsTargetFacingMe())
        {
            if(!_isTextActive)
            {
                ActivateText("E");
            }

            _isFacingMe = true;
            IsTargetInteracting();
        }
        else if(_isFacingMe)
        {
            DeactivateText();
            _isFacingMe = false;
        }
    }

    private bool IsTargetFacingMe()
    {
        Ray ray = new Ray(_target.position + _raycastOffset, _target.TransformDirection(Vector3.forward));
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
            Debug.Log(_target);
            _target = other.transform;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        _target = null;
    }
}
