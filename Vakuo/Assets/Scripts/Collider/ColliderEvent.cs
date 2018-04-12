using UnityEngine;
using System.Collections;

public class ColliderEvent : MonoBehaviour
{
    private IColliderListener listener;

    public void Initialize(IColliderListener listener)
    {
        this.listener = listener;
    }

    private void OnTriggerEnter(Collider collider)
    {
        listener.OnColliderEnter(gameObject, collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        listener.OnColliderExit(gameObject, collider);
    }
}
