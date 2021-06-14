using System;
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
                IGameTile[] gameFieldTileData = _gameField.GameFieldTileDataWithBorders();
                
                gameFieldTileData[column + row*_gameFieldPlayData.GameFieldDimensionY].SetTileData(tileIsOccupied);
                #pragma warning disable 168
            } catch (Exception e) { 
                #pragma warning restore 168
                // ignored - this is game field borders
            }
        }

        private void GenerateNewSnakeFood() {
            IGameTile[] gameFieldTileData = _gameField.GameFieldTileDataWithoutBorders();

            int row = Random.Range(0, _gameFieldPlayData.GameFieldDimensionX);
            int column = Random.Range(0, _gameFieldPlayData.GameFieldDimensionY);

            bool tileIsOccupiedBySnakeOrWall = true;
            
            while (tileIsOccupiedBySnakeOrWall) {
                row = Random.Range(0, _gameFieldPlayData.GameFieldDimensionX);
                column = Random.Range(0, _gameFieldPlayData.GameFieldDimensionY);

                tileIsOccupiedBySnakeOrWall = gameFieldTileData[row + column * _gameFieldPlayData.GameFieldDimensionY]
                    .IsTileDataOccupiedBySnakeOrWall();
            }
            
            _gameField.GenerateSnakeFoodOnGameField(row, column);
        }

        private void StartNewGame(GameFieldPlayData gameFieldPlayData) {
            _gameFieldPlayData = gameFieldPlayData;

            if (Camera.main is { }) Camera.main.transform.position = new Vector3(Mathf.RoundToInt(_gameFieldPlayData.GameFieldDimensionX / 2f), 
                Mathf.RoundToInt(_gameFieldPlayData.GameFieldDimensionY / 2f) , -10f);

            CheckGamePlayDataForCorrectValues();
            
            _gameField.GenerateGameField(_gameFieldPlayData.GameFieldDimensionX, _gameFieldPlayData.GameFieldDimensionY);
            IGameTile[] gameFieldTileData = _gameField.GameFieldTileDataWithoutBorders();

            int columnOfSnakePosition = _gameFieldPlayData.SnakeInitialPositionX * _gameFieldPlayData.GameFieldDimensionY;
            
            gameFieldTileData[_gameFieldPlayData.SnakeInitialPositionY     + columnOfSnakePosition].SetTileData(true);
            gameFieldTileData[_gameFieldPlayData.SnakeInitialPositionY - 1 + columnOfSnakePosition].SetTileData(true);
            gameFieldTileData[_gameFieldPlayData.SnakeInitialPositionY - 2 + columnOfSnakePosition].SetTileData(true);

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
