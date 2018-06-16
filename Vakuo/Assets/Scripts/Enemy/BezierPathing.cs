using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPathing : MonoBehaviour {

	private BezierCurve _bezierCurve;
	public BezierCurve BezierCurve {
        get {
            if(_bezierCurve == null)
            {
                _bezierCurve = GetComponent<BezierCurve>();
            }
            return _bezierCurve;
        }
    }

    public Transform meshObject;

	public CharacterController cc;

    public float walkSpeed;
	public float turnSpeed;

    [Range(0,15)]
	public float percentagePerSecond;

    private float _percentage = 0f;

    /*
    private void Update ()
    {
        WalkPath();
    }
    */

    public bool WalkPath()
    {
        float p = Percentage(1f);
        bool stepped = Step(p, 1f);
        if(stepped)
            _percentage = p;
        return stepped;
    }

    // Returns true if step was completed
    public bool Step(float percentage, float step)
    {
        Vector3 point = BezierCurve.GetPointAt(percentage);
        Vector3 direction = point - transform.position;
        direction = new Vector3(direction.x, 0f, direction.z);

        if (IsAbleToTurn(direction))
        {
            Vector3 move = MoveVector(direction);
            LookDirection(move);
            cc.Move(move);
            return Mathf.Approximately(move.magnitude, direction.magnitude);
        }
        else if(step < 1f)
        {
            LookDirection(direction);
        }
        else
        {
            step /= 2f;
            Step(Percentage(step), step);
            return false;
        }

        return false;
    }

    private Vector3 MoveVector(Vector3 move)
    {
        float magnitude = move.magnitude;
        float walk = walkSpeed * Time.deltaTime;
        Vector3 moveNormalized = move.normalized;
        return moveNormalized * (magnitude > walk ? walk : magnitude);
    }

    private bool IsAbleToTurn(Vector3 direction)
    {
        direction = new Vector3(direction.x, 0f, direction.z);
        float turn = Vector3.Dot(meshObject.right, direction.normalized);
        return Mathf.Abs(turn) > TurnCosine(turnSpeed * Time.deltaTime);
    }

    private void LookDirection(Vector3 direction)
    {
        float turn = Vector3.Dot(meshObject.right, direction.normalized);
        //float maxTurn = TurnCosine(turnSpeed * Time.deltaTime);
        //turn = Mathf.(turn, -maxTurn, maxTurn);
        meshObject.transform.Rotate(Vector3.up * turn * Mathf.Rad2Deg);
    }

    private float Percentage(float step)
    {
        float percentage = _percentage + percentagePerSecond * step * Time.deltaTime * 0.01f;
        if(percentage > 1f)
        {
            percentage -= 1f;
        }
        return percentage;
    }

    private float TurnCosine(float speed)
    {
        return Mathf.Cos(speed * Mathf.Deg2Rad);
    }
}
