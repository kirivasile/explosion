using UnityEngine;
using Bombs.Core;

namespace Bombs.Level {
    [RequireComponent(typeof(Pool))]
    public class BlockableObjectSpawner : MonoBehaviour
    {
        [SerializeField] float minX, maxX;
        [SerializeField] float minY, maxY;
        [SerializeField] float minZ, maxZ;
        [SerializeField] float spawnPeriod;

        [SerializeField] Transform[] blockingObjects;

        public delegate void SpawnHandler(GameObject obj);
        public event SpawnHandler onSpawn;

        Pool objects;
        GameObject spawnObjectData;
        float timer;

        void Start() {
            timer = 0f;

            objects = gameObject.GetComponent<Pool>();
            spawnObjectData = objects.ObjectToPool.gameObject;
        }

        void Update()
        {
            if (timer > spawnPeriod) {
                timer = 0f;
                Spawn();
            }
            timer += Time.deltaTime;
        }

        void Spawn() {
            Vector3 position;
            if (!GetPositionForSpawn(out position)) {
                return;
            }

            GameObject newObj = objects.Get(position, Quaternion.identity).gameObject;
            
            if (onSpawn != null) {
                onSpawn(newObj);
            }          
        }

        bool GetPositionForSpawn(out Vector3 position) {
            position = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                Random.Range(minZ, maxZ)
            );

            TryCorrectPositionByX(ref position);

            if (minX > position.x || position.x > maxX) {
                return false;
            }

            return true; 
        }

        void TryCorrectPositionByX(ref Vector3 position) {
            Vector3 correction;
            if (position.x - minX < maxX - position.x) {
                correction = new Vector3(1f, 0f, 0f);
            } else {
                correction = new Vector3(-1f, 0f, 0f);
            }

            Vector3 objScale = spawnObjectData.transform.localScale;

            while (minX < position.x && position.x < maxX) {
                bool collidesWithAnything = false;

                foreach (Transform block in blockingObjects) {
                    if (CollidesWithBlock(block, position, objScale)) {
                        collidesWithAnything = true;
                    }
                }

                if (collidesWithAnything) {
                    position += correction;
                } else {
                    return;
                }
            }
        }

        bool CollidesWithBlock(Transform block, Vector3 objPosition, Vector3 objScale) {
            return
                block.position.x - block.localScale.x / 2f - objScale.x / 2f < objPosition.x &&
                block.position.x + block.localScale.x / 2f + objScale.x / 2f > objPosition.x &&
                block.position.z - block.localScale.z / 2f - objScale.z / 2f < objPosition.z &&
                block.position.z + block.localScale.z / 2f + objScale.z / 2f > objPosition.z;
        }
    }

}