using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWater : MonoBehaviour {
    private bool running = false;
    private IEnumerator KillPlayerOnWater(GameObject player)
    {
        running = true;

        AstronautController controller = player.GetComponent<AstronautController>();
        PlayerLife pl = player.GetComponent<PlayerLife>();
        controller._animator.SetBool("damaged", true);

        yield return new WaitForSeconds(2f);

        controller._animator.SetBool("damaged", false);
        controller.Respawn();
        pl.Kill();

        running = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player" && !running)
        {
            AstronautController controller = collider.gameObject.GetComponent<AstronautController>();
            if(controller != null)
                StartCoroutine(KillPlayerOnWater(collider.gameObject));
        }
    }
}
