using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI;

public class EntityInventory : MonoBehaviour
{
    public BasicContainer mainContainer = new BasicContainer();
    void Start()
    {
        mainContainer.OnInventoryChange.AddListener(delegate { Debug.Log("Inventory changed!"); });
    }

    // Update is called once per frame
    void Update()
    {

    }

}