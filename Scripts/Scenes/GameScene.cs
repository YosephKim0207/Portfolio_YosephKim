using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EnumList;
using static UI_Base;

public class GameScene : BaseScene {
    const float endindTime = 258.0f;
    const int firstPhaseTime = 69;
    const int secondPhaseTime = 86;
    const int thirdPhaseTime = 207;
    const int finalPhaseTime = 220;
    const int defaultRegenTime = 17;
    float _respawnCycle = 10.0f;
    GameObject _enemy;
    GameObject _rootUI;
    GameObject go;
    Stack<UI_Scene> _scenes;
    bool gameOverPopUp;
    bool endingPopUp;
    GameState State;
    UI_Popup _guidePopup;
    PlayerController playerController;

    protected override void Init() {
        base.Init();

        SceneType = EnumList.Scene.Game;
        Manager.PlayerData.GamePlayState = GameState.Playing;
        SetUI();

        GameObject _map = Manager.Map.LoadMap(001);
        go = Resources.Load<GameObject>("Prefabs/Player");
        GameObject player = Object.Instantiate<GameObject>(go);
        playerController = player.GetComponent<PlayerController>();
        player.name = go.name;

        // 튜토리얼 아이템 생성 및 팝업 설정
        go = Resources.Load<GameObject>("Prefabs/Items/Gun_001");
        go = Object.Instantiate(go, player.transform.position + (Vector3.left * 3), Quaternion.identity);
        go.name = "Gun_001";

        _enemy = Resources.Load<GameObject>("Prefabs/Enemy");
        _enemy.name = "Enemy";

        gameOverPopUp = false;
        endingPopUp = false;

        _rootUI = GameObject.Find("@UI_Root");

        Manager.Sound.Play("BGM/RipAndTear", EnumList.SoundType.BGM, 0.7f);
        UI_Timer.TimeAction = null;
        UI_Timer.TimeAction += RespawnEnemyPhase;

    }

    private void Update() { 
        switch (Manager.PlayerData.GamePlayState) {
            case GameState.Playing:
                if(_guidePopup != null && playerController._hasGun){
                    Manager.UI.ClosePopupUI();
                }

                break;
            case GameState.GameOver:
                GameOver();
                break;
            case GameState.Ending:
                break;
        }
    }

    void RespawnEnemyPhase(int time) {
        //if (time == 5) {
        //    Manager.EnemyRespawn.Respawn(Manager.PlayerData.playerPosition.position, _enemy, RespawnPattern.Square, 2);
        //    Manager.EnemyRespawn.RemainEnemy -= 1;
        //    Debug.Log(Manager.EnemyRespawn.RemainEnemy);

        //    return;
        //}
        if (time > endindTime && Manager.EnemyRespawn.RemainEnemy < 1) {
            Manager.PlayerData.GamePlayState = GameState.Ending;
            Manager.Mouse.DefaultMouseShape();
            Ending();
            return;
        }

        if (time < firstPhaseTime && (time % _respawnCycle) == 0) {
            Manager.EnemyRespawn.Respawn(Manager.PlayerData.playerPosition.position, _enemy, RespawnPattern.Square, 3);
            return;
        }

        else if (time > secondPhaseTime && (time % _respawnCycle) == 0 && time < 128) {
            Manager.EnemyRespawn.Respawn(Manager.PlayerData.playerPosition.position, _enemy, RespawnPattern.Square, 4);
            return;
        }

        else if (time == thirdPhaseTime) {
            Manager.EnemyRespawn.Respawn(Manager.PlayerData.playerPosition.position, _enemy, RespawnPattern.Square, 8);
            _respawnCycle = 9.0f;
            return;
        }

        else if (time == finalPhaseTime - 10) {
            Manager.EnemyRespawn.Respawn(Manager.PlayerData.playerPosition.position, _enemy, RespawnPattern.Square, 0);
            return;
        }

        else if (time == finalPhaseTime) {
            Manager.EnemyRespawn.Respawn(Manager.PlayerData.playerPosition.position, _enemy, RespawnPattern.Square, 0);
            return;
        }
        else if ((time % defaultRegenTime) == 0 & time < finalPhaseTime) {
            Manager.EnemyRespawn.Respawn(Manager.PlayerData.playerPosition.position, _enemy, RespawnPattern.Square, 1);
            return;
        }


    }


    protected void CallGameOverPopup() {
        Manager.Sound.StopAll();
        UI_Popup _popup = Manager.UI.ShowPopupUI<UI_Popup>(false, "Canvas/UI_Popup");
        _popup.name = "UI_Popup";
        _popup.SetText("GameOver", 120);
        _popup.SetButtonData(
            new ButtonData { text = "타이틀 화면", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { Manager.UI.CloseAllPopupUI(); SceneManager.LoadScene(0); } },
            new ButtonData { text = "게임 재시작", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { Manager.UI.CloseAllPopupUI(); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); } },
            new ButtonData { text = "게임 종료", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { Application.Quit(); } }
            );

    }

    protected void CallEndingPopup() {
        Manager.Sound.StopAll();
        Manager.Sound.Play("BGM/RipAndTear", SoundType.BGM, 0.4f);

        UI_Popup _popup = Manager.UI.ShowPopupUI<UI_Popup>(false, "Canvas/UI_Popup");
        Image img = _popup.gameObject.GetUIFrom<Image>("BackGroundPanel");
        img.sprite = Resources.Load<Sprite>($"Sprites/BackGround/Ending");
        _popup.SetText("Victory!", Color.red, 150);

        _popup.SetButtonData(
            new ButtonData { text = "타이틀 화면", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { Manager.UI.CloseAllPopupUI(); SceneManager.LoadScene(0); } },
            new ButtonData { text = "게임 재시작", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { Manager.UI.CloseAllPopupUI(); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); } },
            new ButtonData { text = "게임 종료", img = "Button_01", font = "MainFont", fontSize = 52, DoSomthingAction = (PointerEventData data) => { Application.Quit(); } }
            );
    }


    public override void SetUI() {
        Manager.UI.ShowSceneUI<UI_Canvas>("Canvas/UI_GameCanvas");

        _guidePopup = Manager.UI.ShowPopupUI<UI_Popup>(false, "Canvas/UI_Guide");
        _guidePopup.SetText("E 키를 눌러 총기 습득", Color.black, 100);
    }

    void GameOver() {
        if (!gameOverPopUp) {
            gameOverPopUp = true;
            Manager.UI.CloseAllSceneUI();
            CallGameOverPopup();
        }
    }

    void Ending() {
        if (!endingPopUp) {
            endingPopUp = true;
            Manager.UI.CloseAllSceneUI();
            CallEndingPopup();
        }
    }
}
