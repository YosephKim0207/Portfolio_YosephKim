using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface ILoader<Key, Value> {
    Dictionary<Key, Value> MakeDict();
}

public class DataManager {
    public Dictionary<string, Gun> gunDict { get; private set; } = new Dictionary<string, Gun>();

    public void Init() {
        gunDict = LoadJson<GunData, string, Gun>("GunData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value> {
        TextAsset textAsset = Resources.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
