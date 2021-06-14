using com.neeksdk.SnakeTest.Snake;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace com.neeksdk.SnakeTest.Infrastructure {
    public class LocationInstaller : MonoInstaller {
        public GameObject snakePrefab;
        public GameObject gameFieldPrefab;
        
        public override void InstallBindings() {
            BindSnake();
            BindGameField();
        }

        private void BindSnake() {
            SnakeContainer snakeContainer = Container
                .InstantiatePrefabForComponent<SnakeContainer>(snakePrefab, Vector3.zero, quaternion.identity, null);
            
            snakeContainer.gameObject.SetActive(false);
            
            Container
                .Bind<SnakeContainer>()
                .FromInstance(snakeContainer)
                .AsSingle();
        }

        private void BindGameField() {
            GameFieldScript gameFieldScript = Container
                .InstantiatePrefabForComponent<GameFieldScript>(gameFieldPrefab, Vector3.zero, quaternion.identity, null);

            Container
                .Bind<GameFieldScript>()
                .FromInstance(gameFieldScript)
                .AsSingle();
        }
    }
}
