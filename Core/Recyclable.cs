using UnityEngine;

namespace Bombs.Core {
    public class Recyclable : MonoBehaviour {
        public delegate void RecycleHandler(Recyclable obj);
        public RecycleHandler onRecycle;

        public void SetActive(bool value) {
            gameObject.SetActive(value);
        }

        public bool activeSelf {
            get {
                return gameObject.activeSelf;
            }
        }

        protected void Recycle(Recyclable obj) {
            if (onRecycle != null) {
                onRecycle(obj);
            } else {
                Debug.Log("No subscribers onRecycle");
            }
        }
    }
}