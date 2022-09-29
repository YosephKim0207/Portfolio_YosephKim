using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Bullet : UI_Scene {
    Text _text;

    private void Start() {
        Init();
    }

    protected override void Init() {
        base.Init();

        PlayerController.RemainAmmoAction -= setAmmoCount;
        PlayerController.RemainAmmoAction += setAmmoCount;
        _text = GetComponentInChildren<Text>();
    }


    void setAmmoCount(int ammo) {
        _text.text = $"Ammo {ammo}";
    }
}