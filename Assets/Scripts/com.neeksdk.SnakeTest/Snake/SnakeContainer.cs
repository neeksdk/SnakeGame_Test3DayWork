using UnityEngine;

namespace com.neeksdk.SnakeTest.Snake {
    public class SnakeContainer : MonoBehaviour {
        public SnakeHead snakeHead;

        public void SetSnakeInitialPosition(int row, int column) {
            snakeHead.HeadSnakePart().SetSnakeInitialPosition(row, column);
        }
    }
}
