using System;
using UnityEngine;

namespace com.neeksdk.SnakeTest {
    public class BaseSnakeFood : MonoBehaviour, ISnakeFood {
        [SerializeField] protected string snakeFoodName;
        [SerializeField] protected SnakeFoodTypesE snakeFoodType; 
        
        public static event Action<ISnakeFood> OnFoodHasBeenEatenBySnake = delegate { };

        public GameObject GetGameObject() {
            return gameObject;
        }

        public string GetSnakeFoodName() {
            return snakeFoodName;
        }
        
        public void SetupSnakeFood(string snakeFoodNameToBeSet) {
            snakeFoodName = snakeFoodNameToBeSet;
        }

        public void IsEatenBySnake() {
            OnFoodHasBeenEatenBySnake(this);
        }
    }
}
