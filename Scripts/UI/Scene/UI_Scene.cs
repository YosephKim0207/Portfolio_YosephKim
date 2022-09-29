using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base {
    protected virtual void Init() {
        Manager.UI.SetCanvas(gameObject, false);
    }
}