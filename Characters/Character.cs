using UnityEngine;
using Bombs.Core;

namespace Bombs.Characters {
    public class Character : Recyclable {
        [SerializeField] int maxHealth = 100;

        int health;
        TextMesh healthDisplay;

        /*
            Тут я думал, что можно обойтись использованием onRecycle. 
            Но потом понял, что в дальнейшем, при развитии игры, смерть будет включать в себя ещё дополнительные функции помимо утилизации в Pool.
            Поэтому для неё стоит выделить отдельный event
        */
        public delegate void DeathHandler(Character character);
        public DeathHandler onDeath;
        
        public static string TAG = "Character";

        void OnEnable() {
            healthDisplay = GetComponentInChildren<TextMesh>();

            Health = maxHealth;
        }

        public void TakeDamage(int amount) {
            if (amount == 0) {
                return;
            }
            Health = Mathf.Max(0, Health - amount);
            if (health == 0) {
                Death();
            }
        }

        int Health {
            get {
                return health;
            }
            set {
                health = value;
                healthDisplay.text = value.ToString();
            }
        }

        public Vector3 GetLeftPointByViewDirection(Vector3 inDirection) {
            Vector3 leftDirection = Vector3.Normalize(new Vector3(-inDirection.z, 0f, inDirection.x));
            Vector3 leftPoint = transform.position + leftDirection * transform.localScale.x / 2f;
            return leftPoint;
        }

        public Vector3 GetRightPointByViewDirection(Vector3 inDirection) {
            Vector3 rightDirection = Vector3.Normalize(new Vector3(inDirection.z, 0f, -inDirection.x));
            Vector3 rightPoint = transform.position + rightDirection * transform.localScale.x / 2f;
            return rightPoint;
        }

        void Death() {
            if (onDeath != null) {
                onDeath(this);
                onDeath = null;
            } else {
                Debug.Log("No subscribers onDeath");
            }
            Recycle(this);
        }
    }
}
