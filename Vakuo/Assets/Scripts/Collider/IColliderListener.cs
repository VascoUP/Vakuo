using UnityEngine;
using System.Collections;

public interface IColliderListener
{
    void OnColliderEnter(GameObject source, Collider collider);

    void OnColliderExit(GameObject source, Collider collider);
}
