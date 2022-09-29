using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager {
    GameObject _map;
    Coroutine _coSleep;
    public bool[,] _collsionData;

    public Grid Grid { get; private set; }
    public int MinX { get; private set; }
    public int MaxX { get; private set; }
    public int MinY { get; private set; }
    public int MaxY { get; private set; }
    public int xCount { get; private set; }
    public int yCount { get; private set; }

    Vector3 _fixedPosition = new Vector3(0.0f, 0.5f);

    public GameObject LoadMap(int mapId) {
        string mapName = "Map_" + mapId.ToString("000");

        GameObject go = Resources.Load<GameObject>($"Prefabs/Maps/{mapName}");
        

        if (go != null) {
            _map = UnityEngine.Object.Instantiate(go);
            _map.name = go.name;
            Grid = go.GetComponent<Grid>();

            TextAsset txt = Resources.Load<TextAsset>($"Data/MapCollisionData/{mapName}");

            StringReader reader = new StringReader(txt.text);

            MinX = int.Parse(reader.ReadLine());
            MaxX = int.Parse(reader.ReadLine());
            MinY = int.Parse(reader.ReadLine());
            MaxY = int.Parse(reader.ReadLine());

            xCount = MaxX - MinX + 1;
            yCount = MaxY - MinY + 1;
            _collsionData = new bool[yCount, xCount];
            
            for (int y = yCount - 1; y >= 0; --y) {
                // line을 안 쓸 경우 줄바꿈도 true/false 판별 진행 
                string line = reader.ReadLine();
                for(int x = 0; x < xCount; ++x) {
                    _collsionData[y, x] = (line[x] == '1' ? true : false);

                }
            }

            return _map;
        }

        return null;
    }

    public bool CheckCollision(Pos cellPos) {
        //Debug.LogError($"cellPOs : {cellPos.X}, {cellPos.Y}");
        if (cellPos.X <= 0 || cellPos.X >= xCount) {
            return false;
        }
        if(cellPos.Y <= 0 || cellPos.Y >= yCount) {
            return false;
        }

        //int x = cellPos.X - MinX;
        //int y = MaxY - cellPos.Y + 1;

        // collsion cellPos에 대해 false return
        //if (_collsionData[cellPos.Y, cellPos.X] == true){
        //    //Debug.Log("coll detect");
        //}
        return !_collsionData[cellPos.Y, cellPos.X];
    }

    // 다중 캐릭터가 장거리에서 동시에 길찾기 실행시 cpu에 과부하 발생 및 프레임 저하
    // 장거리일 경우 일단 직선 경로로 가도록 하기, 가다가 collsion 만나면 그때부터 A* 실행하도록
    #region A*
    // 우선순위큐
    // F : 최종점수 (G + H) - H는 현재부터 목적지까지의 추정 비용
    // G : 현재까지의 비용
    public struct PQNode : IComparable<PQNode> {
        public int F, G, Y, X;

        public int CompareTo(PQNode other) {
            if (F == other.F) {
                return 0;
            }

            return F < other.F ? -1 : 1;
        }
    }

    public struct Pos {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;

        public static bool operator == (Pos pos1, Pos pos2) {
            return (pos1.X == pos2.X) && (pos1.Y == pos2.Y);
        }
        public static bool operator !=(Pos pos1, Pos pos2) {
            return !(pos1 == pos2);
        }
    }

    //List<PQNode> 
    // 8방위 이동정보 및 비용 정보 위 / 아래 / 좌 / 우 / 좌상 / 좌하 / 우상 / 우하 
    int[] _deltaX = new int[] { 1, -1, 0, 0, 1, -1, 1, -1 };
    int[] _deltaY = new int[] { 0, 0, -1, 1, -1, -1, 1, 1 };
    int[] _cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 };
    int _strCost = 10;
    int _digCost = 14;
    
    // grid를 사용하여 pathfind를 하는 이유 : 플레이어는 8방위 이동만 가능하기 때문에
    // pathFind할 때 그리드를 노드로 취급하여 F를 계산하려고 
    public Stack<Vector3> FindPath(Transform curPos, Transform destPos) {
        PriorityQueue<PQNode> openPQ = new PriorityQueue<PQNode>();
        int[,] _openCheck = new int[yCount, xCount];
        bool[,] _closedCheck = new bool[yCount, xCount];
        Pos[,] _parent = new Pos[yCount, xCount];

        // PathFind()로 check되지 않은 경우 true
        for (int y = 0; y < yCount; ++y){
            for(int x = 0; x < xCount; ++x) {
                _openCheck[y, x] = Int32.MaxValue;
                _closedCheck[y, x] = true;
            }
        }

        // 현재 그리드 정보 가져오기
        Pos curArrPos = CellToArrPos(Grid.WorldToCell(curPos.position));
        Pos destArrPos = CellToArrPos(Grid.WorldToCell(destPos.position));
        
        // destArrPos가 Collision 위치인 경우 그 주변에 이동 가능한 곳을 목적지로 보정하기
        destArrPos = CheckCollisionDest(destArrPos);


        int startF = calH(curArrPos, destArrPos);
        if(startF <= 14) {
            Stack<Vector3> _holdPos = new Stack<Vector3>();
            _holdPos.Push(curPos.position);
            return _holdPos;
        }

        PQNode startNode = new PQNode() { F = startF, G = 0, X = curArrPos.X, Y = curArrPos.Y};
        // 시작 노드를 openList에 추가하기
        //Debug.Log($"Start Node : {startNode.X}, {startNode.Y}");
        openPQ.Push(startNode);
        PQNode curNode = new PQNode();
        _parent[startNode.Y, startNode.X] = curArrPos;
        int count = 0;
        List<PQNode> stepTracker = new List<PQNode>();
        List<PQNode> searchTracker = new List<PQNode>();
        List<Pos> colTracker = new List<Pos>();
        List<PQNode> closedTracker = new List<PQNode>();
        List<Pos> openPassTracker = new List<Pos>();
        // destCellPos에 도착할 때까지 탐색하는 while문 넣기
        while (openPQ.Count > 0) {
            curNode = openPQ.Pop();
            stepTracker.Add(curNode);
            //Debug.Log($"Start ({curNode.X}, {curNode.Y})'s (F : {curNode.F}) NextStep");
            ++count;
            if (curNode.Y == destArrPos.Y && curNode.X == destArrPos.X) {
                //Debug.Log("CurNode == destArrPos check");

                break;
            }

            // 다른 노드에서 탐색 중에 이미 방문한 경우 skip
            if (_closedCheck[curNode.Y, curNode.X] == false) {
                closedTracker.Add(curNode);
                continue;
            }

            // startNode를 closed로 넣기
            _closedCheck[curNode.Y, curNode.X] = false;

            

            // TODO 8방위 F 구하여 우선순위큐에 넣기
            for (int i = 0; i < _deltaY.Length; ++i) {
                // 체크할 위치 설정
                Pos nextArrPos = new Pos(curNode.Y + _deltaY[i], curNode.X + _deltaX[i]);
                //Debug.Log($"Check {nextArrPos.X} , {nextArrPos.Y}");


                // nextPos가 이동 불가능한 cell의 좌표인 경우 다음 방위를 체크
                //Debug.Log($"nextCellPos : {nextCellPos}");
                bool colCheck = Manager.Map.CheckCollision(nextArrPos);
                if (!colCheck) {
                    //Debug.Log("nextPos's Collsion Check");
                    if (!colCheck) {
                        colTracker.Add(nextArrPos);
                    }
                    
                    continue;
                }


                // nextPos가 이미 점검한 pos라면 continue
                if(_closedCheck[nextArrPos.Y, nextArrPos.X] == false) {
                    //Debug.Log("closed check");
                    
                    continue;
                }

                // 휴리스틱 체크
                int _h = calH(nextArrPos, destArrPos);
                // nextPos까지 이동하는데 소요된 cost
                int _g = curNode.G + _cost[i];

                //Debug.Log($"G + H : {_g + _h}");
                
                
                // nextPos가 한 번도 탐색된 적 없다면 int32.maxValue일 것이고, 이미 탐색되었더라도 현 경로가 이 cell 최단경 유 경로
                if (_openCheck[nextArrPos.Y, nextArrPos.X] > _g + _h) {
                    //Debug.Log("open Check");
                    _openCheck[nextArrPos.Y, nextArrPos.X] = _g + _h;

                    PQNode node = new PQNode();

                    node.Y = nextArrPos.Y;
                    node.X = nextArrPos.X;
                    node.G = _g;
                    node.F = node.G + _h;

                    searchTracker.Add(node);
                    openPQ.Push(node);
                }
                else {
                    openPassTracker.Add(nextArrPos);
                    continue;
                }

                // nextPos의 부모 경로 설정
                //Debug.Log("set nextPos's Parent");
                _parent[nextArrPos.Y, nextArrPos.X] = new Pos(curNode.Y, curNode.X);
            }
        }
            return CurFromDestPath(_parent, destArrPos, curArrPos);
    }
        
    // destPos까지 이동 예상 비용
    int calH(Pos curPos, Pos destPos) {
        // temp
        int yDistance = Abs(curPos.Y, destPos.Y);
        int xDistance = Abs(curPos.X, destPos.X);
        int xyGap = Abs(yDistance, xDistance);

        return xyGap * _strCost + Mathf.Min(yDistance, xDistance) * _digCost;
    }

    Pos CheckCollisionDest(Pos destPos) {
        if (CheckCollision(destPos)) {
            return destPos;
        }
        
        Pos replacePos = destPos;


        for (int i = 0; i < _deltaY.Length; ++i) {
            replacePos = new Pos(destPos.Y + _deltaY[i], destPos.X + _deltaX[i]);

            if (CheckCollision(replacePos)) {
                break;
            }
        }
        return replacePos;
        
    }

    // destCellPos 주변의 F값이 최소인 cell 찾아 역추적 - DestCellPos까지 못 가기 때문
    Stack<Vector3> CurFromDestPath(Pos[,] parent, Pos curPos, Pos startPos) {
        Stack<Vector3> path = new Stack<Vector3>();
        Pos prePos = parent[curPos.Y, curPos.X];

        // DestCellPos로부터 parentCellPos를 역추적하며 pathStack에 저장
        while (prePos != startPos) { 
            // ArrayPos를 WorldPos로 변환
            Vector3Int preCellPos = new Vector3Int(prePos.X + MinX, prePos.Y + MinY, 0);
            Vector3 preWorldPos = Grid.CellToWorld(preCellPos) + _fixedPosition;

            // 변환된 worldPos를 path stack에 저장
            path.Push(preWorldPos);

            prePos = parent[prePos.Y, prePos.X];
        }
        

        return path;
    }


    public Pos CellToArrPos(Vector3Int cellPos) {
        Pos arrPos = new Pos();
        arrPos.X = cellPos.x - MinX;
        arrPos.Y = cellPos.y - MinY;

        return arrPos;
    }

    public Vector3Int ArrToCellPos(Pos arrPos) {
        Vector3Int cellPos = new Vector3Int(arrPos.X + MinX, arrPos.Y + MinY, 0);

        return cellPos;
    }

    // 두 정수 사이의 크기 차이
    int Abs(int a, int b) {
        int cal = a - b;
        if(cal  < 0) {
            return -cal;
        }
        else {
            return cal;
        }
    }
    #endregion
}