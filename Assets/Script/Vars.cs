public enum TileType
{
    Normal_1,
    Normal_2,
    Normal_3,
    Normal_4,
    Normal_5,
    Dynamite,
    Bomb,
    None
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Extreme
}

public enum GameState
{
    PLaying,
    Pausing,
    Animating,
    Ended
}

public static class GameConstants
{
    public struct AnimationTime
    {
        public const float Swap = 0.1f;
        public const float SwapWait = 0.1f;
        public const float Drop = 0.1f;
        public const float DropWait = 0.3f;
        public const float Explode = 0.2f;
    }
    public const int PointMultiplier = 100;
    public const int MoveCount = 10;
}