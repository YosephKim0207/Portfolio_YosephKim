using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    Vector3 _bullPos;
    Camera _cam;
    public CreatureController creature;
    public Vector3 DestPos { get; set; }
    public Rigidbody2D _rigidbody;
    float _speed = 0.0f;
    float _outRange = 0.05f;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
        _speed = 28f;

    }

    private void OnEnable() {
        _rigidbody.velocity = DestPos * _speed;
    }


    protected virtual void Update() {
        // 카메라 영역+a 벗어나는 경우 총알 제거
        _bullPos = _cam.WorldToViewportPoint(transform.position);
        if (_bullPos.x <= -_outRange || _bullPos.x >= 1.0f + _outRange || _bullPos.y <= -_outRange || _bullPos.y >= 1.0f + _outRange) {
            creature = null;
            Manager.Pool.PushPoolChild(this.gameObject);
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision) {
        // 총알 간의 충돌이 발생하는 경우 || 아이템과 충돌하는 경우 
        if (collision.gameObject.layer.Equals(8) || collision.gameObject.layer.Equals(9)) {
            return;
        }
        // Player가 쏜 총알이 Enemy와 충돌하는 경우
        else if (collision.gameObject.layer.Equals(10)) {
            //Debug.Log("Bullet Hit Player");
            if (creature is PlayerController) {
                EnemyController enemy = collision.GetComponent<EnemyController>();

                if (enemy != null) {
                    enemy.HP -= creature.GunInfo.damage;
                    creature = null;
                    Manager.Pool.PushPoolChild(gameObject);
                }
            }
        }
        // Enemy가 쏜 총알이 Player와 충돌하는 경우
        else if (collision.gameObject.layer.Equals(6)) {
            //Debug.Log("Bullet Hit Enemy");
            if (creature is EnemyController) {
                //Debug.Log("OnTrigger : Enemy to Player");
                // bullet이 player에게 damage 가함 
                PlayerController player = collision.GetComponent<PlayerController>();
                // player Fever인 경우 무적
                if (player.IsFever == false) {
                    player.HP -= creature.GunInfo.damage;
                }

                creature = null;
                Manager.Pool.PushPoolChild(gameObject);
            }
        }
        else {
            Manager.Pool.PushPoolChild(gameObject);
        }

        
    }
}
