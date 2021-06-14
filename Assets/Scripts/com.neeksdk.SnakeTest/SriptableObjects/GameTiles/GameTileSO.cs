using UnityEngine;

namespace com.neeksdk.SnakeTest {
    [CreateAssetMenu(fileName = "New game tile", menuName = "New SO/Game tile")]
    public class GameTileSO : ScriptableObject {
        public new string name;
        public string description;
        public GameObject tilePrefab;
    }
}
