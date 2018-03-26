using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour {

    [SerializeField]
    private float _jumpeForce;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Is player");
            AstronautController controller = collision.gameObject.GetComponent<AstronautController>();
            if(controller != null)
            {
                controller.Jump(_jumpeForce);
            }
        }
    }
}
