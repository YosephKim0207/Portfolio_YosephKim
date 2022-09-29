using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager {

    class Pool {
        Stack _poolStack = new Stack();

        // initNum만큼 Pool에 child Push하는 초기화 진행
        public void Init(GameObject originObj, GameObject parentObj, int initNum = 5) {
            for (int i = 0; i < initNum; ++i) {
                GameObject childObj = Object.Instantiate(originObj, parentObj.transform);
                childObj.name = $"{originObj.name}";
                childObj.SetActive(false);
                _poolStack.Push(childObj);
            }
        }

        public void Push(GameObject originObj, GameObject parentObj = null) {
            originObj.SetActive(false);
            _poolStack.Push(originObj);
        }


        public GameObject Pop(Vector3 pos, Quaternion rot) {
            GameObject childObj = (GameObject)_poolStack.Pop();

            childObj.transform.position = pos;
            childObj.transform.rotation = rot;

            childObj.SetActive(true);
            return childObj;
        }


        public GameObject Pop(Vector3 pos, Quaternion rot, Vector3 shootDir) {
            GameObject childObj = (GameObject)_poolStack.Pop();

            childObj.transform.position = pos;
            childObj.transform.rotation = rot;
            
            BulletController bullet = childObj.GetComponent<BulletController>();
           
            bullet.DestPos = shootDir;

            childObj.SetActive(true);
            return childObj;
        }

        public GameObject Pop(Vector3 pos, Quaternion rot, Vector3 shootDir, CreatureController creatureCS) {
            GameObject childObj = (GameObject)_poolStack.Pop();

            childObj.transform.position = pos;
            childObj.transform.rotation = rot;

            BulletController bullet = childObj.GetComponent<BulletController>();
            bullet.DestPos = shootDir;
            bullet.creature = creatureCS;

            childObj.SetActive(true);
            return childObj;
        }

        public int Count() {
            return _poolStack.Count;
        }
    }

    // Obj당 Pool들 관리
    Dictionary<string, Pool> _poolDic = new Dictionary<string, Pool>();    
    GameObject _root;

    public void Init() {
        _root = GameObject.Find("@RootPool");

        if(_root == null) {
            _root = new GameObject("@RootPool");
        }
    }


    public GameObject UsePool(GameObject originObj) { 
        Transform poolObj = _root.transform.Find(originObj.name);
        if (poolObj == null) {
            // Pool들의 root 하위로 originObj에 대한 pool 생성
            poolObj = InitPoolObj(originObj, _root.transform).transform;

            // return PopPoolChild(originObj, poolObj);
            return PopPoolChild(originObj, poolObj.gameObject, Vector3.zero, Quaternion.identity);
        }
        else {
            // return PopPoolChild(originObj, poolObj);
            return PopPoolChild(originObj, poolObj.gameObject, Vector3.zero, Quaternion.identity);
        }
    }
    public GameObject UsePool(GameObject originObj, Vector3 pos, Quaternion rot, Vector3 shootDir) {
        //GameObject poolObj = GameObject.Find(originObj.name);
        Transform poolObj = _root.transform.Find(originObj.name);
        if (poolObj == null) {
            // Pool들의 root 하위로 originObj에 대한 pool 생성
            poolObj = InitPoolObj(originObj, _root.transform).transform;

            // return PopPoolChild(originObj, poolObj);
            return PopPoolChild(originObj, poolObj.gameObject, pos, rot, shootDir);
        }
        else {
            // return PopPoolChild(originObj, poolObj);
            return PopPoolChild(originObj, poolObj.gameObject, pos, rot, shootDir);
        }
    }


    public GameObject UsePool(CreatureController creatureSC, GameObject originObj, Vector3 pos, Quaternion rot, Vector3 shootDir) {
        Transform poolObj = _root.transform.Find(originObj.name);
        if (poolObj == null) {
            // Pool들의 root 하위로 originObj에 대한 pool 생성
            poolObj = InitPoolObj(originObj, _root.transform).transform;

            return PopPoolChild(creatureSC, originObj, poolObj.gameObject, pos, rot, shootDir);
        }
        else {
            return PopPoolChild(creatureSC, originObj, poolObj.gameObject, pos, rot, shootDir);
        }
    }

    GameObject InitPoolObj(GameObject originObj, Transform parent) {
        // Pooling된 Obj들이 들어갈 부모 Obj 만들기
        GameObject poolObj = new GameObject(name : originObj.name);
        poolObj.transform.parent = parent;

        // 새로 생긴 poolObj 초기화
        Pool pool = new Pool();
        pool.Init(originObj, poolObj);

        // 원본obj이름을 key로 딕셔너리에 Stack add
        _poolDic.Add(originObj.name, pool);

        return poolObj;
    }

    GameObject PopPoolChild(GameObject originObj, GameObject parentObj, Vector3 pos, Quaternion rot) {
        // 해당 오브젝트의 pool이 없는 경우
        if (_poolDic.ContainsKey(originObj.name).Equals(false)) {
            //Debug.Log("PopPoolChild_ContainsKey : True");

            Pool pool = new Pool();
            pool.Init(originObj, parentObj);
            _poolDic.Add(originObj.name, pool);
            return pool.Pop(pos, rot);
        }
        // 해당 오브젝트의 pool이 있는 경우 
        else {
            Pool pool = _poolDic[originObj.name];

            // 비활성화 된 Obj가 남아있는 경우
            if (!pool.Count().Equals(0)) {
                //return pool.Pop();
                return pool.Pop(pos, rot);
            }
            // 비활성화 된 Obj가 남아있지 않은 경우
            else {
                // Obj를 Pool에 추가 생성(Init을 이용) 및 Pop
                pool.Init(originObj, parentObj, 1);
                //return pool.Pop();
                return pool.Pop(pos, rot);
            }
        }
    }

    // Pool에서 Obj pop하여 사용하기 
    GameObject PopPoolChild(CreatureController creatureCS, GameObject originObj, GameObject parentObj, Vector3 pos, Quaternion rot, Vector3 shootDir) {
        // 해당 오브젝트의 pool이 없는 경우
        if (_poolDic.ContainsKey(originObj.name).Equals(false)) {
            //Debug.Log("PopPoolChild_ContainsKey : True");

        Pool pool = new Pool();
        pool.Init(originObj, parentObj);
        _poolDic.Add(originObj.name, pool);
        return pool.Pop(pos, rot, shootDir, creatureCS);


        }
        // 해당 오브젝트의 pool이 있는 경우 
        else {
            Pool pool = _poolDic[originObj.name];

            // 비활성화 된 Obj가 남아있는 경우
            if (!pool.Count().Equals(0)) {
                return pool.Pop(pos, rot, shootDir, creatureCS);
            }
            // 비활성화 된 Obj가 남아있지 않은 경우
            else {
                // Obj를 Pool에 추가 생성(Init을 이용) 및 Pop
                pool.Init(originObj, parentObj, 1);
                return pool.Pop(pos, rot, shootDir, creatureCS);
            }
        }
    }

    // Pool에서 Obj pop하여 사용하기 
    GameObject PopPoolChild(GameObject originObj, GameObject parentObj, Vector3 pos, Quaternion rot, Vector3 shootDir) {
        // 해당 오브젝트의 pool이 없는 경우
        if (_poolDic.ContainsKey(originObj.name).Equals(false)) {
            Debug.Log("PopPoolChild_ContainsKey : True");

            Pool pool = new Pool();
            pool.Init(originObj, parentObj);
            _poolDic.Add(originObj.name, pool);
            return pool.Pop(pos, rot, shootDir);

        }
        // 해당 오브젝트의 pool이 있는 경우 
        else {
            Pool pool = _poolDic[originObj.name];

            // 비활성화 된 Obj가 남아있는 경우
            if (!pool.Count().Equals(0)) {
                return pool.Pop(pos, rot, shootDir);
            }
            // 비활성화 된 Obj가 남아있지 않은 경우
            else {
                // Obj를 Pool에 추가 생성(Init을 이용) 및 Pop
                pool.Init(originObj, parentObj, 1);
                //return pool.Pop();
                return pool.Pop(pos, rot, shootDir);
            }
        }
    }

    // 사용한 오브젝트를 PoolRoot에 반환 
    public void PushPoolChild(GameObject childObj) {
        Pool pool = _poolDic[childObj.name];
        childObj.SetActive(false);
        pool.Push(childObj);
    }

    // Pool 제거
    public void DeletePool(GameObject originObj, Transform parent = null) {
        if(parent == null) {
            GameObject poolObj = GameObject.Find(originObj.name);
            Object.Destroy(poolObj);
            _poolDic.Remove(originObj.name);
        }
        else {
            // TODO
            // parent를 이용하는 경우 추가하기
            Debug.Log("DeletePool Use parent need TODO");
        }
    }

}
