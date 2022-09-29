using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Base : MonoBehaviour {
    // Button 프리펩에서 세팅하고자 하는 컴포넌트 반환 받기 e.g Image / Button / Text / etc
    // 현 시점 아래의 함수는 각 Canvas에는 하나 이상의 UI를 장착하지 말 것을 기준으로 한다
    protected virtual T GetUI<T>(string name = null) where T : UnityEngine.Object {
        foreach (T component in gameObject.GetComponentsInChildren<T>()) {
            if (string.IsNullOrEmpty(name)) {
                return component;
            }
        }

        return null;
    }

    protected virtual T GetUIFrom<T>(string name) where T : UnityEngine.Object {
        Transform transform = this.transform.Find(name);

        foreach (T component in transform.GetComponentsInChildren<T>()) {
            if (string.IsNullOrEmpty(name) || component.name == name) {
                return component;
            }
        }

        return null;
    }



    public void SetText(string contents, int fontSize = 36) {
        Text _text = GetUI<Text>();
        _text.text = contents;
        _text.font = Resources.Load<Font>($"Fonts/MainFont");
        _text.fontSize = fontSize;
        _text.color = Color.black;
    }

    public void SetText(string contents, Color color, int fontSize = 36, string fontStyle = "MainFont") {
        Text _text = GetUI<Text>();
        _text.text = contents;
        _text.font = Resources.Load<Font>($"Fonts/{fontStyle}");
        _text.fontSize = fontSize;
        _text.color = color;
    }



    public void SetImg(string imgName) {
        Image _image = GetUI<Image>();
        _image.sprite = Resources.Load<Sprite>($"Sprites/UI/UI_{imgName}");
    }

    public static void BindEvent(GameObject go, Action<PointerEventData> action) {
        UI_EventHandler evtHandler = go.GetComponent<UI_EventHandler>();
        if(evtHandler == null) {
            go.AddComponent<UI_EventHandler>();
            evtHandler = go.GetComponent<UI_EventHandler>();
        }

        evtHandler.OnClickHandler -= action;
        evtHandler.OnClickHandler += action;
    }

    public void SetButtonData(params ButtonData[] _buttonDatas) {
        foreach (ButtonData _data in _buttonDatas) {
            GameObject _button = Resources.Load<GameObject>($"Prefabs/UI/Popup/UI_Button");
            _button = GameObject.Instantiate(_button);
            _button.transform.SetParent(this.GetComponentInChildren<HorizontalOrVerticalLayoutGroup>().transform);
            BindEvent(_button, _data.DoSomthingAction);

            UI_Button _component = _button.GetComponent<UI_Button>();
            _component.SetText(_data.text, _data.fontSize);
            _component.SetImg(_data.img);
            _component.name = "UI_Button";
        }

    }


    public struct ButtonData {
        public string text;
        public string img;
        public string font;
        public int fontSize;
        public Action<PointerEventData> DoSomthingAction;

    }
}