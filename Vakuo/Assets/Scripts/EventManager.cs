using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Delegate funtion for when the games enters a new state
public delegate void StateEnter(GameStatus enterState);
// Delegate funtion for when the games exits a state
public delegate void StateExit(GameStatus exitState);
// Delegate funtion that used to call different update funtions
public delegate void UpdateMonoBehavior();

public delegate void GameEvent();

public delegate void ObjectsEvent(GameObject gameObject1, GameObject gameObject2);

public class EventManager : MonoBehaviour {
    public StateEnter onEnterState;
    public StateExit onExitState;
    public GameEvent onPlayerGrounded;
    public ObjectsEvent onPlayerPushed;
    public GameEvent onAttack;
    public GameEvent onEnemyDeath;
}
