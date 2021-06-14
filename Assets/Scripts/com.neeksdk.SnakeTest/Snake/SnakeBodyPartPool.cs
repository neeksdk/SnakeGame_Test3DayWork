using System.Collections.Generic;
using com.neeksdk.SnakeTest.Snake;
using UnityEngine;

namespace com.neeksdk.SnakeTest {
    public class SnakeBodyPartPool : MonoBehaviour {
        // ReSharper disable once InconsistentNaming
        private const int AMOUNT_OF_SNAKE_BODY_PARTS_ADDED_TO_POOL = 10;
        private readonly Dictionary<string, Queue<SnakePart>> _snakeBodyPartsDictionary = new Dictionary<string, Queue<SnakePart>>();

        internal SnakePart GetSnakeBodyPartFromPool(string bodyPartName, GameObject snakeBodyPartPrefab) {
            if (_snakeBodyPartsDictionary.Count == 0 || !_snakeBodyPartsDictionary.ContainsKey(bodyPartName)) {
                return AddNewQueueToDictionary();
            }

            Queue<SnakePart> pool = _snakeBodyPartsDictionary[bodyPartName];

            if (pool.Count == 0) {
                AddNewSnakeBodyPartToPool(pool);    
            }

            return pool.Dequeue();
            

            SnakePart AddNewQueueToDictionary() {
                Queue<SnakePart> newBodyPartQueue = new Queue<SnakePart>();
                AddNewSnakeBodyPartToPool(newBodyPartQueue);
                
                _snakeBodyPartsDictionary.Add(bodyPartName, newBodyPartQueue);

                return newBodyPartQueue.Dequeue();
            }

            void AddNewSnakeBodyPartToPool(Queue<SnakePart> snakePartPool, int snakeBodyPartsAddedToPool = AMOUNT_OF_SNAKE_BODY_PARTS_ADDED_TO_POOL) {
                for (int i = 0; i < snakeBodyPartsAddedToPool; i++) {
                    GameObject newTile = Instantiate(
                        snakeBodyPartPrefab,
                        transform,
                        false
                        );
                    
                    newTile.SetActive(false);

                    SnakePart gameTileInterface = newTile.GetComponent<SnakePart>();

                    snakePartPool.Enqueue(gameTileInterface);
                }
            }
        }

        internal void ReturnSnakeBodyPartsToPool(SnakePart snakeBodyPart) {
            if (_snakeBodyPartsDictionary.ContainsKey(snakeBodyPart.GetSnakeBodyPartName())) {
                snakeBodyPart.gameObject.SetActive(false);
                _snakeBodyPartsDictionary[snakeBodyPart.GetSnakeBodyPartName()].Enqueue(snakeBodyPart);
            } else {
                Destroy(snakeBodyPart.gameObject);
            }
        }
    }
}
