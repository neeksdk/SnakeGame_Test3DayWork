using UnityEngine;

namespace com.neeksdk.SnakeTest {
    [RequireComponent(typeof(MeshRenderer))]
    public abstract class GameTileBase : MonoBehaviour, IGameTile {
        protected GameTileStructData TileData;
        protected Material Material;
        protected string TileName;

        public enum TileMapVerticalOffset {
            Top = 4,
            Middle = 2,
            Bottom = 1
        }
        
        public enum TileMapHorizontalOffset {
            Left = 1,
            Middle = 2,
            Right = 4
        }

        public GameObject GetGameObject() {
            return gameObject;
        }

        public string GetName() {
            return TileName;
        }

        public void SetupGameTile(string tileName) {
            TileName = tileName;
            Material = GetComponent<MeshRenderer>().material;
        }

        public virtual void SetTileMapOffset(TileMapHorizontalOffset tileMapHorizontalOffset, TileMapVerticalOffset tileMapVerticalOffset) {
            float xOffset = 1 - 0.125f * (int)tileMapHorizontalOffset;
            float yOffset = 1 - 0.125f * (int)tileMapVerticalOffset;

            Material.mainTextureOffset = new Vector2(xOffset, yOffset);
        }

        public virtual void SetTileData(int row, int column, bool isOccupiedBySnakeFoodOrWall) {
            TileData = new GameTileStructData() {
                Row = row,
                Column = column,
                IsOccupiedBySnakeFoodOrWall = isOccupiedBySnakeFoodOrWall
            };
        }

        public virtual void SetTileData(bool isOccupiedBySnakeFoodOrWall) {
            TileData.IsOccupiedBySnakeFoodOrWall = isOccupiedBySnakeFoodOrWall;
            //Material.color = isOccupiedBySnakeFoodOrWall ? Color.red : Color.white;
        }

        public virtual bool IsTileDataOccupiedBySnakeOrWall() {
            return TileData.IsOccupiedBySnakeFoodOrWall;
        }

        public GameTileStructData GetTileData() {
            return TileData;
        }
    }
}
