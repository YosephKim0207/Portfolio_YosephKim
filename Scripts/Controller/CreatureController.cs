using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumList;

public class CreatureController : MonoBehaviour {

    protected Camera _cam;
    protected bool IsCamShake { get; set; }
    protected float _speed = 10.0f;
    protected float _jumpDist = 5.0f;
    protected float _jumpSpeed;

    protected int _hp;
    public virtual int HP {
        get { return _hp; }
        set {
            // hp가 damage보다 클 경우 Damage State로 전환 및 hp 감소
            if (value <= 0) {
                _hp = 0;
                State = CreatureState.Dead;
            }
            else if(value > 0) {
                State = CreatureState.Damaged;
                _hp = value;
            }

        }
    }

    public Gun GunInfo { get; protected set; }

    protected Coroutine _coPullTrigger;
    Vector3 jumpDest;
    protected bool _isJump = false;
    protected Animator _animator;
    protected Rigidbody2D _rigidbody;
    protected Vector3Int _creatureGridPos = Vector3Int.zero;
    protected GunController _equipedGun;
    protected WaitForSeconds _CoMakeBulletWaitSec;
    protected SpriteRenderer _sprite;
    protected Transform _target;
    public Transform ShootTarget { get { return _target; } }
    protected bool _triggerOn = true;
    public bool TriggerState { get { return _triggerOn; } }
    protected bool _isBulletProof = false;
    protected bool _makeJump = false;
    protected Vector3 _fixedPosition = new Vector3(0.5f, 0.5f);
    protected Vector3 _destPos { get; set; }
    protected CreatureDir _lastDir = CreatureDir.Down;
    CreatureState _state = CreatureState.Idle;

  
    void Start() {
        Init();
    }
    

    protected virtual void Init() {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        GunInfo = new Gun();
    }

    // Fixed Time Stamp == 0.01666667 (1/60 근사치)
    void FixedUpdate() {
        UpdateController();
    }

    protected virtual CreatureState State {
        get { return _state; }
        set {
            if (_state.Equals(value)) {
                return;
            }

            _state = value;
        }
    }

    protected virtual CreatureDir MoveDir {
        get { return _lastDir; }
        set {
            if (value.Equals(CreatureDir.None)) {
                _lastDir = value;
                _destPos = Vector3.zero;
                State = CreatureState.Idle;
                return;
            }

            State = CreatureState.Move;

            switch (value) {
                case CreatureDir.Up:
                    _lastDir = value;
                    _destPos = Vector3.up;
                    break;
                case CreatureDir.Down:
                    _lastDir = value;
                    _destPos = Vector3.down;
                    break;
                case CreatureDir.Left:
                    _lastDir = value;
                    _destPos = Vector3.left;
                    break;
                case CreatureDir.Right:
                    _lastDir = value;
                    _destPos = Vector3.right;
                    break;
                case CreatureDir.UpLeft:
                    _lastDir = value;
                    _destPos = (Vector3.up + Vector3.left).normalized;
                    break;
                case CreatureDir.UpRight:
                    _lastDir = value;
                    _destPos = (Vector3.up + Vector3.right).normalized;
                    break;
                case CreatureDir.DownLeft:
                    _lastDir = value;
                    _destPos = (Vector3.down + Vector3.left).normalized;
                    break;
                case CreatureDir.DownRight:
                    _lastDir = value;
                    _destPos = (Vector3.down + Vector3.right).normalized;
                    break;
            }
        }
    }

    protected virtual void UpdateController() {
        switch (State) {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Move: 
                UpdateMoving();
                break;
            case CreatureState.Attack:
                UpdateAttack();
                UpdateMoving();
                break;
            case CreatureState.Jump:
                UpdateJump();
                break;
            case CreatureState.Damaged:
                UpdateDamaged();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle() {
        _animator.Play("Idle");
        _sprite.flipX = true;
        _rigidbody.velocity = Vector2.zero;
    }

    protected virtual void UpdateMoving() {
        if (!MoveDir.Equals(CreatureDir.None)) {
            _animator.Play("Walk_Down");
            _sprite.flipX = true;
        }
        _rigidbody.MovePosition(transform.position + _destPos * _speed * Time.fixedDeltaTime);

        if (_makeJump) {
            jumpDest = transform.position + _destPos * _jumpDist;
            State = CreatureState.Jump;
        }
        
    }

    protected virtual void UpdateAttack() {
        // _coPullTrigger == null ㄷㅐ신 triggerOn을 조건. triggerOn은 GunController에서 사용
        if (_triggerOn) {
            _coPullTrigger = StartCoroutine("CoPullTrigger");
        }



    }

    protected virtual void UpdateJump() {
        

        _isJump = true;
        _isBulletProof = true;


        //// Player 손 제거
        Transform child = transform.GetChild(0);
        child.gameObject.SetActive(false);

        _animator.Play("Jump");

        _sprite.flipX = true;
        if(MoveDir.Equals(CreatureDir.Right) || MoveDir.Equals(CreatureDir.UpRight) || MoveDir.Equals(CreatureDir.DownRight)) {
            _sprite.flipX = false;
        }
        
        _jumpSpeed = _speed * 2.0f;
        float dist = (jumpDest - transform.position).magnitude;

        RaycastHit2D rayHit;
        rayHit = Physics2D.Raycast(this.transform.position, _destPos, 1.5f, LayerMask.GetMask("Collision"));
        if (rayHit.transform != null) {
            _destPos = Vector3.zero;
        }

        if (dist < _jumpSpeed * Time.fixedDeltaTime && rayHit.transform == null) {
            transform.position = jumpDest;
            State = CreatureState.Idle;
            child.gameObject.SetActive(true);
            _isBulletProof = false;
            _isJump = false;
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
            State = CreatureState.Idle;
            child.gameObject.SetActive(true);
            _isBulletProof = false;
            _isJump = false;
        }
        else {  
            _rigidbody.MovePosition(transform.position + _destPos * _jumpSpeed * Time.fixedDeltaTime);
        }
        
    }

    protected virtual void UpdateDamaged() {
        //Debug.Log($"{gameObject.name}'s HP : {hp}");
        State = CreatureState.Idle;
    }


    protected virtual void UpdateDead() { }

    protected virtual void GunInit() {
        _equipedGun = GetComponentInChildren<GunController>();
        if (_equipedGun == null) {
            Debug.Log($"{name}'s Gun is Empty");
        }
        else {
            GunInfo = _equipedGun.getGunInfo;
            _CoMakeBulletWaitSec = new WaitForSeconds(GunInfo.shootCoolTime);
            _equipedGun.Equiped = true;

        }

    }

    protected virtual IEnumerator CoPullTrigger() {
        _triggerOn = false;

        // Creature가 소지 중인 Gun에서 총알 발사
        // Gun이 Reload 중일 경우 pullTrigger하여도 총알 발사 불가
        if (_equipedGun.Reload == false) {
            _equipedGun.TriggerState = true;

            if(_equipedGun.Creature is PlayerController) {
                IsCamShake = true;
            }
        }

        yield return _CoMakeBulletWaitSec;

        _coPullTrigger = null;
        _triggerOn = true;
    }
}
