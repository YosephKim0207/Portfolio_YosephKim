using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HpBar : UI_Scene {
    Slider _slider;

    private void Start() {
        Init();
    }



    protected override void Init() {
        base.Init();

        _slider = gameObject.GetComponentInChildren<Slider>();

        PlayerController.HpAction -= setHp;
        PlayerController.HpAction += setHp;
    }

    void setHp(int hp) {

        _slider.value = hp / (float)100;
    }
}