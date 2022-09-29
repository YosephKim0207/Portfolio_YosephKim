using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumList;

public abstract class ItemController : MonoBehaviour {
    protected ItemName _itemName;
    protected PlayerController _playerController;
    public ItemName ItemName { get { return _itemName; } }

    protected abstract void Init();

    protected virtual void Start() {
        Init();
    }

}
