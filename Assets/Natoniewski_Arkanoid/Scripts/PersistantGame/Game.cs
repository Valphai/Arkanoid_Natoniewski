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
        private GameSaver gameSaver;
        private MenuState menuState = new MenuState();
        private StartState startState = new StartState();
        private PlayState playState = new PlayState();
        private int currentGameLife;
        [SerializeField] private Generator gen;
        [SerializeField] private BallManager ballManager;
        [SerializeField] private Vaus vaus;
        private const int saveVersion = 1;

        private void Awake() => EnterMenuState();
        private void OnEnable()
        {
            gameSaver = new GameSaver();
            BallManager.OnPlayStart += ExitState;
            BallManager.OnLifeLost += LoseGameHealth;
            MenuButtons.OnNewGame += StartNewGame;
            MenuButtons.OnGameSave += SaveGame;
            MenuButtons.OnGameLoad += LoadGame;
        }
        private void OnDisable()
        {
            
            BallManager.OnPlayStart -= ExitState;
            BallManager.OnLifeLost -= LoseGameHealth;
            MenuButtons.OnNewGame -= StartNewGame;
            MenuButtons.OnGameSave -= SaveGame;
            MenuButtons.OnGameLoad -= LoadGame;
        }
        private void Update()
        {
            CurrentState.OnUpdate(this);

            if (Input.GetKeyDown(KeyCode.N))
                StartNewGame();
        }
        public void EnterMenuState() => menuState.OnEnter(this);
        public void EnterStartState() => startState.OnEnter(this);
        public void EnterPlayState() => playState.OnEnter(this);
        public void ExitState() => CurrentState.OnExit(this);
        public void OnStartUpdate()
        {
            ballManager.OnStartUpdate();
            vaus.OnStartUpdate();
        }
        public void OnPlayUpdate()
        {
            ballManager.OnPlayUpdate();
            vaus.OnPlayUpdate();
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
            scoreManager.Save(writer);
        }
        public override void Load(GameDataReader reader)
        {
            int version = reader.SaveVersion;
            currentGameLife = reader.ReadInt();
            vaus.Load(reader);
            gen.Load(reader);
            scoreManager.Load(reader);
        }
        private void SaveGame() => gameSaver.Save(this, saveVersion);
        private void LoadGame()
        {
            StartNewGame();
            gameSaver.Load(this);
        }
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
                ExitState();
                scoreBoard.Activate();
                return;
            }
            RebuildGameLevel();
        }
    }
}
