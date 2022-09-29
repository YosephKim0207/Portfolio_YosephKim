using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    static Manager s_instance;
    static Manager Instance { get { Init(); return s_instance; } }

    MapManager _map = new MapManager();
    PoolManager _pool = new PoolManager();
    DataManager _data = new DataManager();
    MouseManager _mouse = new MouseManager();
    PlayerDataManager _playerData = new PlayerDataManager();
    EnemyRespawnManager _enemyRespawn = new EnemyRespawnManager();
    UIManager _ui = new UIManager();
    SoundManager _sound = new SoundManager();

    public static MapManager Map { get { return Instance._map; } }
    public static PoolManager Pool {get { return Instance._pool; } }
    public static DataManager Data { get { return Instance._data; } }
    public static MouseManager Mouse { get { return Instance._mouse; } }
    public static PlayerDataManager PlayerData { get { return Instance._playerData; } }
    public static EnemyRespawnManager EnemyRespawn { get { return Instance._enemyRespawn; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static SoundManager Sound { get { return Instance._sound; } }

    private void Start() {
        Init();
    }

    private void Update() {
    }

    static void Init() {
        if (s_instance == null) {
            GameObject go = GameObject.Find("@Manager");
            if (go == null) {
                go = new GameObject { name = "@Manager" };
                go.AddComponent<Manager>();

                Application.targetFrameRate = 60;
                QualitySettings.vSyncCount = 1;
            }

            // TODO
            // 씬 전환시 문제 테스트 용으로 dontDestroyOnLoad 주석처리 
            //DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Manager>();

            s_instance._pool.Init();
            s_instance._data.Init();
            s_instance._mouse.Init();
            s_instance._enemyRespawn.Init();
            s_instance._sound.Init();
        }
    }

}
