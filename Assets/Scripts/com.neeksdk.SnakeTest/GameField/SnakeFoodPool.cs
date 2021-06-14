using System.Collections.Generic;
using UnityEngine;

namespace com.neeksdk.SnakeTest {
    public class SnakeFoodPool : MonoBehaviour {
        // ReSharper disable once InconsistentNaming
        private const int AMOUNT_OF_FOOD_ITEMS_ADDED_TO_POOL = 2;
        private readonly Dictionary<string, Queue<ISnakeFood>> _snakeFoodDictionary = new Dictionary<string, Queue<ISnakeFood>>();

        internal ISnakeFood GetSnakeFoodFromPool(SnakeFoodSO foodData) {
            if (_snakeFoodDictionary.Count == 0 || !_snakeFoodDictionary.ContainsKey(foodData.name)) {
                return AddNewQueueToDictionary();
            }

            Queue<ISnakeFood> pool = _snakeFoodDictionary[foodData.name];

            if (pool.Count == 0) {
                AddNewTileToPool(pool);    
            }

            return pool.Dequeue();
            

            ISnakeFood AddNewQueueToDictionary() {
                Queue<ISnakeFood> newFoodQueue = new Queue<ISnakeFood>();
                AddNewTileToPool(newFoodQueue);
                
                _snakeFoodDictionary.Add(foodData.name, newFoodQueue);

                return newFoodQueue.Dequeue();
            }

            void AddNewTileToPool(Queue<ISnakeFood> snakeFoodPool, int foodCountAddedToPool = AMOUNT_OF_FOOD_ITEMS_ADDED_TO_POOL) {
                for (int i = 0; i < foodCountAddedToPool; i++) {
                    GameObject newSnakeFood = Instantiate(
                        foodData.foodPrefab,
                        transform,
                        false
                    );
                    
                    newSnakeFood.SetActive(false);

                    ISnakeFood snakeFoodInterface = newSnakeFood.GetComponent<BaseSnakeFood>();

                    snakeFoodPool.Enqueue(snakeFoodInterface);
                }
            }
        }

        internal void ReturnSnakeFoodToPool(ISnakeFood foodTile) {
            if (_snakeFoodDictionary.ContainsKey(foodTile.GetSnakeFoodName())) {
                foodTile.GetGameObject().SetActive(false);
                _snakeFoodDictionary[foodTile.GetSnakeFoodName()].Enqueue(foodTile);
            } else {
                Destroy(foodTile.GetGameObject());
            }
        }
    }
}
