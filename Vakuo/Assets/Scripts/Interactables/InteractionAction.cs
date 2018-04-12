using UnityEngine;

public abstract class InteractionAction : MonoBehaviour
{
    public float channelingTime;

    public abstract void OnInteraction();
}
