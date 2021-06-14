using System;
using TMPro;
using UnityEngine;

namespace com.neeksdk.SnakeTest.GameMenu {
    public class Options : MonoBehaviour {
        private int _rows = 10, _columns = 6, _snakeSpeed = 1;
        [SerializeField] private TMP_Text rowsValueText, columnsValueText, snakeSpeedText;

        public static event Action<int> OnChangeSnakeSpeed = delegate { }; 

        private void Start() {
            rowsValueText.text = $"{_rows}";
            columnsValueText.text = $"{_columns}";
            snakeSpeedText.text = $"{_snakeSpeed}";
            OnChangeSnakeSpeed(_snakeSpeed);
        }

        public GameManager.GameFieldPlayData GameFieldPlayDataFromOptions() {
            GameManager.GameFieldPlayData newGameFieldData = new GameManager.GameFieldPlayData() {
                GameFieldDimensionX = _rows, 
                GameFieldDimensionY = _columns, 
                SnakeInitialPositionX = 3, 
                SnakeInitialPositionY = Mathf.RoundToInt(_columns / 2f)
            };

            return newGameFieldData;
        }

        #region UI Button methods
        
        public void ChangeRows(int row) {
            int newRows = _rows + row;
            
            if (newRows < 5 || newRows > 20) return;

            _rows = newRows;
            rowsValueText.text = $"{_rows}";
        }
        
        public void ChangeColumns(int column) {
            int newColumns = _columns + column;
            
            if (newColumns < 5 || newColumns > 20) return;

            _columns = newColumns;
            columnsValueText.text = $"{_columns}";
        }
        
        public void ChangeSnakeSpeed(int speed) {
            int newSnakeSpeed = _snakeSpeed + speed;
            
            if (newSnakeSpeed < 1 || newSnakeSpeed > 9) return;

            _snakeSpeed = newSnakeSpeed;
            snakeSpeedText.text = $"{_snakeSpeed}";
            OnChangeSnakeSpeed(_snakeSpeed);
        }
        
        #endregion
    }
}
