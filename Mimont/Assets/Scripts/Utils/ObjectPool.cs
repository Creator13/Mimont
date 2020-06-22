using System.Collections.Generic;
using UnityEngine;

namespace Utils {
public class ObjectPool<T> : MonoBehaviour where T : Component {
    [SerializeField] private T prefab;
    [SerializeField] private int initialSize;
    [SerializeField] private bool canExpand;
    
    private List<T> freeList;
    private List<T> usedList;

    private void Awake() {
        freeList = new List<T>(initialSize);
        usedList = new List<T>(initialSize);

        for (var i = 0; i < initialSize; i++) {
            Spawn();
        }
    }

    public T Get() {
        var free = freeList.Count;
        if (free == 0) {
            if (canExpand) {
                Spawn();
                free++;
            }
        }

        var obj = freeList[free - 1];
        freeList.RemoveAt(free - 1);
        usedList.Add(obj);
        return obj;
    }

    public void ReturnObject(T obj) {
        Debug.Assert(usedList.Contains(obj));

        usedList.Remove(obj);
        freeList.Add(obj);

        var objTransform = obj.transform;
        objTransform.parent = transform;
        objTransform.localPosition = Vector3.zero;
        obj.gameObject.SetActive(false);
    }

    private void Spawn() {
        var obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);
        freeList.Add(obj);
    }
}
}
