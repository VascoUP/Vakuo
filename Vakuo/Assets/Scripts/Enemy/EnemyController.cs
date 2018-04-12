using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IColliderListener {
    [SerializeField]
    private float _ySpeed;
    [SerializeField]
    private float _pushSpeed;
    [SerializeField]
    private int _lifes = 1;

    private bool _onCooldown = false;
    
    void Start()
    {
        // Set collider events for Head and Body children
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger && (collider.gameObject.name == "Head" || collider.gameObject.name == "Body"))
            {
                ColliderEvent cl = collider.gameObject.AddComponent<ColliderEvent>();
                cl.Initialize(this);
            }
        }
    }

    private IEnumerator WaitCooldown()
    {
        yield return new WaitForSeconds(1f);
        _onCooldown = false;
    }

    private void OnHeadCollision(AstronautController astronaut)
    {
        astronaut.Jump(_ySpeed);
        // Damage enemy
         _lifes--;
        if(_lifes <= 0)
        {
            Destroy(gameObject);
            StopAllCoroutines();
        }
    }

    private void OnBodyCollision(AstronautController astronaut)
    {
        Vector3 direction = astronaut.transform.position - transform.position;
        direction.y = 0;
        Vector3 nDirection = direction.normalized;
        nDirection.x *= Mathf.Sign(direction.x);
        nDirection.z *= Mathf.Sign(direction.z);
        
        astronaut.Push(_pushSpeed, _ySpeed, new Vector3(direction.x, 1, direction.z));

        // Damage astronaut
    }

    public void OnColliderEnter(GameObject source, Collider collider)
    {
        if (_onCooldown)
            return;

        if (collider.gameObject.tag == "Player")
        {
            _onCooldown = true;
            StartCoroutine(WaitCooldown());

            AstronautController astronaut = collider.gameObject.GetComponent<AstronautController>();
            if (astronaut == null)
                return;

            if(source.name == "Head")
            {
                OnHeadCollision(astronaut);
            }
            else if(source.name == "Body")
            {
                OnBodyCollision(astronaut);
            }
        }
    }

    public void OnColliderExit(GameObject source, Collider collider)
    { }
}
