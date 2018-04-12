using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour {

    [SerializeField]
    private float _jumpeForce;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            AstronautController controller = collision.gameObject.GetComponent<AstronautController>();
            if(controller != null)
            {
                controller.Jump(_jumpeForce);
            }
        }
    }
}
