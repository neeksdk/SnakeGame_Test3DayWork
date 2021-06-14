using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using com.neeksdk.SnakeTest.GameMenu;
using UnityEngine;

namespace com.neeksdk.SnakeTest.Snake {
    [RequireComponent(typeof(SnakePart)), RequireComponent(typeof(SnakeBodyPartPool))]
    public class SnakeHead : MonoBehaviour {
        [SerializeField] private GameObject snakeBodyPartPrefabAddedWhenSnakeGrowth;
        [SerializeField] private SnakePart initialSnakeBody, initialSnakeTail;
        private SnakePart _headSnakePart, _newSnakePart, _oldSnakePart;
        private bool _snakeIsDead = true, _needToInitializeNewSnakePart;
        private HeadLookDirections _headLookWhenLastMove, _headLooksNow;
        private readonly List<SnakePart> _arrayOfGeneratedSnakeBodies = new List<SnakePart>();
        private float _snakeSpeed;
        
        public static event Action SnakeIsDead = delegate { }; 

        private enum HeadLookDirections {
            Up,
            Right,
            Down,
            Left
        }

        private void Awake() {
            _headSnakePart = GetComponent<SnakePart>();
            _headLookWhenLastMove = HeadLookDirections.Right;
            _headLooksNow = HeadLookDirections.Right;
            _snakeSpeed = 0.75f;
            Options.OnChangeSnakeSpeed += ChangeSnakeSpeed;
        }
        
        private void OnEnable() {
            _snakeIsDead = true;
        }

        private void OnDestroy() {
            Options.OnChangeSnakeSpeed -= ChangeSnakeSpeed;
        }

        public void StartSnakeMovement() {
            _headSnakePart.PlaySnakeStartAnimation();
            initialSnakeBody.PlaySnakeStartAnimation();
            initialSnakeTail.PlaySnakeStartAnimation();
            Invoke(nameof(StartSnakeMovementAfterPause), 1.25f);
        }

        private void StartSnakeMovementAfterPause() {
            _snakeIsDead = false;
            InvokeRepeating(nameof(MoveHead), 1f, _snakeSpeed);
        }

        public SnakePart HeadSnakePart() {
            return _headSnakePart;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag($"Food")) {
                other.GetComponent<ISnakeFood>().IsEatenBySnake();
                _needToInitializeNewSnakePart = true;
            } else if (other.CompareTag($"Obstacle")) {
                CancelInvoke(nameof(MoveHead));
                _snakeIsDead = true;
                PlaySnakeDeadAnimations();
                Invoke(nameof(SnakeIsDeadCallback), 1.5f);
            }
        }

        private void SnakeIsDeadCallback() {
            SnakeIsDead();

            for (int i = _arrayOfGeneratedSnakeBodies.Count - 1; i >= 0; i--) {
                SnakePart snakePart = _arrayOfGeneratedSnakeBodies[i];
                Destroy(snakePart.gameObject);
            }

            _arrayOfGeneratedSnakeBodies.Clear();
            
            _headSnakePart.SetNewTargetThatFollowsMe(initialSnakeBody);
            initialSnakeBody.SetNewFollowingTarget(_headSnakePart);
            initialSnakeBody.SetNewTargetThatFollowsMe(initialSnakeTail);
            initialSnakeTail.SetNewFollowingTarget(initialSnakeBody);

            _headLooksNow = HeadLookDirections.Right;
        }

        private void PlaySnakeDeadAnimations() {
            _headSnakePart.PlaySnakeDeadAnimation();
            initialSnakeBody.PlaySnakeDeadAnimation();
            initialSnakeTail.PlaySnakeDeadAnimation();

            foreach (SnakePart snakePart in _arrayOfGeneratedSnakeBodies) {
                snakePart.PlaySnakeDeadAnimation();
            }
        }

        private void ChangeSnakeSpeed(int newSpeed) {
            _snakeSpeed = 0.8f - 0.05f * newSpeed;
        }
        
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private SnakePart InitializeNewSnakeBodyPart() {
            GameObject newSnakeBodyPart = Instantiate(snakeBodyPartPrefabAddedWhenSnakeGrowth, transform.parent, false);
            SnakePart snakePart = newSnakeBodyPart.GetComponent<SnakePart>();
            Transform snakePartTransform = snakePart.transform;
            
            snakePartTransform.position = transform.position;
            snakePartTransform.eulerAngles = transform.eulerAngles;
            
            snakePart.ManuallySetFollowingPartPositionAndRotation(transform.position, transform.eulerAngles);
            
            return snakePart;
        }

        private void MoveHead() {
            Vector3 headRotation = transform.rotation.eulerAngles;
            Vector3 headPosition = Vector3.zero;
            switch (_headLooksNow) {
                case HeadLookDirections.Up:
                    headPosition = Vector3.up;
                    _headLookWhenLastMove = HeadLookDirections.Up;
                    break;
                case HeadLookDirections.Right:
                    headPosition = Vector3.right;
                    _headLookWhenLastMove = HeadLookDirections.Right;
                    break;
                case HeadLookDirections.Down:
                    headPosition = Vector3.down;
                    _headLookWhenLastMove = HeadLookDirections.Down;
                    break;
                case HeadLookDirections.Left:
                    headPosition = Vector3.left;
                    _headLookWhenLastMove = HeadLookDirections.Left;
                    break;
                default:
                    break;
            }

            if (_needToInitializeNewSnakePart) {
                _needToInitializeNewSnakePart = false;
                _newSnakePart = InitializeNewSnakeBodyPart();
                _oldSnakePart = _headSnakePart.GetSnakePartThatFollowingMe();
                
                _arrayOfGeneratedSnakeBodies.Add(_newSnakePart);

                _headSnakePart.SetNewTargetThatFollowsMe(null);

                _headSnakePart.SetFollowingPosition(transform.position += headPosition, headRotation);
                
                _newSnakePart.SetNewFollowingTarget(_headSnakePart);
                _headSnakePart.SetNewTargetThatFollowsMe(_newSnakePart);
            } else {
                _headSnakePart.SetFollowingPosition(transform.position += headPosition, headRotation);

                if (_newSnakePart == null) return;

                _newSnakePart.SetNewTargetThatFollowsMe(_oldSnakePart);
                _oldSnakePart.SetNewFollowingTarget(_newSnakePart);
                _newSnakePart = null;
                _oldSnakePart = null;
            }
        }

        private void Update() {
            if (_snakeIsDead) return;

            if (Input.GetKeyDown(KeyCode.UpArrow) && _headLookWhenLastMove != HeadLookDirections.Down) {
                _headLooksNow = HeadLookDirections.Up;
                transform.rotation = Quaternion.Euler(0,0,180);
            } else if (Input.GetKeyDown(KeyCode.DownArrow) && _headLookWhenLastMove != HeadLookDirections.Up) {
                _headLooksNow = HeadLookDirections.Down;
                transform.rotation = Quaternion.Euler(0,0,0);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow) && _headLookWhenLastMove != HeadLookDirections.Right) {
                _headLooksNow = HeadLookDirections.Left;
                transform.rotation = Quaternion.Euler(0,0,270);
            } else if (Input.GetKeyDown(KeyCode.RightArrow) && _headLookWhenLastMove != HeadLookDirections.Left) {
                _headLooksNow = HeadLookDirections.Right;
                transform.rotation = Quaternion.Euler(0,0,90);
            }
        }
    }
}
