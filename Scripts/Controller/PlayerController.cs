using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static EnumList;

public class PlayerController : CreatureController {
    GameObject go;
    GameObject _hand;
    GameObject _fieldItem;
    Coroutine _coIsFever;
    CanEquip _canEquip = CanEquip.No;
    float _camShakeTimer;
    float camShakeLimit = 0.04f;
    float camShakeX = 0.1f;
    float camShakeY = 0.025f;
    public bool _hasGun { get; private set; }
    bool _hasFever = false;
    bool _equipPossible = true;
    public bool HasFever {
        get { return _hasFever; }
        set {
            _hasFever = value;
        }
    }
    bool _isFever = false;

    static public Action<int> HpAction = null;
    static public Action<int> RemainAmmoAction = null;
    static public Action<float> FeverAction = null;

    public override int HP {
        get => base.HP;
        set {
            base.HP = value;

            // HP UI들에게 송출
            HpAction.Invoke(value);

            if(HP == 0) {
                Manager.PlayerData.GamePlayState = GameState.GameOver;

            }
        }
    }

    public bool IsFever {
        get { return _isFever; }
        set {
            _isFever = value;

            if (_isFever && (_coIsFever == null)) {
                Debug.Log("Fever Is Start");
                FeverAction.Invoke(FeverTime);
                _coIsFever = StartCoroutine("CoIsFever");
                _CoMakeBulletWaitSec = new WaitForSeconds(0.0f);
                _equipedGun.IsFever = true;
            }
            else if(!_isFever) {
                _CoMakeBulletWaitSec = new WaitForSeconds(GunInfo.shootCoolTime);
                _equipedGun.IsFever = false;
            }
        }
    }
    public float FeverTime { get; set; }

    private CanEquip PlayerCanEquip {
        get { return _canEquip; }
        set { _canEquip = value; }
    }

    protected override void Init() {
        base.Init();
        Manager.PlayerData.GamePlayState = GameState.Playing;
        _cam = Camera.main;
        IsCamShake = false;
        _camShakeTimer = 0.0f;

        HpAction = null;
        RemainAmmoAction = null;
        FeverAction = null;

        go = Resources.Load<GameObject>("Prefabs/Player_Gun_Hand");
        if (go != null) {
            _hand = UnityEngine.Object.Instantiate(go, transform);
            _hand.name = "Player_Hand";
            _hand.transform.position = new Vector3(-0.3f, -0.3f, 0.0f);
        }

        // 플레이어 생성시 총기 들고 있는 경우 사용
        // GunInit();
        

        go = null;
        _hp = 100;

    }

    void LateUpdate() {
        if (!IsCamShake) {
            _cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
        }
        else {
            _cam.transform.position = new Vector3(transform.position.x + camShakeX, transform.position.y + camShakeY, -10.0f);

            _camShakeTimer += Time.fixedDeltaTime;

            if (_camShakeTimer > camShakeLimit) {
                IsCamShake = false;
                _camShakeTimer = 0.0f;
            }
        }
    }

    protected override void UpdateController() {
        if(Manager.PlayerData.GamePlayState != GameState.Playing) {
            State = CreatureState.Dead;
        }
        
        Manager.PlayerData.playerPosition = transform;

        _hand.transform.position = transform.position + new Vector3(-0.3f, -0.3f, 0.0f);
        //Debug.Log(Manager.Map.Grid.CellToWorld(Vector3Int.FloorToInt(transform.position)));
        switch (State) {
            case CreatureState.Idle:
                GetAction();
                break;
            case CreatureState.Move:
                GetAction();
                break;
            case CreatureState.Attack:
                GetAction();
                break;
            case CreatureState.Damaged:
                break;
        }
        

        base.UpdateController();
    }
    private void Update() {
    }

    void GetAction() {

        if(_isJump == false) {
            // 이동 관련 행동
            GetMoveDir();

            // UI 클릭시 반환
            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }

            // 총알 발사 관련 행동 
            if (Input.GetMouseButton(0) && GetComponentInChildren<GunController>()) {
                if (State.Equals(CreatureState.Idle) || State.Equals(CreatureState.Move)) {
                    State = CreatureState.Attack;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                State = CreatureState.Idle;
            }

            // 아이템 습득 관련 행동 (player가 E키를 누름 && player주변에 아이템이 있는 경우) 
            if (Input.GetKeyDown(KeyCode.E) && PlayerCanEquip.Equals(CanEquip.Yes) && _equipPossible) {
                _equipPossible = false;
                GetEquip();
                //Debug.Log("Take Gun!");
            }

            if (Input.GetKeyDown(KeyCode.F) && HasFever) {
                IsFever = true;
            }
        }
    }

    void GetMoveDir() {
        if (Input.GetKey(KeyCode.W)) {
            MoveDir = CreatureDir.Up;

            if (Input.GetKey(KeyCode.A)) {
                MoveDir = CreatureDir.UpLeft;
            }
            else if (Input.GetKey(KeyCode.D)) {
                MoveDir = CreatureDir.UpRight;
            }
        }
        else if (Input.GetKey(KeyCode.S)) {
            MoveDir = CreatureDir.Down;

            if (Input.GetKey(KeyCode.A)) {
                MoveDir = CreatureDir.DownLeft;
            }
            else if (Input.GetKey(KeyCode.D)) {
                MoveDir = CreatureDir.DownRight;
            }
        }
        else if (Input.GetKey(KeyCode.A)) {
            MoveDir = CreatureDir.Left;
        }
        else if (Input.GetKey(KeyCode.D)) {
            MoveDir = CreatureDir.Right;
        }
        else {
            MoveDir = CreatureDir.None;
        }
    }

    protected override void UpdateMoving() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            _makeJump = true;
        }

        base.UpdateMoving();
    }

    protected override void UpdateJump() {
       base.UpdateJump();

        _makeJump = false;
    }

    protected override void UpdateAttack() { 
        //Debug.Log($"{_target.position}");
        base.UpdateAttack();

    }

    #region 아이템 탐지
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<GunController>() != null) {
            //Debug.Log($"{collision.name} can Equip");
            PlayerCanEquip = CanEquip.Yes;
            _fieldItem = collision.gameObject;
        }

        // item 주도적 움직으로 바꾸기 
        else if (collision.GetComponent<ItemController>() != null) {
            ItemController itemController = collision.GetComponent<ItemController>();
            switch (itemController.ItemName) {
                case ItemName.HpPotion:
                    break;
                case ItemName.Fever:
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.GetComponent<GunController>()) {
            PlayerCanEquip = CanEquip.No;
            _fieldItem = null;
        }
    }
    #endregion

    void GetEquip() {
        TakeGun();
    }

    IEnumerator CoIsFever() {
        yield return new WaitForSeconds(FeverTime);
        _equipedGun.IsFever = false;
        IsFever = false;
        HasFever = false;
        _coIsFever = null;
    }



    #region 총기 습득/드롭 관련
    // 총기 위치 손으로 초기화 
    void TakeGun() {
        if (_equipedGun == null) {  // 총을 소지하고 있지 않는 경우
            InitGunPos();
            _equipPossible = true;
        }
        else {  // 총을 소지하고 있는 경우
            DropGun();
            InitGunPos();
            _equipPossible = true;
        }

    }

    void InitGunPos() {
        _equipedGun = _fieldItem.GetComponent<GunController>();
        if(_equipedGun == null) {
            Debug.Log($"{_equipedGun}'s GunContrller is null");
        }
        _equipedGun.transform.SetParent(_hand.transform);
        _equipedGun.name = $"{_fieldItem.name}";
        GunInfo = _equipedGun.getGunInfo;
        _CoMakeBulletWaitSec = new WaitForSeconds(GunInfo.shootCoolTime);

        _fieldItem = null;
        _equipedGun.transform.localPosition = new Vector3(-0.06f, -0.06f, 0.0f);
        ChangeGunSetting(_equipedGun.gameObject, takeGun: true);
        _equipedGun.Equiped = true;
        RemainAmmoAction.Invoke(GunInfo.ammo);

    }

    void DropGun() {
        // 소지 중인 총을 필드에 드롭(복제 VER)
        //GameObject dropItem = Object.Instantiate(_gun, transform.position, _gun.transform.rotation);

        // 소지 중인 총을 필드에 드롭(아이템 옮기기 ver)
        GameObject _dropGun = _equipedGun.gameObject;
        _dropGun.transform.position = transform.position;
        _dropGun.transform.SetParent(null);
        _equipedGun.Equiped = false;

        GameObject.Destroy(_dropGun);
        // 총기 재습득 가능 버전
        // ChangeGunSetting(_dropGun, takeGun: false);
    }

    void ChangeGunSetting(GameObject go, bool takeGun = true) {
        
        GunController itemController = go.GetComponent<GunController>();
        itemController.enabled = !itemController.enabled;
        CircleCollider2D itemCollider = go.GetComponent<CircleCollider2D>();
        itemCollider.enabled = !itemCollider.enabled;
        //SpriteRenderer itemRenderer = go.GetComponent<SpriteRenderer>();
        SpriteRenderer itemRenderer = go.GetComponentInChildren<SpriteRenderer>();


        if (takeGun) {
            itemRenderer.sortingOrder = 40;
            _hasGun = true;
        }
        else {
            itemRenderer.sortingOrder = 20;
            _hasGun = false;
        }
    }
    #endregion

}