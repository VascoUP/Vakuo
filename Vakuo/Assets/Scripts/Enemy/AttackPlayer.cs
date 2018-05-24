using UnityEngine;
public class AttackPlayer : MonoBehaviour
{
    public Transform Player;
    public float MoveSpeed = 4;
    public float JumpSpeed = 3;
    public float AttackRange = 0.1f;
    public float WaitTime = 0.5f;

    private CharacterController characterController;

    private enum States
    {
        MOVING,
        WAITING,
        ATTACKING,
    }

    private States state = States.MOVING;
    private float counter;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 targetPosition = new Vector3(Player.position.x,
            characterController.transform.position.y,
            Player.position.z);
        characterController.transform.LookAt(targetPosition);

        switch (state)
        {
            case States.MOVING:
                if (Vector3.Distance(characterController.transform.position, Player.position) <= 0)
                {
                    counter = 0;
                    state = States.WAITING;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                }
                break;
            case States.WAITING:
                if (counter >= WaitTime)
                {
                    counter = 0;
                    state = States.ATTACKING;
                }
                else
                {
                    counter += Time.deltaTime;
                }
                break;
            case States.ATTACKING:
                state = States.MOVING;
                break;
        }
    }
}