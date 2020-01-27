using System.Collections.Generic;
using UnityEngine;
using Bombs.Characters;
using Bombs.Level;

namespace Bombs.Bomb {
    public class ExplosionManager : MonoBehaviour {
        [SerializeField] BlockableObjectSpawner bombSpawner;
        [SerializeField] BlockableObjectSpawner characterSpawner;

        HashSet<Character> aliveCharacters;

        void Start() {
            aliveCharacters = new HashSet<Character>();

            bombSpawner.onSpawn += OnBombSpawn;
            characterSpawner.onSpawn += OnCharacterSpawn;
        }

        void OnBombSpawn(GameObject bombObj) {
            Bomb bomb = bombObj.GetComponent<Bomb>();
            if (bomb == null) {
                Debug.Log(string.Format("Explosion handler: empty bomb component"));
            } else {
                bomb.onExplosion += OnBombExplosion;
            }
        }

        void OnBombExplosion(Bomb bomb) {
            Vector3 bombPosition = bomb.transform.position;

            HashSet<Character> characters = new HashSet<Character>(aliveCharacters);

            foreach (Character character in characters) {
                character.TakeDamage(GetExplosionDamage(character, bomb));
            }
        }

        void OnCharacterSpawn(GameObject characterObj) {
            Character character = characterObj.GetComponent<Character>();
            if (character == null) {
                Debug.Log(string.Format("Explosion handler: empty character component"));
            } else {
                character.onDeath += RemoveDeadCharacters;
                aliveCharacters.Add(character);
            }
        }

        void RemoveDeadCharacters(Character character) {
            aliveCharacters.Remove(character);
        }

        int GetExplosionDamage(Character character, Bomb bomb) {
            Vector3 bombPosition = bomb.transform.position;
            if (Vector3.Distance(character.transform.position, bombPosition) > bomb.ExplosionRadius) {
                return 0;
            }

            Vector3 characterDirection = character.transform.position - bombPosition;

            Vector3 leftEdgePoint = character.GetLeftPointByViewDirection(characterDirection);
            Vector3 rightEdgePoint = character.GetRightPointByViewDirection(characterDirection);

            Vector3 leftEdgeDirection = leftEdgePoint - bombPosition;
            Vector3 rightEdgeDirection = rightEdgePoint - bombPosition;

            bool leftEdgeHit = CheckDirectionHit(bombPosition, leftEdgeDirection);
            bool rightEdgeHit = CheckDirectionHit(bombPosition, rightEdgeDirection);

            if (leftEdgeHit && rightEdgeHit) {
                return bomb.MaxExplosionDamage;
            } 
            else if (leftEdgeHit || rightEdgeHit) {
                return bomb.MaxExplosionDamage / 2;
            }
            else {
                return 0;
            }
        }

        bool CheckDirectionHit(Vector3 bombPosition, Vector3 direction) {
            RaycastHit hit;
            bool isHit = Physics.Raycast(bombPosition, direction, out hit);
            if (isHit) {
                return hit.transform.tag != Wall.TAG;
            }
            return true;
        }
    }
}