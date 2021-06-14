using System;
using com.neeksdk.SnakeTest.Snake;
using UnityEngine;

namespace com.neeksdk.SnakeTest.GameMenu {
    [RequireComponent(typeof(Animator))]
    public class MainMenu : MonoBehaviour {
        [SerializeField] private Options options;
        [SerializeField] private Animator mainMenuAnimator;
        private static readonly int HideMainMenu = Animator.StringToHash("HideMainMenu");
        private static readonly int ShowOptions1 = Animator.StringToHash("ShowOptions");
        private static readonly int HideOptions1 = Animator.StringToHash("HideOptions");
        private static readonly int ShowMainMenu = Animator.StringToHash("ShowMainMenu");
        public static event Action<GameManager.GameFieldPlayData> StartNewGame = delegate { };

        private void Start() {
            SnakeHead.SnakeIsDead += ShowMenu;
            ShowMenu();
        }

        private void OnDestroy() {
            SnakeHead.SnakeIsDead -= ShowMenu;
        }

        private void ShowMenu() {
            mainMenuAnimator.SetTrigger(ShowMainMenu);
        }
        
        public void StartNewGameButtonClicked() {
            StartNewGame(options.GameFieldPlayDataFromOptions());
            mainMenuAnimator.SetTrigger(HideMainMenu);
        }

        public void ShowOptions() {
            mainMenuAnimator.SetTrigger(ShowOptions1);
        }

        public void HideOptions() {
            mainMenuAnimator.SetTrigger(HideOptions1);
        }
    }
}
