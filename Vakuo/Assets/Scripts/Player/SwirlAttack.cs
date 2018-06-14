using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwirlAttack : MonoBehaviour {

    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private float _swirlSpeed;
    [SerializeField]
    private float _swirlTime;
    [SerializeField]
    private float _attackRange;

    private bool _isAttacking = false;

    private IEnumerator Attack(int combo)
    {
        var time = 0f;
        Vector3 pos;
        RaycastHit[] raycastHits;

        Debug.Log("Attacking");

        _isAttacking = true;

        yield return null;

        while(time < _swirlTime)
        {
            time += Time.deltaTime;

            pos = transform.position;
            raycastHits = Physics.SphereCastAll(transform.position, _attackRange, Vector3.forward, 0.1f, 1 << LayerMask.NameToLayer("Enemy"));
            foreach (var hit in raycastHits)
            {
                Vector3 vec = hit.transform.position;
                Vector3 direction = vec - _target.transform.position;
                
                if (Vector3.Dot(direction, _target.transform.forward) > 0.7)
                {
                    Debug.Log("Hit enemy: " + hit.transform.gameObject.name);
                }
            }

            yield return null;
        }

        Debug.Log("End Attacking");

        _isAttacking = false;
    }

	// Update is called once per frame
	private void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Mouse0) && !_isAttacking)
        {
            StartCoroutine(Attack(0));
        }
	}
}
