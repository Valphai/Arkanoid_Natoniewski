namespace PersistantGame.States
{
    public class StartState : IState
    {
        public void OnEnter(Game game)
        {
            game.CurrentState = this;
        }

        public void OnExit(Game game)
        {
        }

        public void OnUpdate(Game game)
        {
            game.OnStartUpdate();
        }
    }
}