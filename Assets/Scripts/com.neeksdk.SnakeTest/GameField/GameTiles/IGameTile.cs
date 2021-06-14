namespace com.neeksdk.SnakeTest {
    public interface IGameTile {
        UnityEngine.GameObject GetGameObject();
        string GetName();
        void SetupGameTile(string tileName);
        void SetTileMapOffset(GameTileBase.TileMapHorizontalOffset tileMapHorizontalOffset, GameTileBase.TileMapVerticalOffset tileMapVerticalOffset);
        void SetTileData(int row, int column, bool isOccupiedBySnakeFoodOrWall);
        void SetTileData(bool isOccupiedBySnakeFoodOrWall);
        bool IsTileDataOccupiedBySnakeOrWall();
    }
}
