using UnityEngine;

public abstract class ItemsEnabler : MonoBehaviour {

    public abstract void EnableItems(Transform target, bool isEnable);

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            EnableItems(collider.transform, true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            EnableItems(collider.transform, false);
        }
    }
}
