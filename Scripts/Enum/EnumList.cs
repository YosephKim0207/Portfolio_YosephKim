using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumList {
    public enum CreatureState {
        Idle,
        Move,
        Attack,
        Fever,
        Jump,
        Damaged,
        Dead,
    }

    public enum CreatureDir {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        None,
    }

    public enum GameState {
        Playing,
        GameOver,
        Ending,
    }

    public enum SoundType {
        BGM,
        Effect,
        MaxTypeCount,
    }

    public enum FindPathState {
        Null,
        UseDirect,
        ReFindPath,
        UsePathStack,
    }

    public enum CreatureType {
        Null,
        Player,
        Enemy,
    }

    public enum CanEquip {
        Yes,
        No,
    }

    public enum RespawnPattern {
        NormalRandom,
        Square,
    }

    public enum ItemType {
        Gun,
        Item,
    }

    public enum ItemName {
        HpPotion,
        Fever,
    }

    public enum Scene {
        Unknown,
        Title,
        Lobby,
        Game,
    }

    public enum LayoutGroupList {
        None,
        Horizon,
        Vertical,
        Grid,
    }
}