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
        private int _columnsCount;
        private IGameTile[] _gameField;
        private IGameTile[] _gameFieldWithoutBorders;
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
            _columnsCount = columnWithWalls;
            _gameField = new IGameTile[rowWithWalls * columnWithWalls];
            _gameFieldWithoutBorders = new IGameTile[row * col];

            for (int j = 0; j < columnWithWalls; j++) {
                for (int i = 0; i < rowWithWalls; i++) {
                    IGameTile newGameTile = _tilePool.GetGameTileFromPool(gameTileScriptableObject);
                    GameObject tileGameObject = newGameTile.GetGameObject();
                    tileGameObject.transform.position = new Vector3(i, j, 0);
                    //tileGameObject.SetActive(true);
                    StartCoroutine(GameObjectAppearOnGameFieldAfterTime(tileGameObject,
                        Random.Range(0.01f, 0.8f)));
                    tileGameObject.name = $"{newGameTile.GetName()}_{i}_{j}";
                    tileGameObject.tag = "GameField";
                    SetTileMapOffset(ref newGameTile, i, j, rowWithWalls - 1, columnWithWalls - 1);
                    _gameField[j + i * _columnsCount] = newGameTile;

                    if (j > 0 && i > 0 && j < columnWithWalls - 1 && i < rowWithWalls - 1) {
                        _gameFieldWithoutBorders[j - 1 + (i - 1) * (_columnsCount - 2)] = newGameTile;
                    }

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
            for (int i = _gameField.Length - 1; i >= 0; i--) {
                _tilePool.ReturnGameTileToPool(_gameField[i]);
            }
            
            _foodPool.ReturnSnakeFoodToPool(_snakeFoodOnGameField);
            
            _gameField = null;
            _snakeFoodOnGameField = null;
        }

        public void GenerateSnakeFoodOnGameField(int row, int column) {
            _snakeFoodOnGameField = _foodPool.GetSnakeFoodFromPool(snakeFoodScriptableObject);
            GameObject foodGameObject = _snakeFoodOnGameField.GetGameObject();
            
            //Exclude game field borders by increasing values by +1
            foodGameObject.transform.position = new Vector3(row + 1, column + 1, 0); 
            foodGameObject.SetActive(true);
        }

        public void SnakeFoodHasBeenEaten(ISnakeFood eatenFood) {
            _foodPool.ReturnSnakeFoodToPool(eatenFood);
        }

        public IGameTile[] GameFieldTileDataWithoutBorders() {
            return _gameFieldWithoutBorders;
        }

        public IGameTile[] GameFieldTileDataWithBorders() {
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

        private IGameTile GameTile(int row, int column) {
            return _gameField[column + row * _columnsCount];
        }
    }
}
