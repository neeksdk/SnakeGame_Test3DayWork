using System.Collections;
using com.neeksdk.SnakeTest.Snake;
using UnityEngine;
using Random = UnityEngine.Random;

namespace com.neeksdk.SnakeTest {
    [RequireComponent(typeof(GameTilePool), typeof(SnakeFoodPool))]
    public class GameFieldScript : MonoBehaviour {
        [SerializeField] private GameTileSO gameTileScriptableObject;
        [SerializeField] private SnakeFoodSO snakeFoodScriptableObject;
        
        private GameTilePool _tilePool;
        private SnakeFoodPool _foodPool;
        private IGameTile[,] _gameField;
        private ISnakeFood _snakeFoodOnGameField;

        private void Start() {
            _tilePool = GetComponent<GameTilePool>();
            _foodPool = GetComponent<SnakeFoodPool>();
            SnakeHead.SnakeIsDead += DestroyGameField;
        }

        private void OnDestroy() {
            SnakeHead.SnakeIsDead -= DestroyGameField;
        }

        public void GenerateGameField(int row, int col) {
            int rowWithWalls = row + 2;
            int columnWithWalls = col + 2;
            _gameField = new IGameTile[rowWithWalls, columnWithWalls];

            for (int i = 0; i < rowWithWalls; i++) {
                for (int j = 0; j < columnWithWalls; j++) {
                    IGameTile newGameTile = _tilePool.GetGameTileFromPool(gameTileScriptableObject);
                    GameObject tileGameObject = newGameTile.GetGameObject();
                    tileGameObject.transform.position = new Vector3(i, j, 0);
                    //tileGameObject.SetActive(true);
                    StartCoroutine(GameObjectAppearOnGameFieldAfterTime(tileGameObject,
                        Random.Range(0.01f, 0.8f)));
                    tileGameObject.name = $"{newGameTile.GetName()}_{i}_{j}";
                    tileGameObject.tag = "GameField";
                    SetTileMapOffset(ref newGameTile, i, j, rowWithWalls - 1, columnWithWalls - 1);
                    _gameField[i, j] = newGameTile;

                    if (i == 0 || j == 0 || i == rowWithWalls - 1 || j == columnWithWalls - 1) {
                        tileGameObject.tag = "Obstacle";
                    }
                }
            }

            IEnumerator GameObjectAppearOnGameFieldAfterTime(GameObject target, float time) {
                yield return new WaitForSeconds(time);
                target.SetActive(true);

                yield return null;
            }
        }

        private void DestroyGameField() {
            /*for (int i = _gameField.Length - 1; i >= 0; i--) {
                _tilePool.ReturnGameTileToPool(_gameField[i]);
            }*/

            foreach (IGameTile gameTile in _gameField) {
                _tilePool.ReturnGameTileToPool(gameTile);
            }
            
            _foodPool.ReturnSnakeFoodToPool(_snakeFoodOnGameField);
            
            _gameField = null;
            _snakeFoodOnGameField = null;
        }

        public void GenerateSnakeFoodOnGameField(int row, int column) {
            _snakeFoodOnGameField = _foodPool.GetSnakeFoodFromPool(snakeFoodScriptableObject);
            GameObject foodGameObject = _snakeFoodOnGameField.GetGameObject();
            foodGameObject.transform.position = new Vector3(row, column, 0); 
            foodGameObject.SetActive(true);
        }

        public void SnakeFoodHasBeenEaten(ISnakeFood eatenFood) {
            _foodPool.ReturnSnakeFoodToPool(eatenFood);
        }

        public IGameTile[,] GameFieldTileDataWithBorders() {
            return _gameField;
        }

        private static void SetTileMapOffset(ref IGameTile gameTile, in int row, in int column, in int maxRow, in int maxColumn) {
            GameTileBase.TileMapVerticalOffset verticalOffset = GameTileBase.TileMapVerticalOffset.Middle;
            GameTileBase.TileMapHorizontalOffset horizontalOffset = GameTileBase.TileMapHorizontalOffset.Middle;

            bool tileIsOccupiedBySnakeFoodOrWall = false;
            
            if (row == 0) {
                horizontalOffset = GameTileBase.TileMapHorizontalOffset.Left;
                tileIsOccupiedBySnakeFoodOrWall = true;
            }

            if (row == maxRow) {
                horizontalOffset = GameTileBase.TileMapHorizontalOffset.Right;
                tileIsOccupiedBySnakeFoodOrWall = true;
            }

            if (column == 0) {
                verticalOffset = GameTileBase.TileMapVerticalOffset.Bottom;
                tileIsOccupiedBySnakeFoodOrWall = true;
            }

            if (column == maxColumn) {
                verticalOffset = GameTileBase.TileMapVerticalOffset.Top;
                tileIsOccupiedBySnakeFoodOrWall = true;
            }
            
            gameTile.SetTileMapOffset(horizontalOffset, verticalOffset);
            gameTile.SetTileData(row, column, tileIsOccupiedBySnakeFoodOrWall);
        }
    }
}
