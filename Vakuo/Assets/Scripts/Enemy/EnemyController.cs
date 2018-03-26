using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    [SerializeField]
    private GameObject _top;
    [SerializeField]
    private float _jumpSpeed;

    public int lifes = 1;

    public IEnumerator waitCollision;
    
    public void OnHeadCollision(AstronautController astronaut)
    {
        Debug.Log("Damage enemy");
        astronaut.Jump(_jumpSpeed);

        StopAllCoroutines();

        lifes--;
        if(lifes <= 0)
            Destroy(gameObject);
    }

    IEnumerator WaitCheckCollision()
    {
        yield return new WaitForSeconds(0.05f);

        //Damage player
        Debug.Log("Damage player");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && waitCollision == null)
        {
            waitCollision = WaitCheckCollision();
            StartCoroutine(waitCollision);
        }
    }
}
