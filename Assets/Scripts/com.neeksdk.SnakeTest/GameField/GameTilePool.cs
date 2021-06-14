using System.Collections.Generic;
using UnityEngine;

namespace com.neeksdk.SnakeTest {
    public class GameTilePool : MonoBehaviour {
        // ReSharper disable once InconsistentNaming
        private const int AMOUNT_OF_TILES_ADDED_TO_POOL = 5;
        private readonly Dictionary<string, Queue<IGameTile>> _gameTilesDictionary = new Dictionary<string, Queue<IGameTile>>();

        internal IGameTile GetGameTileFromPool(GameTileSO tileData) {
            if (_gameTilesDictionary.Count == 0 || !_gameTilesDictionary.ContainsKey(tileData.name)) {
                return AddNewQueueToDictionary();
            }

            Queue<IGameTile> pool = _gameTilesDictionary[tileData.name];

            if (pool.Count == 0) {
                AddNewTileToPool(pool);    
            }

            return pool.Dequeue();
            

            IGameTile AddNewQueueToDictionary() {
                Queue<IGameTile> newTileQueue = new Queue<IGameTile>();
                AddNewTileToPool(newTileQueue);
                
                _gameTilesDictionary.Add(tileData.name, newTileQueue);

                return newTileQueue.Dequeue();
            }

            void AddNewTileToPool(Queue<IGameTile> gameTilePool, int tileCountAddedToPool = AMOUNT_OF_TILES_ADDED_TO_POOL) {
                for (int i = 0; i < tileCountAddedToPool; i++) {
                    GameObject newTile = Instantiate(
                        tileData.tilePrefab,
                        transform,
                        false
                        );
                    
                    newTile.SetActive(false);

                    IGameTile gameTileInterface = newTile.GetComponent<GameTileBase>();
                    gameTileInterface.SetupGameTile(tileData.name);
                    
                    gameTilePool.Enqueue(gameTileInterface);
                }
            }
        }

        internal void ReturnGameTileToPool(IGameTile gameTile) {
            if (_gameTilesDictionary.ContainsKey(gameTile.GetName())) {
                gameTile.GetGameObject().SetActive(false);
                _gameTilesDictionary[gameTile.GetName()].Enqueue(gameTile);
            } else {
                Destroy(gameTile.GetGameObject());
            }
        }
    }
}
