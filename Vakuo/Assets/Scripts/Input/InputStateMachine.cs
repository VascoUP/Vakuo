using UnityEngine;
using UnityEditor;

public class InputStateMachine
{
    private KeyStateMachine[] _keyStates;

    public InputStateMachine(string[] codes)
    {
        _keyStates = new KeyStateMachine[codes.Length];
        for(int i = 0; i < codes.Length; i++)
        {
            _keyStates[i] = new KeyStateMachine(codes[i]);
        }
    }

    public void Update()
    {
        foreach(KeyStateMachine keyState in _keyStates)
        {
            keyState.Update();
        }
    }

    public KeyStateMachine.InputStatus GetKeyState(string key)
    {
        foreach(KeyStateMachine keyState in _keyStates)
        {
            if (keyState.code == key)
                return keyState.status;
        }

        return KeyStateMachine.InputStatus.IGNORE;
    }
}

public class KeyStateMachine
{
    public enum InputStatus { IGNORE, JUST_PRESSED, PRESSING };
    public InputStatus status = InputStatus.IGNORE;
    public string code;

    public KeyStateMachine(string code)
    {
        this.code = code;
    }

    public void Update()
    {
        if (EvaluateJustPressed(code))
        {
            status = InputStatus.JUST_PRESSED;
        }
        else if(EvaluatePressing(code))
        {
            status = InputStatus.PRESSING;
        }
        else
        {
            status = InputStatus.IGNORE;
        }
    }

    private bool EvaluateJustPressed(string code)
    {
        return Input.GetButtonDown(code);
    }

    private bool EvaluatePressing(string code)
    {
        return Input.GetButton(code) && (status == InputStatus.JUST_PRESSED || status == InputStatus.PRESSING);
    }
}