using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour {
    private enum EnemyStates
    {
        IDDLE, PLAYER_ENTERED, DECIDE_TO_MOVE, MOVE_TO_PLAYER, WAIT_CHARGE, CHARGE, DAMAGED, FLEE
    }
    private EnemyStates _state;
    private bool runningState = false;
    private IEnumerator runningCoroutine;

    [SerializeField]
    private CharacterController _characterController;
    
    [SerializeField]
    private BezierPathing _pathing;
    private bool _walkedPath;

    [SerializeField]
    private Transform _target;

    // Instance of event manager
    public EventManager _events;

    // Acceleration downward of the astronaut while in the air
    public float gravity = 15.0f;
    private float yVelocity = 0f;

    public float walkSpeed;
    public float jumpSpeed;
    public float chargeSpeed;
    public float prepareChargeDuration;
    public float chargeDuration;
    public float damagedDuration;
    public float distToAttack;
    public float distanceToCharge;
    public float maxDistanceAwayFromPath;

    public float _pushYSpeed;
    public float _pushHorizontalSpeed;

    public int _lifes = 1;

    private bool _onCooldown = false;

    public bool isDebug = false;

    private void OnEnable()
    {
        StopAllCoroutines();
        runningCoroutine = null;
        ChangeState(EnemyStates.IDDLE);
    }

    public void DamagedByPlayer()
    {
        StopAllCoroutines();
        ChangeState(EnemyStates.DAMAGED);
    }

    public void OutOfBounds()
    {
        Destroy(gameObject);
    }

    private void ChangeState(EnemyStates state)
    {
        _state = state;
        runningState = false;
    }

    private IEnumerator PlayerEntered(float jumpSpeed)
    {
        if(!_characterController.isGrounded)
        {
            yield return new WaitWhile(() => {
                return !_characterController.isGrounded;
            });
        }

        yVelocity = jumpSpeed;
        yield return null;

        yield return new WaitWhile(() => {
            return !_characterController.isGrounded;
        });

        ChangeState(EnemyStates.DECIDE_TO_MOVE);
    }

    private IEnumerator WalkToTarget(Transform target, float moveSpeed, float distance)
    {
        Vector3 direction = target.transform.position - transform.position;

        while (direction.magnitude > distance)
        {
            direction.Normalize();
            _characterController.Move(direction * moveSpeed * Time.deltaTime);

            yield return null;

            direction = target.transform.position - transform.position;
        }

        ChangeState(EnemyStates.WAIT_CHARGE);
    }

    private IEnumerator PrepareChargeAttack(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        ChangeState(EnemyStates.CHARGE);
    }

    private IEnumerator ChargeAttack(Vector3 direction, float moveSpeed, float duration)
    {
        float time = 0f;
        direction = transform.TransformDirection(direction);
        direction.Normalize();

        while (time < duration)
        {
            float deltaTime = Time.deltaTime;
            time += deltaTime;
            _characterController.Move(direction * moveSpeed * deltaTime);

            yield return null;
        }

        ChangeState(EnemyStates.FLEE);
    }

    private IEnumerator RunFromPlayer(Vector3 point, float moveSpeed)
    {
        /*
        Vector3 direction = point - transform.position;
        direction.Normalize();

        while (Mathf.Approximately(transform.position.x, point.x) &&
                Mathf.Approximately(transform.position.y, point.y) &&
                Mathf.Approximately(transform.position.z, point.z))
        {
            //TODO: Move might lead to a point where the enemy runs forever
            _characterController.Move(direction * moveSpeed * Time.deltaTime);
        }
        */
        yield return null;

        ChangeState(EnemyStates.DECIDE_TO_MOVE);
    }

    private IEnumerator Damaged(float jumpSpeed, float duration)
    {
        yVelocity = jumpSpeed;
        yield return new WaitForSeconds(duration);
        ChangeState(EnemyStates.DECIDE_TO_MOVE);
    }

    private bool WillAttackPlayer()
    {
        //TODO: Communicate between all enemies to decide which one attacks
        return true;
    }

    private void StartState()
    {
        if (runningCoroutine != null)
        {
            runningState = true;
            StartCoroutine(runningCoroutine);
        }
    }

    private void UpdateYVelocity()
    {
        if (_characterController.isGrounded && yVelocity != 0f)
        {
            yVelocity = 0f;
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;
        }
        _characterController.Move(Vector3.up * yVelocity);
    }

    private float DistanceToPath()
    {
        Vector3 point = transform.InverseTransformPoint(_pathing.BezierCurve.GetPointAt(0));
        return (transform.position - point).magnitude;
    }

    private void WalkPath()
    {
        if(_pathing != null)
        {
            _walkedPath = _pathing.WalkPath();
        }
    }

    private void Update()
    {
        if(isDebug)
        {
            Debug.Log(_state);
            Debug.Log(runningCoroutine != null);
            Debug.Log(runningCoroutine != null ? runningCoroutine.ToString() : "null");
        }
        
        if(_target != null && _state == EnemyStates.IDDLE && _walkedPath)
        {
            Vector3 point = transform.InverseTransformPoint(_pathing.BezierCurve.GetPointAt(0));
            float distToP = (_target.transform.position - point).magnitude;
            Debug.Log("dist to p:" + distToP);
            if ((_target.transform.position - point).magnitude > distToAttack)
            {
                _state = EnemyStates.PLAYER_ENTERED;
            }
        }
        
        if (!runningState)
        {
            switch (_state)
            {
                case EnemyStates.IDDLE:
                    runningCoroutine = null;
                    break;
                case EnemyStates.PLAYER_ENTERED:
                    runningCoroutine = PlayerEntered(jumpSpeed);
                    break;
                case EnemyStates.DECIDE_TO_MOVE:
                    runningCoroutine = null;
                    if (WillAttackPlayer())
                    {
                        ChangeState(EnemyStates.MOVE_TO_PLAYER);
                    }
                    return;
                case EnemyStates.MOVE_TO_PLAYER:
                    runningCoroutine = WalkToTarget(_target, walkSpeed, distanceToCharge);
                    break;
                case EnemyStates.WAIT_CHARGE:
                    runningCoroutine = PrepareChargeAttack(prepareChargeDuration);
                    break;
                case EnemyStates.CHARGE:
                    runningCoroutine = ChargeAttack(_target.position - transform.position, chargeSpeed, chargeDuration);
                    break;
                case EnemyStates.FLEE:
                    runningCoroutine = RunFromPlayer(transform.position, walkSpeed);
                    break;
                case EnemyStates.DAMAGED:
                    runningCoroutine = Damaged(jumpSpeed, damagedDuration);
                    break;
            }

            StartState();
        }
        
        float dist = DistanceToPath();
        Debug.Log(dist);
        Debug.Log(_state);
        if (DistanceToPath() > maxDistanceAwayFromPath && _state != EnemyStates.IDDLE)
        {
            Debug.Log("Stop corroutines: move to iddle");
            StopAllCoroutines();
            ChangeState(EnemyStates.IDDLE);
        }

        if (_state == EnemyStates.IDDLE)
        {
            WalkPath();
        }

        UpdateYVelocity();
    }


    private void CollideWithPlayer(AstronautController astronaut)
    {
        Vector3 direction = astronaut.transform.position - transform.position;
        direction.y = 0;
        Vector3 nDirection = direction.normalized;
        nDirection.x *= Mathf.Sign(direction.x);
        nDirection.z *= Mathf.Sign(direction.z);

        astronaut.Push(_pushHorizontalSpeed, _pushYSpeed, new Vector3(direction.x, 1, direction.z));

        // Damage astronaut
        _events.onPlayerPushed(astronaut.gameObject, gameObject);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            AstronautController astronaut = collider.gameObject.GetComponent<AstronautController>();
            if (astronaut != null)
                CollideWithPlayer(astronaut);
        }

        if (collider.tag != "Ground" &&
            !(collider.tag == "Enemy" && collider.gameObject == gameObject) &&
            collider.tag != "Enemy Camp" &&
            _state == EnemyStates.CHARGE)
        {
            StopAllCoroutines();
            runningCoroutine = null;
            ChangeState(EnemyStates.FLEE);
        }
    }
}
