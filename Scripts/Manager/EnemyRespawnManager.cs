using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static EnumList;

public class EnemyRespawnManager {
    Camera _cam;
    int _respawnRange = 5;
    int _remainEnemy = 0;
    public int RemainEnemy { get { return _remainEnemy; } set { _remainEnemy = value; } }
    MapManager.Pos pos;
    MapManager.Pos playerPos;
    MapManager.Pos respawnRangePos;
    Stack<MapManager.Pos> respawnPosStack = new Stack<MapManager.Pos>();
    Vector3 viewLeftDownPos = new Vector3(0.0f, 0.0f, -10.0f);
    Vector3 vidwRightUpPos = new Vector3(1.0f, 1.0f, -10.0f);
    float _minX;
    float _maxX;
    float _minY;
    float _maxY;
    GameObject go;

    MapManager.Pos[] respawnArray;


    public void Init() {
        bool[,] _collisionData = Manager.Map._collsionData;
        _cam = Camera.main;
        CheckMaxCamRange();
    }




    public void Respawn(Vector3 _playerTransPos, GameObject enemy, RespawnPattern respawnPattern = RespawnPattern.NormalRandom, int density = 1) {
        respawnPosStack.Clear();
        playerPos = Manager.Map.CellToArrPos(Manager.Map.Grid.WorldToCell(_playerTransPos));


        switch (respawnPattern) {
            case RespawnPattern.NormalRandom:
                break;
            case RespawnPattern.Square:
                // Temp
                SetMaxRespawnRange(_playerTransPos, CheckMaxCamRange());

                SetSquarePattern(_respawnRange, playerPos);
                MakeEnemy(enemy, density);
                break;
        }

    }

    void MakeEnemy(GameObject enemy, int density = 1) {
        // TODO respawn 좌표 만들기
        //while (respawnPosStack.Count != 0) {
        //    pos = respawnPosStack.Pop();
        //    _remainEnemy += 1;
        //    Vector3Int cellPos = Manager.Map.ArrToCellPos(pos);
        //    Object.Instantiate(enemy, cellPos, Quaternion.identity);
        //}
        //respawnPosStack.Clear();
        if (density < 1 || density > respawnPosStack.Count) {
            density = respawnPosStack.Count / 4;
        }

        for (int i = 0; i < density; ++i) {
            pos = respawnArray[i];
            Vector3Int cellPos = Manager.Map.ArrToCellPos(pos);
            //Object.Instantiate(enemy, cellPos, Quaternion.identity);
            go = Manager.Pool.UsePool(enemy);
            go.transform.position = cellPos;
            go = null;

            RemainEnemy += 1;
            //Debug.Log(Manager.EnemyRespawn.RemainEnemy);

        }

    }

    float CheckMaxCamRange() {
        // cam이 현재 반대에서 촬영 중인 것 감안하여 min/max 바뀌어서 변수에 저장
        // Player의 위치가 달려져도 worldLefDownPos에 저장되는 월드좌표가 x = 10임 뭔가 이상 확인 필요 
        Vector3 worldLeftDownPos = _cam.ViewportToWorldPoint(viewLeftDownPos);
        Vector3 worldRightUpPos = _cam.ViewportToWorldPoint(vidwRightUpPos);
        _maxX = worldLeftDownPos.x;
        _minX = worldRightUpPos.x;
        _maxY = worldLeftDownPos.y;
        _minY = worldRightUpPos.y;

        //Debug.Log($"Cam maxX : {_maxX}");

        return _maxX;
    }

    // 꼭지점 제외한 사각형 꼴의 리스폰 희망 위치들을 respawnPosStack에 Push
    void SetSquarePattern(int distance, MapManager.Pos playerArrPos) {
        if (distance < 1) {
            distance = 10;
        }


        MapManager.Pos respawnPos = playerArrPos;
        respawnPos.X -= distance;
        respawnPos.Y -= distance;
        int initYPos = respawnPos.Y;

        // 리스폰 지역 설정 및 충돌 체크
        for (int i = 0; i <= distance * 2; ++i) {
            for (int j = 0; j <= distance * 2; ++j) {

                // 좌상단 / 우하단 모서리
                if (i == j) {
                    ++respawnPos.Y;
                    continue;
                }

                // 프레임 내부
                if ((i != 0 && i != distance * 2) && (j > 0 && j < distance * 2)) {
                    ++respawnPos.Y;
                    continue;
                }

                // 우상단 / 좌하단 모서리
                if (i == (distance * 2 - j)) {
                    ++respawnPos.Y;
                    continue;
                }

                // collision이 없고, map 영역 이내에서 respawn 되도록 설정
                if (Manager.Map.CheckCollision(respawnPos)) {
                    respawnPosStack.Push(respawnPos);
                    ++respawnPos.Y;
                }

                // enemy respawn 위치를 랜덤 순서로 저장
                respawnArray = respawnPosStack.OrderBy(pos => Random.value).ToArray();
            }

            respawnPos.Y = initYPos;
            ++respawnPos.X;
        }
    }

    // TODO 현 시점 카메라 범위로 NPC 생성시 극심한 렉 발생, 최적화 필요
    // A* Paht쪽 문제인 것 같음
    void SetMaxRespawnRange(Vector3 playerTransPos, float maxCamRange) {
        playerTransPos.x = maxCamRange - playerTransPos.x;
        respawnRangePos = Manager.Map.CellToArrPos(Manager.Map.Grid.WorldToCell(playerTransPos));

        _respawnRange = respawnRangePos.X - playerPos.X + 1;
    }
}