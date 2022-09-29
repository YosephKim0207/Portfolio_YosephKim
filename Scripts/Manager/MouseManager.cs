using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager {
    //GameObject _mouse;
    Camera _cam;
    Vector3 mousePos;
    Texture2D _defaultCursor;
    Texture2D _reloadCursor;
    Vector2 _defaultHotspot;
    Vector2 _reloadHotspot;

    public void Init() {
        _cam = Camera.main;
        mousePos = Vector3.zero;

        _defaultCursor = Resources.Load<Texture2D>("Sprites/Mouse_default");
        _defaultHotspot = new Vector2(_defaultCursor.width / 2, _defaultCursor.height / 2);
        Cursor.SetCursor(_defaultCursor, _defaultHotspot, CursorMode.Auto);
        _reloadCursor = Resources.Load<Texture2D>("Sprites/Mouse_reload");
        _reloadHotspot = new Vector2(_reloadCursor.width / 2, _reloadCursor.height / 2);
    }

    public Vector3 CheckMousePos() {
        if(_cam != null) {
            mousePos.x = Input.mousePosition.x;
            mousePos.y = Input.mousePosition.y;
            mousePos.z = 10.0f;
            mousePos = _cam.ScreenToWorldPoint(mousePos);

            return mousePos;
        }

        return mousePos;

    }

    public void ReloadMouseShape() {

        Cursor.SetCursor(_reloadCursor, _reloadHotspot, CursorMode.Auto);
    }

    public void DefaultMouseShape() {
        Cursor.SetCursor(_defaultCursor, _defaultHotspot, CursorMode.Auto);
    }

    //public Transform CheckMousePos() {
    //    mousePos.x = Input.mousePosition.x;
    //    mousePos.y = Input.mousePosition.y;
    //    mousePos.z = 10.0f;
    //    _mouse.transform.position = _cam.ScreenToWorldPoint(mousePos);

    //    return _mouse.transform;
    //}

}
