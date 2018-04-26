using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AttackPlayer : MonoBehaviour
{
    public Transform Player;
    public int MoveSpeed = 4;
    public int MaxDist = 10;
    public int MinDist = 5;
    public float waitTime = 0.5f;

    void Start()
    {

    }

    void Update()
    {
        transform.LookAt(Player);

        if (Vector3.Distance(transform.position, Player.position) >= MinDist)
        {
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, Player.position) <= MaxDist)
            {
                //TODO write attack thingy here

                //waitToAttack() mas vou ter de fazer uma maquina de estados ANDAR ESPERAR ATACAR

            }
        }
    }

    public IEnumerator WaitToAttack()
    {
        yield return new WaitForSeconds(waitTime);
    }
}