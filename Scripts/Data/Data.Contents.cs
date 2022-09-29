using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Gun {
    public string name;
    public string gunType;
    public string bulletType;
    public float shootCoolTime;
    public int damage;
    public int ammo;
    public float reloadTime;
}

[Serializable]
public class GunData : ILoader<string, Gun> {
    public List<Gun> gunDatas = new List<Gun>();

    public Dictionary<string, Gun> MakeDict() {
        Dictionary<string, Gun> dict = new Dictionary<string, Gun>();
        foreach (Gun gun in gunDatas) {
            dict.Add(gun.name, gun);
        }

        return dict;
    }
}