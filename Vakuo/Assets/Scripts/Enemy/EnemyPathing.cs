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
    private Transform _meshObject;
    
    [SerializeField]
    private BezierPathing _pathing;

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private CharacterController _targetCC;

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
    public float maxRotation;

    public float _pushYSpeed;
    public float _pushHorizontalSpeed;

    public int _lifes = 1;
    private bool _dead = false;

    private bool _isJumpFrame = false;

    private bool _isInvulnerable = false;
    private bool _isRunningInvulnerable = false;

    private void OnEnable()
    {
        StopAllCoroutines();
        runningCoroutine = null;
        ChangeState(EnemyStates.IDDLE);
    }
    

    public void DamagedByPlayer()
    {
        Debug.Log("DamagedByPlayer:" + _isInvulnerable);
        if (_isInvulnerable)
            return;

        StopAllCoroutines();
        ChangeState(EnemyStates.DAMAGED);
    }

    private void RunWaitInvulnerability()
    {
        if (_isInvulnerable && !_isRunningInvulnerable)
        {
            IEnumerator waitInvulnerable = InvulnerableToPlayer(0.5f);
            StartCoroutine(waitInvulnerable);
        }
    }

    private IEnumerator InvulnerableToPlayer(float duration)
    {
        _isRunningInvulnerable = true;
        yield return new WaitForSeconds(duration);
        _isInvulnerable = false;
        _isRunningInvulnerable = false;
    }


    private void ChangeState(EnemyStates state)
    {
        _state = state;
        runningState = false;
    }

    private void StartState()
    {
        if (runningCoroutine != null)
        {
            runningState = true;
            StartCoroutine(runningCoroutine);
        }
    }

    private IEnumerator PlayerEntered(float jumpSpeed)
    {
        if(!_characterController.isGrounded)
        {
            yield return new WaitWhile(() => {
                return !_characterController.isGrounded;
            });
        }

        Jump(jumpSpeed);
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
            Move(direction * moveSpeed * Time.deltaTime);
            //_characterController.Move(direction * moveSpeed * Time.deltaTime);

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
            Move(direction * moveSpeed * deltaTime);
            //_characterController.Move(direction * moveSpeed * deltaTime);

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
        Jump(jumpSpeed);
        if (--_lifes <= 0)
        {
            _dead = true;
        }
        yield return new WaitForSeconds(duration);
        if (!_dead)
            ChangeState(EnemyStates.DECIDE_TO_MOVE);
        else
            Destroy(gameObject);
    }

    private bool WillAttackPlayer()
    {
        //TODO: Communicate between all enemies to decide which one attacks
        return true;
    }


    private void Move(Vector3 displacement)
    {
        _characterController.Move(displacement);
        RotateDirection(displacement);
    }

    private void RotateDirection(Vector3 direction)
    {
        direction = new Vector3(direction.x, 0, direction.z).normalized;
        float rotation = Vector3.Dot(_meshObject.right, direction);
        rotation *= Mathf.Rad2Deg;
        if (Mathf.Abs(rotation) > maxRotation)
        {
            rotation = Mathf.Sign(rotation) * maxRotation;
        }
        _meshObject.Rotate(Vector3.up * rotation);
    }

    private void Jump(float speed)
    {
        yVelocity = speed;
        _isJumpFrame = true;
    }

    private void UpdateYVelocity()
    {
        if (_characterController.isGrounded && yVelocity != 0f && !_isJumpFrame)
        {
            yVelocity = 0f;
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;
        }

        _isJumpFrame = false;
        _characterController.Move(Vector3.up * yVelocity * Time.deltaTime);
    }

    private void WalkPath()
    {
        if(_pathing != null)
        {
            _pathing.WalkPath();
        }
    }


    private float DistanceToPath()
    {
        Vector3 point = transform.InverseTransformPoint(_pathing.BezierCurve.GetPointAt(0));
        return (transform.position - point).magnitude;
    }

    private bool PlayerIsReachable()
    {
        if(_targetCC != null)
        {
            return !(Mathf.Abs(_target.position.y - transform.position.y) > 2f && _targetCC.isGrounded);
        }
        else
        {
            return !(Mathf.Abs(_target.position.y - transform.position.y) > 4f);
        }
    }


    private void UpdateEnemy()
    {
        if (_target != null && _state == EnemyStates.IDDLE)
        {
            float distToP = (_target.transform.position - transform.position).magnitude;
            if (distToP < distToAttack &&
                PlayerIsReachable())
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

        if ((DistanceToPath() > maxDistanceAwayFromPath || !PlayerIsReachable()) && _state != EnemyStates.IDDLE)
        {
            StopAllCoroutines();
            ChangeState(EnemyStates.IDDLE);
        }

        if (_state == EnemyStates.IDDLE)
        {
            WalkPath();
        }
        else if (_state == EnemyStates.WAIT_CHARGE)
        {
            RotateDirection(_target.position - transform.position);
        }
    }

    private void Update()
    {
        if(!_dead)
        {
            RunWaitInvulnerability();
            UpdateEnemy();
        }

        UpdateYVelocity();
    }


    private void CollideWithPlayer(AstronautController astronaut)
    {
        if (_dead)
            return;

        _isInvulnerable = true;
        Vector3 direction = astronaut.transform.position - transform.position;
        direction.y = 0;
        Vector3 nDirection = direction.normalized;
        nDirection.x *= Mathf.Sign(direction.x);
        nDirection.z *= Mathf.Sign(direction.z);

        astronaut.Push(_pushHorizontalSpeed, _pushYSpeed, new Vector3(direction.x, 1, direction.z));

        // Damage astronaut
        _events.onPlayerPushed(astronaut.gameObject, gameObject);
    }

    private void DamagePlayer(PlayerLife astronautLives)
    {
        astronautLives.Damage();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            AstronautController astronaut = collider.gameObject.GetComponent<AstronautController>();
            PlayerLife astronautLifes = collider.gameObject.GetComponent<PlayerLife>();
            if (astronaut != null)
            {
                CollideWithPlayer(astronaut);
                DamagePlayer(astronautLifes);
            }
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
