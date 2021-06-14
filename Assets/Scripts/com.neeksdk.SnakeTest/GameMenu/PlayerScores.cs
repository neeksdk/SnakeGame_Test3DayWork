using com.neeksdk.SnakeTest.Snake;
using TMPro;
using UnityEngine;

namespace com.neeksdk.SnakeTest.GameMenu {
    [RequireComponent(typeof(Animator))]
    public class PlayerScores : MonoBehaviour {
        [SerializeField] private TMP_Text scoreText;
        private Animator _animator;
        private static int _playerScore;
        private static readonly int ShowScore = Animator.StringToHash("ShowScore");
        private static readonly int HideScore = Animator.StringToHash("HideScore");

        private void Start() {
            _animator = GetComponent<Animator>();
            _playerScore = 0;
            UpdatePlayerScore();
            BaseSnakeFood.OnFoodHasBeenEatenBySnake += SnakeFoodEatenGenerateNewFoodOnGameField;
            MainMenu.StartNewGame += StartNewGame;
            SnakeHead.SnakeIsDead += GameOver;
        }

        private void OnDestroy() {
            BaseSnakeFood.OnFoodHasBeenEatenBySnake -= SnakeFoodEatenGenerateNewFoodOnGameField;
            MainMenu.StartNewGame -= StartNewGame;
        }
        
        private void StartNewGame(GameManager.GameFieldPlayData gameFieldPlayData) {
            _playerScore = 0;
            UpdatePlayerScore();
            _animator.SetTrigger(ShowScore);
        }

        private void GameOver() {
            _animator.SetTrigger(HideScore);
        }
        
        private void SnakeFoodEatenGenerateNewFoodOnGameField(ISnakeFood snakeFood) {
            _playerScore += 1;
            UpdatePlayerScore();
        }

        private void UpdatePlayerScore() {
            scoreText.text = $"Score: {_playerScore:00}";
        }
    }
}
