namespace PersistantGame.States
{
    public interface IState
    {
        public void OnEnter(Game game);
        public void OnUpdate(Game game);
        public void OnExit(Game game);
    }
}