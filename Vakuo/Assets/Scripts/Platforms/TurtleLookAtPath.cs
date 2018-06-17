using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleLookAtPath : PlatformTranslator
{
    [SerializeField]
    private GameObject _meshObject;
    [SerializeField]
    private float _speed;
    private bool isPair = true;
    private Quaternion lookAt;

    protected override void Run()
    {
        time += (direction * Time.deltaTime / duration);
        bool pair = Mathf.CeilToInt(time + 0.2f) % 2 == 0;

        // If pair
        if (pair && !isPair)
        {
            lookAt = Quaternion.LookRotation(startPosition - endPosition, Vector3.up);
            isPair = true;
            //SmoothLookAt(Quaternion.LookRotation(endPosition - startPosition, Vector3.up));
        } else if(!pair && isPair)
        {
            lookAt = Quaternion.LookRotation(endPosition - startPosition, Vector3.up);
            isPair = false;
        }

        SmoothLookAt(lookAt);

        switch (_type)
        {
            case PlatformType.PINGPONG:
                LoopPingPong();
                break;
            case PlatformType.REPEAT:
                LoopRepeat();
                break;
        }

        PreformeTransform();
    }

    private void SmoothLookAt(Quaternion lookDirection)
    {
        _meshObject.transform.rotation = Quaternion.Slerp(_meshObject.transform.rotation, lookDirection, _speed * Time.deltaTime);
    }
}
