using System.Collections.Generic;
using UnityEngine;

namespace Bombs.Core {
    public class Pool : MonoBehaviour
    {
        [SerializeField] Recyclable objectToPool;
        [SerializeField] int startPoolSize;

        List<Recyclable> objects;

        void Start()
        {
            objects = new List<Recyclable>();
            for (int i = 0; i < startPoolSize; ++i) {
                InstantiateNewObject(Vector3.zero, Quaternion.identity);
            }
        }

        public Recyclable Get(Vector3 position, Quaternion rotation) {
            foreach (Recyclable obj in objects) {
                if (!obj.activeSelf) {
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;

                    obj.SetActive(true);
                    return obj;
                }
            }

            Recyclable newObj = InstantiateNewObject(position, rotation);
            newObj.SetActive(true);
            return newObj;
        }

        Recyclable InstantiateNewObject(Vector3 position, Quaternion rotation) {
            Recyclable obj = Instantiate<Recyclable>(objectToPool, position, rotation, transform);
            obj.onRecycle += Recycle;
            obj.gameObject.SetActive(false);

            objects.Add(obj);
            return obj;
        }

        void Recycle(Recyclable recycledObj) {
            recycledObj.SetActive(false);
            
            foreach (Recyclable obj in objects) {
                if (obj == recycledObj) {
                    obj.SetActive(false);
                    return;
                }
            }

            objects.Add(recycledObj);
        }

        public Recyclable ObjectToPool {
            get {
                return objectToPool;
            }
        }

    }

}