using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene {
    protected override void Init() {
        base.Init();

        SceneType = EnumList.Scene.Lobby;

    }

    public override void SetUI() {
    }
}
