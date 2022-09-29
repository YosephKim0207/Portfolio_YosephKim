using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base {
    protected virtual void Start() {
        Init();
    }
    protected virtual void Init() {
        Manager.UI.SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI() {
        Manager.UI.ClosePopupUI(this);
    }
}
