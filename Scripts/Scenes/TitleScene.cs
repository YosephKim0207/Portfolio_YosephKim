using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UI_Base;

public class TitleScene : BaseScene {
    protected void CallPopup(PointerEventData data) {
        UI_Popup _popup = Manager.UI.ShowPopupUI<UI_Popup>(false, "Canvas/UI_Popup");
        _popup.name = "UI_Popup";
        _popup.SetText("이동 : WASD\n공격: 마우스 좌클릭\n스페이스 : 구르기\n무기 변경 : 접근 후 E 버튼\nHP 포션 사용 : 습득시 자동 사용\nFever 아이템 사용 : 습득 후 F 버튼", 80);
        _popup.SetButtonData( new ButtonData { text = "뒤로가기", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { _popup.ClosePopupUI(); } });
    }

    protected override void Init() {
        base.Init();
        SetUI();
        Manager.Map.LoadMap(001);
        Manager.Sound.Play("BGM/RipAndTear", EnumList.SoundType.BGM, 0.4f);
    }

    public override void SetUI() {
        UI_Scene _canvas = Manager.UI.ShowSceneUI<UI_Scene>("Canvas/UI_TitleCanvas");
        _canvas.name = "UI_TitleCanvas";
        _canvas.SetText("Raiders", 170);
        _canvas.SetButtonData(
            new ButtonData { text = "싱글 플레이", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { SceneManager.LoadScene("Game"); } },
            new ButtonData { text = "도움말", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = CallPopup },
            new ButtonData { text = "종료", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { Application.Quit(); } });
    }

    
}
