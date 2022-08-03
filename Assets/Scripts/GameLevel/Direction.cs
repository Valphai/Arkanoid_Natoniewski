namespace GameLevel
{
    public enum Direction
    {
        E, NE, N, NW, W, SW, S, SE
    }
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction dir) => (int)dir < 4 ? dir + 4 : dir - 4;
    }
}