using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : UI_Base {
    public GameObject go { get; private set; }
    public string ButtonName { get; set; }
    public string text {
        get; set;
    }

    public string img {
        get; set;
    }

    public UI_Button() {
        ButtonName = "UI_Button";
    }

    private void Start() {
        Init();
    }

    protected void Init() {
        

    }
}
