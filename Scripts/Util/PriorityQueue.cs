using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T> {
    List<T> _heap = new List<T>();

    public void Push(T data) {
        _heap.Add(data);

        // 우선순위 정렬 - F가 최소인 노드가 _heap[0]에 위치하도록
        int compareIdx = _heap.Count - 1;
        while (compareIdx > 0) {
            int parentIdx = (compareIdx - 1) / 2;
            // 새로 추가된 data가 부모보다 큰 경우 그대로 종료
            if (_heap[compareIdx].CompareTo(_heap[parentIdx]) > 0) {
                break;
            }

            // 부모와 자식의 값 교환
            T temp = data;
            _heap[compareIdx] = _heap[parentIdx];
            _heap[parentIdx] = temp;

            // 루트노드까지 반복하며 비교 진행
            compareIdx = parentIdx;
        }
    }

    public T Pop() {
        // 반환값 저장
        T priorityValue = _heap[0];

        // 최하위 우선순위 노드를 루트노드로
        int parentIdx = 0;
        int lastIdx = _heap.Count - 1;
        int priorityIdx = 0;
        _heap[0] = _heap[lastIdx];
        _heap.RemoveAt(lastIdx);
        --lastIdx;

        // 자녀 노드들의 우선순위를 비교하며 우선순위큐 재정렬
        while (true) {
            int lChildIdx = (2 * parentIdx) + 1;
            int rChildIdx = lChildIdx + 1;

            // rChildIdx가 큐의 크기 이내이면서 R-Child가 L-Child 보다 우선순위가 높은 경우
            if (rChildIdx <= lastIdx && _heap[lChildIdx].CompareTo(_heap[rChildIdx]) > 0) {
                priorityIdx = rChildIdx;


            }
            // lChildIdx가 큐의 크기 이내이면서 L-Child의 우선순위가 R-Child보다 높거나 같은 경우
            else if (lChildIdx <= lastIdx) {
                priorityIdx = lChildIdx;
            }
            // 자녀 노드가 없는 경우
            else {
                break;
            }

            // parent가 lChild & rChild보다 우선순위가 높은 경우
            // 아래와 같은 방식으로 비교할 경우 null참조 발생할 수 있음(rchild가 없는 경우 등)
            //Debug.Log($"parentIdx : {parentIdx}, PQCount : {_heap.Count}");
            if(lChildIdx >= _heap.Count || rChildIdx >= _heap.Count) {
                break;
            }
            if ((_heap[parentIdx].CompareTo(_heap[lChildIdx]) < 0) && (_heap[parentIdx].CompareTo(_heap[rChildIdx]) < 0)) {
                break;
            }

            // 최하위에 있던 노드를 우선순위가 높은 자식 노드와 교환
            T temp = _heap[priorityIdx];
            _heap[priorityIdx] = _heap[parentIdx];
            _heap[parentIdx] = temp;

            parentIdx = priorityIdx;
        }

        // 초기에 저장해둔 반환값 반환
        return priorityValue;
    }

    public int Count { get { return _heap.Count; } }
}
