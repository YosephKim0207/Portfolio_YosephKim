using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapCollisionMaker {
#if UNITY_EDITOR
    [MenuItem("CustomTools/MapTool %#q")]
    static void MapCollisionMake() {
        // 사전 제작한 map들에 대해 조사
        GameObject[] maps = Resources.LoadAll<GameObject>("Prefabs/Maps");
        Tilemap tmBase, tmCol;

        if(maps == null) {
            return;
        }

        // 리스트에 각 map이 갖는 tilemap 컴포넌트 저장
        List<Tilemap> tilemaps = new List<Tilemap>();
        foreach (GameObject map in maps) {  // 각 지도 프리팹에 대해
            foreach (Tilemap tilemap in map.GetComponentsInChildren<Tilemap>()) {
                tilemap.CompressBounds();
                tilemaps.Add(tilemap);
            }


            tmBase = tilemaps[0];
            //tmObj = tilemaps[1];
            tmCol = tilemaps[2];

            // tileMap 지도 그리기
            // tileMap 크기 정보 가져오기
            using (var writer = File.CreateText($"Assets/Resources/Data/MapCollisionData/{map.name}.txt")) {
                writer.WriteLine(tmBase.cellBounds.xMin);
                writer.WriteLine(tmBase.cellBounds.xMax);
                writer.WriteLine(tmBase.cellBounds.yMin);
                writer.WriteLine(tmBase.cellBounds.yMax);

                for (int y = tmBase.cellBounds.yMax; y >= tmBase.cellBounds.yMin; --y) {
                    for(int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; ++x) {
                        TileBase tile = tmCol.GetTile(new Vector3Int(x, y, 0));
                        if(tile == null) {
                            writer.Write("0");
                        }
                        else {
                            writer.Write("1");
                        }
                    }
                    writer.WriteLine();
                }
            }
        }
    }
#endif
}
