using com.neeksdk.SnakeTest.GameMenu;
using com.neeksdk.SnakeTest.Snake;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace com.neeksdk.SnakeTest {
    public class GameManager : MonoBehaviour {
        public static GameFieldStructModel[] GameFieldData;
        private SnakeContainer _snakeContainer;
        private GameFieldScript _gameField;
        private GameFieldPlayData _gameFieldPlayData;

        public struct GameFieldPlayData {
            public int GameFieldDimensionX, GameFieldDimensionY;
            public int SnakeInitialPositionX, SnakeInitialPositionY;
        }

        [Inject]
        private void Construct(SnakeContainer snakeContainer, GameFieldScript gameFieldScript) {
            _snakeContainer = snakeContainer;
            _gameField = gameFieldScript;
        }

        private void Start() {
            BaseSnakeFood.OnFoodHasBeenEatenBySnake += SnakeFoodEatenGenerateNewFoodOnGameField;
            SnakePart.OnSnakeBodyMovedToAnotherTile += MarkEmptyAndOccupiedGameFieldTilesWhenSnakeMoves;
            MainMenu.StartNewGame += StartNewGame;
        }

        private void OnDestroy() {
            BaseSnakeFood.OnFoodHasBeenEatenBySnake -= SnakeFoodEatenGenerateNewFoodOnGameField;
            SnakePart.OnSnakeBodyMovedToAnotherTile -= MarkEmptyAndOccupiedGameFieldTilesWhenSnakeMoves;
            MainMenu.StartNewGame -= StartNewGame;
        }

        private void SnakeFoodEatenGenerateNewFoodOnGameField(ISnakeFood snakeFood) {
            _gameField.SnakeFoodHasBeenEaten(snakeFood);
            GenerateNewSnakeFood();
        }

        private void MarkEmptyAndOccupiedGameFieldTilesWhenSnakeMoves(int row, int column, bool tileIsOccupied) {
            try {
                IGameTile[,] gameFieldTileData = _gameField.GameFieldTileDataWithBorders();
                gameFieldTileData[row, column].SetTileData(tileIsOccupied);
                #pragma warning disable 168
            } catch (System.Exception e) { 
                #pragma warning restore 168
                // ignored - this is game field borders
            }
        }

        private void GenerateNewSnakeFood() {
            IGameTile[,] gameFieldTileData = _gameField.GameFieldTileDataWithBorders();

            int row = Random.Range(1, _gameFieldPlayData.GameFieldDimensionX - 1);
            int column = Random.Range(1, _gameFieldPlayData.GameFieldDimensionY - 1);

            bool tileIsOccupiedBySnakeOrWall = true;
            int tryCount = 0;
            
            while (tileIsOccupiedBySnakeOrWall && tryCount < 1000) {
                row = Random.Range(1, _gameFieldPlayData.GameFieldDimensionX - 1);
                column = Random.Range(1, _gameFieldPlayData.GameFieldDimensionY - 1);

                tileIsOccupiedBySnakeOrWall = gameFieldTileData[row, column].IsTileDataOccupiedBySnakeOrWall();
                tryCount++;
            }
            
            _gameField.GenerateSnakeFoodOnGameField(row, column);
        }

        private void StartNewGame(GameFieldPlayData gameFieldPlayData) {
            _gameFieldPlayData = gameFieldPlayData;

            if (Camera.main is { }) Camera.main.transform.position = new Vector3(Mathf.RoundToInt(_gameFieldPlayData.GameFieldDimensionX / 2f), 
                Mathf.RoundToInt(_gameFieldPlayData.GameFieldDimensionY / 2f) , -10f);

            CheckGamePlayDataForCorrectValues();
            
            _gameField.GenerateGameField(_gameFieldPlayData.GameFieldDimensionX, _gameFieldPlayData.GameFieldDimensionY);
            IGameTile[,] gameFieldTileData = _gameField.GameFieldTileDataWithBorders();

            gameFieldTileData[_gameFieldPlayData.SnakeInitialPositionX, _gameFieldPlayData.SnakeInitialPositionY].SetTileData(true);
            gameFieldTileData[_gameFieldPlayData.SnakeInitialPositionX - 1, _gameFieldPlayData.SnakeInitialPositionY].SetTileData(true);
            gameFieldTileData[_gameFieldPlayData.SnakeInitialPositionX - 2, _gameFieldPlayData.SnakeInitialPositionY].SetTileData(true);

            _snakeContainer.SetSnakeInitialPosition(_gameFieldPlayData.SnakeInitialPositionX, _gameFieldPlayData.SnakeInitialPositionY);
            _snakeContainer.gameObject.SetActive(true);
            _snakeContainer.snakeHead.StartSnakeMovement();
            
            GenerateNewSnakeFood();
        }

        private void CheckGamePlayDataForCorrectValues() {
            if (_gameFieldPlayData.GameFieldDimensionX < 5) {
                _gameFieldPlayData.GameFieldDimensionX = 5;
            }

            if (_gameFieldPlayData.GameFieldDimensionY < 5) {
                _gameFieldPlayData.GameFieldDimensionY = 5;
            }

            if (_gameFieldPlayData.SnakeInitialPositionX < 0 || _gameFieldPlayData.SnakeInitialPositionX > _gameFieldPlayData.GameFieldDimensionX) {
                _gameFieldPlayData.SnakeInitialPositionX = 3;
            }

            if (_gameFieldPlayData.SnakeInitialPositionY < 0 || _gameFieldPlayData.SnakeInitialPositionY > _gameFieldPlayData.GameFieldDimensionY) {
                _gameFieldPlayData.SnakeInitialPositionY = 1;
            } 
        }
    }
}
