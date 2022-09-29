using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Timer : UI_Scene {
    Text _text;

    int _countDown = 0;
    Coroutine _coCountDown;
    WaitForSeconds timeCount = new WaitForSeconds(1.0f);
    static public System.Action<int> TimeAction;

    private void Start() {
        Init();
    }

    protected override void Init() {
        base.Init();
        _countDown = 0;
        _text = GetComponentInChildren<Text>();
        _text.text = $"경과시간 : {_countDown}";
    }

    void Update() {
        if (_coCountDown == null) {
            _coCountDown = StartCoroutine("CoCountDown");
        }
    }

    IEnumerator CoCountDown() {
        yield return timeCount;
        _coCountDown = null;
        _countDown += 1;
        _text.text = $"경과시간 : {_countDown}";
        //if(_countDown > 1) {
            TimeAction.Invoke(_countDown);
        //}
    }
}