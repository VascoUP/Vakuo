using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlatformTranslator : ScriptActivator
{
    private enum PlatformType { PINGPONG, REPEAT };

    [SerializeField]
    private PlatformType _type;

    [SerializeField]
    private Rigidbody _rigidbody;

    public AnimationCurve accelCurve;
    public Vector3 startPosition = -Vector3.forward;
    public Vector3 endPosition = Vector3.forward;

    public float duration = 1;
    protected float position = 0f;
    protected float time = 0f;
    protected float direction = 1f;
    
    protected override void Run() {
        time += (direction * Time.deltaTime / duration);

        switch(_type) {
            case PlatformType.PINGPONG:
                LoopPingPong();
                break;
            case PlatformType.REPEAT:
                LoopRepeat();
                break;
        }

        PreformeTransform();
    }

    protected void PreformeTransform() {        
        var curvePosition = accelCurve.Evaluate(position);
        var pos = Vector3.Lerp(startPosition, endPosition, curvePosition);

        if (Application.isEditor && !Application.isPlaying)
            _rigidbody.transform.position = pos;
        _rigidbody.MovePosition(pos);
    }
    
    protected void LoopPingPong()
    {
        position = Mathf.PingPong(time, 1f);
    }

    protected void LoopRepeat()
    {
        position = Mathf.Repeat(time, 1f);
    }
/*
    void LoopOnce()
    {
        position = Mathf.Clamp01(time);
        if (position >= 1)
        {
            enabled = false;
            if (OnStopCommand != null) OnStopCommand.Send();
            direction *= -1;
        }
    }
 */
}