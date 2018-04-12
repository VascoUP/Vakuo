using UnityEngine;

public abstract class ItemsEnabler : MonoBehaviour {
        
    public abstract void EnableItems(bool isEnable);

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            EnableItems(true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            EnableItems(false);
        }
    }
}
