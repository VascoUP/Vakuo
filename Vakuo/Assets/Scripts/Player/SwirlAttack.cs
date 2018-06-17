using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwirlAttack : MonoBehaviour {
    [SerializeField]
    private EventManager _events;
    [SerializeField]
    private GameObject _meshObject;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private GameObject _attackTrigger;
    [SerializeField]
    private float _swirlTime;
    [SerializeField]
    private float _attackRange;

    private bool _isAttacking = false;

    private HashSet<int> _enemiesHit = new HashSet<int>();
    
    private void OnEnable()
    {
        _events = Utils.GetComponentOnGameObject<EventManager>("Game Events");
    }

    private IEnumerator Attack(int combo)
    {
        var time = 0f;
        Vector3 pos;
        RaycastHit[] raycastHits;
        
        _isAttacking = true;
        _animator.SetBool("attack", true);

        yield return null;

        while(time < _swirlTime)
        {
            time += Time.deltaTime;

            pos = transform.position;
            raycastHits = Physics.SphereCastAll(transform.position, _attackRange, Vector3.forward, 0.1f, 1 << LayerMask.NameToLayer("Enemy"));
            foreach (var hit in raycastHits)
            {
                if(_enemiesHit.Contains(hit.transform.gameObject.GetInstanceID()))
                {
                    break;
                }

                Vector3 vec = hit.transform.position;
                Vector3 direction = vec - _meshObject.transform.position;

                if (Vector3.Dot(direction, _meshObject.transform.right) > 0.7 ||
                    Vector3.Dot(direction, -_meshObject.transform.right) > 0.7)
                {
                    _events.onAttack();
                    EnemyPathing enemy = hit.transform.gameObject.GetComponent<EnemyPathing>();
                    if(enemy != null)
                    {
                        _enemiesHit.Add(hit.transform.gameObject.GetInstanceID());
                        enemy.DamagedByPlayer();
                    }
                } 
            }

            yield return null;
        }

        _enemiesHit.Clear();
        _animator.SetBool("attack", false);
        _isAttacking = false;
    }

	// Update is called once per frame
	private void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Mouse0) && !_isAttacking)
        {
            _attackTrigger.SetActive(true);
            _attackTrigger.SetActive(false);
            StartCoroutine(Attack(0));
        }
	}
}
