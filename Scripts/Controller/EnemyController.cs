using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumList;

public class EnemyController : CreatureController {
    FindPathState PathState = FindPathState.UseDirect;
    Coroutine _coDoSomething;
    Coroutine _coFindPath;
    float _coDoTime = 1.0f;
    float _coFIndPathTime = 0.25f;
    GameObject _player;
    GameObject _dropItem;
    Grid _grid;
    int xCount;
    int yCount;
    int usePathStackCount = 0;
    Stack<Vector3> _pathStack;
    Vector3 nextPos;
    bool _deadFlag = true;

    // TODO
    // 몬스터 종류마다 클래스를 따로 두고 관리하는 것보다
    // 하나의 Enemy 클래스로 두고 데이터 값에 따라 몬스터를 분류하는 것이
    // 새로운 몬스터를 추가 / 관리하기 편하다
    protected override void Init() {
        base.Init();
        HP = 10;
        _speed *= 0.4f;

        GunInit();

        _player = GameObject.Find("Player");
        _target = _player.transform;

        _grid = Manager.Map.Grid;
        xCount = Manager.Map.xCount;
        yCount = Manager.Map.yCount;
    }


    protected override void UpdateController() {
        if (_coDoSomething == null && Manager.PlayerData.GamePlayState == GameState.Playing) {
            _coDoSomething = StartCoroutine("CoDoSomething");
        }
        else if(Manager.PlayerData.GamePlayState == GameState.GameOver || Manager.PlayerData.GamePlayState == GameState.Ending){
            State = CreatureState.Idle;
        }

        _rigidbody.velocity = Vector2.zero;


        switch (State) {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Move:  
                UpdateAttack();
                UpdateMoving();
                break;
            case CreatureState.Attack: 
                UpdateAttack();
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

    protected override void UpdateMoving() {
        if (_coFindPath == null) {
            _coFindPath = StartCoroutine("CoFindPath");
        }

        base.UpdateMoving();
    }
    
    protected override void UpdateDead() {

        // temp random rate
        if (_deadFlag) {
            Debug.Log("Eenemy : Dead!");

            _deadFlag = false;
            int rand = UnityEngine.Random.Range(0, 100);

            // 70프로의 확률로 아무 것도 드랍 안 함
            // 10프로의 확률로 HP 포션
            // HP 포션은 접근시 자동 섭취
            if (rand >= 15 && rand < 65) {
                ChooseDropItem(ItemType.Item, rand);
            }
            // 10프로의 확률로 normal 총기
            // 그 중에서 5/5로 pistol or assaultRifle
            else if (rand >= 65 && rand < 80) {
                ChooseDropItem(ItemType.Gun, rand, 0);
            }
            // 5프로의 확률로 rare 총기
            // 그 중 5/5로 pistol or assaultRifle
            else if (rand >= 80 && rand < 85) {
                ChooseDropItem(ItemType.Gun, rand, 1);
            }
            // 5프로의 확률로 epic 총기
            //else if (rand >= 85 && rand < 90) {

            //}
            // 2프로의 확률로 fever time 아이템
            // fever 아이템은 접근시 자동 섭취, 원하는 떄에 아이템 사용 가능
            // UI로 아이템 먹었는지 표시하기
            else if (rand >= 90 && rand < 100) {
                ChooseDropItem(ItemType.Item, rand);
            }

            Manager.EnemyRespawn.RemainEnemy -= 1;
            Debug.Log(Manager.EnemyRespawn.RemainEnemy);

            Manager.Pool.PushPoolChild(this.gameObject);
            State = CreatureState.Move;
            _deadFlag = true;
        }
    }

    void DropItem(string itemName) {
        _dropItem = Resources.Load<GameObject>($"Prefabs/Items/{itemName}");
        if (_dropItem != null) {
            _dropItem = Object.Instantiate<GameObject>(_dropItem, transform.position, Quaternion.identity);
            _dropItem.name = itemName;
        }
    }

    void ChooseDropItem(ItemType itemType, int rand, int itemRare = 0, int rate = 2) {
        switch (itemType) {
            case ItemType.Item:
                // 포션
                if (rand >= 15 && rand < 65) {
                    DropItem($"{itemType}_HpPotion");
                }
                // Fever
                else if (rand >= 98 && rand < 100) {
                    DropItem($"{itemType}_Fever");
                }
                break;

            case ItemType.Gun:
                int gunType = (rand % 2);
                if (gunType == 0) {
                    DropItem($"{itemType}_{gunType}{itemRare}1");
                }
                else if (gunType == 1) {
                    DropItem($"{itemType}_{gunType}{itemRare}1");
                }
                break;
        }

    }

    // TODO
    IEnumerator CoFindPath() {
        // 플레이어와 일정 거리 이내인 경우 다음 길찾기 중단
        if ((_target.position - transform.position).magnitude < 2.5f) {
            State = CreatureState.Attack;
        }

        // 진행방향 전방 충돌 점검
        RaycastHit2D rayHit1, rayHit2, rayHit3;
        //LayerMask mask = LayerMask.GetMask("Collision") | LayerMask.GetMask("Enemy");
        rayHit1 = Physics2D.Raycast(this.transform.position, _destPos, 2.5f, LayerMask.GetMask("Collision"));
        //rayHit2 = Physics2D.Raycast(this.transform.position + _destPos, new Vector3(_destPos.x - 0.3f, _destPos.y - 0.3f, 0.0f), 2.0f, LayerMask.GetMask("Collision"));
        //rayHit3 = Physics2D.Raycast(this.transform.position + _destPos, new Vector3(_destPos.x + 0.3f, _destPos.y + 0.3f, 0.0f), 2.0f, LayerMask.GetMask("Collision"));


        Debug.DrawRay(this.transform.position, _destPos * 1.5f, Color.red, 1.0f);
        //Debug.DrawRay(this.transform.position + _destPos, new Vector3(_destPos.x - 0.3f, _destPos.y - 0.3f, 0.0f) * 1.5f, Color.blue, 1.0f);
        //Debug.DrawRay(this.transform.position + _destPos, new Vector3(_destPos.x + 0.3f, _destPos.y + 0.3f, 0.0f) * 1.5f, Color.blue, 1.0f);

        // 진행방향이 Collision Layer Map Obj를 향하는 경우에만 FindPath 함수 실시
        //if (rayHit1.transform == null && rayHit2.transform == null && rayHit3.transform == null && _pathStack == null) {
        if (rayHit1.transform == null && _pathStack == null) {

            PathState = FindPathState.UseDirect;

        }
        else if (_pathStack == null || _pathStack.Count == 0) {
            PathState = FindPathState.ReFindPath;
            }
        else if (usePathStackCount > 5) {
            PathState = FindPathState.ReFindPath;
            usePathStackCount = 0;
            }
         else {
            PathState = FindPathState.UsePathStack;
         }
        

        // TODO
        // refindpath와 pathstackisnull 합치기
        switch (PathState) {
            case FindPathState.UseDirect:
                _destPos = (_target.position - transform.position).normalized;
                break;
            case FindPathState.ReFindPath:
                _pathStack = Manager.Map.FindPath(this.transform, _target);
                SetPathUseStack();
                break;
            case FindPathState.UsePathStack:
                SetPathUseStack();
                ++usePathStackCount;
                break;
        }

        yield return new WaitForSeconds(_coFIndPathTime);

        _coFindPath = null;
    }


    void SetPathUseStack() {
        nextPos = _pathStack.Pop();
        if ((_pathStack.Count > 0) && (nextPos - transform.position).magnitude < 0.5) {
            nextPos = _pathStack.Pop();
        }
        _destPos = (nextPos - transform.position).normalized;
    }


    IEnumerator CoDoSomething() {
        int rand = UnityEngine.Random.Range(0, 10);

        if (rand >= 0 && rand < 1) {
            State = CreatureState.Attack;
        }
        else if (rand >= 1 && rand < 10) {
            State = CreatureState.Move;
        }

        switch (State) {
            case CreatureState.Attack:
                _coDoTime = UnityEngine.Random.Range(0, 2);
                break;
            case CreatureState.Move:
                _coDoTime = UnityEngine.Random.Range(2, 7);
                break;
        }

        yield return new WaitForSeconds(_coDoTime);

        _coDoSomething = null;
    }
}