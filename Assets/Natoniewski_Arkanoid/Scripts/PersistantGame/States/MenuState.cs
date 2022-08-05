namespace PersistantGame.States
{
    public class MenuState : IState
    {
        public void OnEnter(Game game)
        {
            game.CurrentState = this;
        }

        public void OnExit(Game game)
        {
            game.EnterStartState();
        }

        public void OnUpdate(Game game)
        {
        }
    }
}