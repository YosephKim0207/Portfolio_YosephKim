using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumList;

public class HpPotionController : ItemController {
    int _recoverHp = 15;
    float _disappearTime = 12.0f;
    float _countDown = 0.0f;
    bool _takeItem = false;
    Vector3 _destPos = Vector3.zero;

    protected override void Init() {
        _itemName = ItemName.HpPotion;

    }

    private void Update() {
        // 포션 습득시 player와의 상호작용
        if (_takeItem) {
            _countDown = 0.0f;
            _destPos = _playerController.transform.position;
            this.transform.position += (_destPos - this.transform.position).normalized;
        }

        if(_takeItem && (_destPos - this.transform.position).magnitude < 1.0f) {
            if (_playerController.HP > 85) {
                _playerController.HP = 100;
            }
            else {
                _playerController.HP += _recoverHp;
            }

            Destroy(this.gameObject);
        }

        // 포션 드롭 이후 자동 소멸을 위한 코드
        _countDown += Time.deltaTime;

        if(_countDown > _disappearTime) {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController>() != null) {
            _playerController = collision.GetComponent<PlayerController>();

            _takeItem = true;
        }

    }

    
}
