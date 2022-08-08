namespace GameLevel
{
    public interface IScorableItem
    {
        public int scoreToGet { get; set; }
        public int GetScore();
    }
}