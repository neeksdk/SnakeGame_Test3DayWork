using UnityEngine;

namespace com.neeksdk.SnakeTest {
    public interface ISnakeFood {
        GameObject GetGameObject();
        string GetSnakeFoodName();
        void SetupSnakeFood(string foodName);

        void IsEatenBySnake();
    }
}
