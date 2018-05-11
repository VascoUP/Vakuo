using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeshController : MonoBehaviour {
    [SerializeField]
    private GameObject _body;

    [SerializeField]
    private float _damping;

    [SerializeField]
    private float _maxInputError = 0.01f;

    private float _localRotAngle = 0.0f;
    private float _rotAngle = 0.0f;
    private Vector3 _playerRotAxis;
    private float _playerRotAngle = 0.0f;

    private Quaternion RotToQuaternion()
    {
        _rotAngle = _localRotAngle + _playerRotAngle * _playerRotAxis.y;
        return Quaternion.AngleAxis(_rotAngle, Vector3.up);
    }

    private void UpdateValues()
    {
        _localRotAngle = Mathf.Atan2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * Mathf.Rad2Deg;
        transform.rotation.ToAngleAxis(out _playerRotAngle, out _playerRotAxis);
    }
    
	private void LateUpdate ()
    {
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) >= _maxInputError || Mathf.Abs(Input.GetAxisRaw("Vertical")) >= _maxInputError)
        {
            UpdateValues();
        }

        _body.transform.rotation = Quaternion.Lerp(_body.transform.rotation, RotToQuaternion(), Time.deltaTime * _damping);
    }
}
