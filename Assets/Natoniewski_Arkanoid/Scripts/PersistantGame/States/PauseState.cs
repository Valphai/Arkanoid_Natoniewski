namespace PersistantGame.States
{
    public class PauseState : IState
    {
        public void OnEnter(Game game)
        {
            game.CurrentState = this;
            // sound effects etc...
            game.OnPauseEnter();
        }

        public void OnExit(Game game)
        {
            // sound effects etc...
            game.OnPauseExit();
            game.EnterPlayState();
        }

        public void OnUpdate(Game game)
        {
        }
    }
}