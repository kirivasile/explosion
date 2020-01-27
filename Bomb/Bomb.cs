using UnityEngine;
using Bombs.Core;

namespace Bombs.Bomb {
    public class Bomb : Recyclable {
        [SerializeField] float explosionTime;
        [SerializeField] float explosionRadius;
        [SerializeField] int maxExplosionDamage;

        public delegate void ExplosionHadler(Bomb bomb);
        public ExplosionHadler onExplosion;

        float timer;

        void OnEnable() {
            timer = 0f;
        }

        void Update() {
            if (timer > explosionTime) {
                Explosion();
            }
            timer += Time.deltaTime;
        }

        void Explosion() {
            if (onExplosion != null) {
                onExplosion(this);
                onExplosion = null;
            } else {
                Debug.Log("No subscribers onExposion");
            }

            Recycle(this);
        }

        public float ExplosionRadius {
            get {
                return explosionRadius;
            }
        }

        public int MaxExplosionDamage {
            get {
                return maxExplosionDamage;
            }
        }
    }

}