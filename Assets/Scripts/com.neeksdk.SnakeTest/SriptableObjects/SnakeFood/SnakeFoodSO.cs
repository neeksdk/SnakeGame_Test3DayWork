using UnityEngine;

namespace com.neeksdk.SnakeTest {
    
    [CreateAssetMenu(fileName = "New snake food", menuName = "New SO/Snake food")]
    public class SnakeFoodSO : ScriptableObject {
        public new string name;
        public string description;
        public GameObject foodPrefab;
    }
}
