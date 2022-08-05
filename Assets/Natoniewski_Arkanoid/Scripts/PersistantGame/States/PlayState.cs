namespace PersistantGame.States
{
    public class PlayState : IState
    {
        public void OnEnter(Game game)
        {
            game.CurrentState = this;
        }

        public void OnExit(Game game)
        {
            game.EnterMenuState();
        }

        public void OnUpdate(Game game)
        {
            game.OnPlayUpdate();
        }
    }
}