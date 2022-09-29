using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager {
    GameObject go;
    int _order = 10;    // UI_Popup 간의 sorting order는 10부터 시작
    

    GameObject RootGo {
        get {
            GameObject rootGo = GameObject.Find("@UI_Root");

            if (rootGo == null) {
                rootGo = new GameObject { name = "@UI_Root" };
            }

            return rootGo;
        }
    }

    GameObject RootCanvas {
        get {
            GameObject rootCanvas = GameObject.Find("RootCanvas");

            if(rootCanvas == null) {
                rootCanvas = Resources.Load<GameObject>($"Prefabs/UI/Popup/Canvas/UI_TitleCanvas");
                rootCanvas = Object.Instantiate(rootCanvas);
                rootCanvas.name = "RootCanvas";
                rootCanvas.transform.SetParent(RootGo.transform);
            }

            return rootCanvas;
        }
    }
        

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    Stack<UI_Scene> _sceneStack = new Stack<UI_Scene>();
    UI_Scene _sceneUI = null;

    public void SetCanvas(GameObject go, bool sort = true) {
        Canvas canvas = go.GetComponent<Canvas>();
        if(canvas == null) {
            canvas = go.AddComponent<Canvas>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort) {
            canvas.sortingOrder = _order;
            ++_order;
        }
        else {
            canvas.sortingOrder = 0;
        }
    }

    // 기본적으로 스크립트를 중심으로 검색 및 반환(오브젝트 명과 스크립트 명을 일치시킬 것)
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene {
        if (string.IsNullOrEmpty(name)) {
            name = typeof(T).Name;
        }

        go = Resources.Load<GameObject>($"Prefabs/UI/Scene/{name}");
        go = Object.Instantiate(go);
        go.name = name;

        go.transform.SetParent(RootGo.transform);

        T scene = go.GetComponent<T>();
        if (scene == null) {
            scene = go.AddComponent<T>();
        }

        _sceneUI = scene;

        _sceneStack.Push(scene);

        go = null;

        return scene;
    }

    public void CloseAllSceneUI() {
        while(_sceneStack.Count > 0) {
            UI_Scene scene = _sceneStack.Pop();
            Object.Destroy(scene.gameObject);
            scene = null;
        }
    }

    // 기본적으로 스크립트를 중심으로 검색 및 반환(오브젝트 명과 스크립트 명을 일치시킬 것)
    public T ShowPopupUI<T>(bool useRootCanvas = true, string name = null) where T : UI_Popup {
        if (string.IsNullOrEmpty(name)) {
            name = typeof(T).Name;
        }

        go = Resources.Load<GameObject>($"Prefabs/UI/Popup/{name}");
        go = Object.Instantiate(go);
        go.name = name;

        if(useRootCanvas) {
            go.transform.SetParent(RootCanvas.transform);
        }
        else {
            go.transform.SetParent(RootGo.transform);
        }

        T popup = go.GetComponent<T>();
        if(popup == null) {
            popup = go.AddComponent<T>();
        }

        _popupStack.Push(popup);

        go = null;

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup) {
        if(_popupStack.Peek() != popup) {
            Debug.Log("Close Popup Failed");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI() {
        if(_popupStack.Count == 0) {
            return;
        }

        UI_Popup popup = _popupStack.Pop();
        Object.Destroy(popup.gameObject);
        popup = null;
        --_order;
    }

    public void CloseAllPopupUI() {
        while(_popupStack.Count > 0) {
            ClosePopupUI();
        }
    }
}
