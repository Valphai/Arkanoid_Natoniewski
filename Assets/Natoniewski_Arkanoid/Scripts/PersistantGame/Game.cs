using GameSave;
using GameUI;
using PersistantGame.States;
using UnityEngine;

namespace PersistantGame
{
    public class Game : IPersistantObject
    {
        public IState CurrentState;
        private ScoreManager scoreManager { get { return ScoreManager.Instance; } }
        private ScoreBoard scoreBoard { get { return ScoreBoard.Instance; } }
        private LifeTracker lifeTracker { get { return LifeTracker.Instance; } }
        private UIInput uIInput { get { return UIInput.Instance; } }
        private GameSaver gameSaver;
        private PauseState pauseState = new PauseState();
        private StartState startState = new StartState();
        private PlayState playState = new PlayState();
        private int currentGameLife;
        [SerializeField] private Generator gen;
        [SerializeField] private BallManager ballManager;
        [SerializeField] private Vaus vaus;
        private const int saveVersion = 1;

        private void Awake() => EnterPauseState();
        private void OnEnable()
        {
            gameSaver = new GameSaver();
            BallManager.OnPlayStart += EnterPlayState;
            BallManager.OnLifeLost += LoseGameHealth;
            UIInput.OnNewGame += StartNewGame;
            UIInput.OnGameSave += SaveGame;
            UIInput.OnGameLoad += LoadGame;
            UIInput.OnResumeGame += ExitCurrentState; // from PauseState
            UIInput.OnPauseGame += EnterPauseState;
        }
        private void OnDisable()
        {
            BallManager.OnPlayStart -= EnterPlayState;
            BallManager.OnLifeLost -= LoseGameHealth;
            UIInput.OnNewGame -= StartNewGame;
            UIInput.OnGameSave -= SaveGame;
            UIInput.OnGameLoad -= LoadGame;
            UIInput.OnResumeGame -= ExitCurrentState; // from PauseState
            UIInput.OnPauseGame -= EnterPauseState;
        }
        private void Update() => CurrentState.OnUpdate(this);
        public void EnterPauseState() => pauseState.OnEnter(this);
        public void EnterStartState() => startState.OnEnter(this);
        public void EnterPlayState() => playState.OnEnter(this);
        public void OnPauseEnter() => ballManager.OnPauseEnter();
        public void OnPauseExit() => ballManager.OnPauseExit();
        public void OnStartUpdate()
        {
            ballManager.OnStartUpdate();
            vaus.OnStartUpdate();
        }
        public void OnPlayUpdate()
        {
            ballManager.OnPlayUpdate();
            vaus.OnPlayUpdate();
            uIInput.OnPlayUpdate();
        }
        public void StartNewGame()
        {
            currentGameLife = 3;
            lifeTracker.UpdateLife(currentGameLife);
            gen.StartNewGame();
            scoreManager.StartNewGame();
            ballManager.StartNewGame();
            vaus.StartNewGame();
            EnterStartState();
        }
        public override void Save(GameDataWriter writer)
        {
            writer.Write(currentGameLife);
            vaus.Save(writer);
            gen.Save(writer);
            ballManager.Save(writer);
            scoreBoard.Save(writer);
            scoreManager.Save(writer);
        }
        public override void Load(GameDataReader reader)
        {
            EnterPlayState();
            int version = reader.SaveVersion;
            currentGameLife = reader.ReadInt();
            lifeTracker.UpdateLife(currentGameLife);
            vaus.Load(reader);
            gen.Load(reader);
            ballManager.Load(reader);
            scoreBoard.Load(reader);
            scoreManager.Load(reader);
        }
        private void ExitCurrentState() => CurrentState.OnExit(this);
        private void SaveGame() => gameSaver.Save(this, saveVersion);
        private void LoadGame() => gameSaver.Load(this);
        private void RebuildGameLevel()
        {
            gen.RebuildGameLevel();
            ballManager.StartNewGame();
            vaus.StartNewGame();
            EnterStartState();
        }
        private void LoseGameHealth()
        {
            currentGameLife--;
            lifeTracker.UpdateLife(currentGameLife);
            if (currentGameLife <= 0)
            {
                EnterPauseState();
                scoreBoard.Activate();
                gen.ResetSeed();
                return;
            }
            RebuildGameLevel();
        }
    }
}
