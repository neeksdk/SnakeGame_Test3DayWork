using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace com.neeksdk.SnakeTest.Snake {
    [RequireComponent(typeof(Animator))]
    public class SnakePart : MonoBehaviour {
        [SerializeField] private SnakePart snakePartThatIFollow, snakePartThatFollowingMe;
        [SerializeField] private string snakeBodyPartName;
        [SerializeField] private SnakeBodyPartsEnum snakeBodyPart;
        
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const float SNAKE_MOVEMENT_SPEED = 10f;
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const float DELAY_BETWEEN_ANIMATIONS = 0.025f;
        
        private Vector3 _positionOfFollowingSnakePart;
        private Vector3 _rotationOfCurrentSnakePart;
        
        [SerializeField] private Animator snakeAnimator;
        private static readonly int SnakeDead = Animator.StringToHash("SnakeDead");
        private static readonly int SnakeStartPlaying = Animator.StringToHash("StartPlay");
        
        public static event Action<int, int, bool> OnSnakeBodyMovedToAnotherTile = delegate { }; 

        private void Start() {
            _positionOfFollowingSnakePart = transform.position;
            // ReSharper disable once Unity.InefficientPropertyAccess
            _rotationOfCurrentSnakePart = transform.eulerAngles;
        }

        private void Update() {
            transform.position = Vector3.Lerp(transform.position, _positionOfFollowingSnakePart, Time.deltaTime * SNAKE_MOVEMENT_SPEED);
            
            if (snakePartThatIFollow == null) return;
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, _rotationOfCurrentSnakePart,
                Time.deltaTime * 100f);
        }

        public void SetFollowingPosition(Vector3 newFollowingPosition, Vector3 newFollowingRotation, float waitTime = 0.0f) {
            StartCoroutine(ChangeFollowingPosition(newFollowingPosition,  newFollowingRotation, waitTime));
        }

        private IEnumerator ChangeFollowingPosition(Vector3 newFollowingPosition, Vector3 newFollowingRotation, float waitTime) {
            yield return new WaitForSeconds(waitTime);
            
            waitTime += DELAY_BETWEEN_ANIMATIONS;
            
            if (snakePartThatFollowingMe != null) {
                snakePartThatFollowingMe.SetFollowingPosition(_positionOfFollowingSnakePart, _rotationOfCurrentSnakePart, waitTime);
            }

            OnSnakeBodyMovedToAnotherTile(
                (int) _positionOfFollowingSnakePart.x, 
                (int) _positionOfFollowingSnakePart.y,
                false);
            
            OnSnakeBodyMovedToAnotherTile(
                (int) newFollowingPosition.x, 
                (int) newFollowingPosition.y,
                true);

            _positionOfFollowingSnakePart = newFollowingPosition;
            _rotationOfCurrentSnakePart = newFollowingRotation;

            yield return null;
        }

        public string GetSnakeBodyPartName() {
            return snakeBodyPartName;
        }

        public void SetNewFollowingTarget(SnakePart newTarget) {
            snakePartThatIFollow = newTarget;
        }

        public void SetNewTargetThatFollowsMe(SnakePart newTarget) {
            snakePartThatFollowingMe = newTarget;
        }

        public SnakePart GetSnakePartThatFollowingMe() {
            return snakePartThatFollowingMe;
        }

        public void ManuallySetFollowingPartPositionAndRotation(Vector3 position, Vector3 rotation) {
            _positionOfFollowingSnakePart = position;
            _rotationOfCurrentSnakePart = rotation;
        }

        public void PlaySnakeStartAnimation() {
            snakeAnimator.SetTrigger(SnakeStartPlaying);
        }

        public void PlaySnakeDeadAnimation() {
            snakeAnimator.SetTrigger(SnakeDead);

            if (snakePartThatFollowingMe != null) {
                snakePartThatFollowingMe.PlaySnakeDeadAnimation();
            }
        }
        
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public void SetSnakeInitialPosition(int x, int y) {
            transform.position = new Vector3(x, y, 0);
            transform.eulerAngles = new Vector3(0, 0, 90);
            _positionOfFollowingSnakePart = transform.position;
            _rotationOfCurrentSnakePart = transform.eulerAngles;
            
            if (snakePartThatFollowingMe != null) {
                snakePartThatFollowingMe.SetSnakeInitialPosition(x - 1, y);
            }
        }
    }
}
