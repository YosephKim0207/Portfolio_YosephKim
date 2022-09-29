using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension {
    public static T GetUIFrom<T>(this GameObject go, string name = null) where T : UnityEngine.Object {
        if(name != null) {
            Transform transform = go.transform.Find(name);
            return transform.GetComponent<T>();
        }

        foreach (T component in go.GetComponentsInChildren<T>()) {
            return component;
        }

        return null;
    }
}
