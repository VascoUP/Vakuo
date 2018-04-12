using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static T GetComponentOnGameObject<T>(string name)
    {
        GameObject gameObject = GameObject.Find(name);
        if (gameObject == null)
            return default(T);
        return gameObject.GetComponent<T>();
    }
}
