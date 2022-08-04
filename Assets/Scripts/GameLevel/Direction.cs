namespace GameLevel
{
    public enum Direction
    {
        E, NE, N, NW, W, SW, S, SE
    }
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction dir) => (int)dir < 4 ? dir + 4 : dir - 4;
        public static Direction[] Fan(this Direction dir)
        {
            int enumCount = 8;
            return new Direction[] {
                
                dir - 2 % enumCount,
                dir - 1 % enumCount,
                dir,
                dir + 1 % enumCount,
                dir + 2 % enumCount
            };
        }
    }
}