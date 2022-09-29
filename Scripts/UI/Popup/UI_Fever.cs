using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fever : UI_Popup {
    Image _image;
    Color _color;
    float time = 0.0f;
    float _passTime = 0.0f;
    float _count = 0.0f;
    float _ratio = 0.0f;
    bool _startFade = false;

    protected override void Init() {
        base.Init();

        _image = GetComponentInChildren<Image>();
        _color = _image.color;
        PlayerController.FeverAction -= SetFade;
        PlayerController.FeverAction += SetFade;
    }

    void SetFade(float feverTime) {
        time = feverTime;
        _count = time / 0.01666667f;
        _ratio = 1.0f / _count;

        _startFade = true;
    }

    private void FixedUpdate() {
        if (_startFade && (_passTime < time)) {
            _passTime += Time.fixedDeltaTime;

            _color.a -= _ratio;
            _image.color = _color;
            
            
        }

        if(_passTime > time) {
            _startFade = false;
            PlayerController.FeverAction -= SetFade;
            _passTime = 0.0f;
            Destroy(this.gameObject);
        }
    }

   
}