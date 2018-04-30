using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AttackPlayer : MonoBehaviour
{
    public Transform Player;
    public int MoveSpeed = 4;
    public float jumpSpeed = 3;
    public int AttackRange = 3;
    public float waitTime = 1.5f;


    private enum States
    {
        TURNING,
        MOVING,
        WAITING,
        ATTACKING,
    }

    private States state = States.TURNING;
    private float counter;

    void Start()
    {

    }

    void Update()
    {
        switch (state)
        {
            case States.TURNING:
                transform.LookAt(Player);
                state = States.MOVING;
                break;
            case States.MOVING:
                if (Vector3.Distance(transform.position, Player.position) <= AttackRange)
                {
                    counter = 0;
                    state = States.WAITING;
                }
                else
                {
                    transform.position += transform.forward * MoveSpeed * Time.deltaTime;
                }
                break;
            case States.WAITING:
                if (counter >= waitTime)
                {
                    state = States.ATTACKING;
                }
                else
                {
                    counter += Time.deltaTime;
                }
                break;
            case States.ATTACKING:
                //TODO write attack thingy here, for now it's jumping but in the wrong way ;)
                transform.position += transform.up * jumpSpeed * Time.deltaTime;
                state = States.TURNING;
                break;
        }
    }

    public IEnumerator WaitToAttack()
    {
        yield return new WaitForSeconds(waitTime);
    }
}