using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour {
    public EnumList.Scene SceneType { get; protected set; } = EnumList.Scene.Unknown;

    void Start() {
        Init();
    }

    protected virtual void Init() {
        Object evtObj = GameObject.FindObjectOfType(typeof(EventSystem));
        if(evtObj == null) {
            GameObject go = Resources.Load<GameObject>("Prefabs/EventSystem");
            go = Object.Instantiate(go);
            go.name = "@EventSystem";
        }
    }

    public abstract void SetUI();
}
