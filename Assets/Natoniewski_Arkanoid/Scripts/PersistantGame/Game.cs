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
        private GameSaver gameSaver;
        private MenuState menuState = new MenuState();
        private StartState startState = new StartState();
        private PlayState playState = new PlayState();
        [SerializeField] private Generator gen;
        [SerializeField] private BallManager ballManager;
        [SerializeField] private Vaus vaus;
        private const int saveVersion = 1;
        private void Awake() => EnterMenuState();
        private void OnEnable()
        {
            gameSaver = new GameSaver();
            BallManager.PlayStart += ExitState;
            BallManager.EndGame += LostTheGame;
            MenuButtons.OnNewGame += StartNewGame;
            MenuButtons.OnGameSave += SaveGame;
            MenuButtons.OnGameLoad += LoadGame;
        }
        private void OnDisable()
        {
            
            BallManager.PlayStart -= ExitState;
            BallManager.EndGame -= LostTheGame;
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
            vaus.transform.position = Vector2.up * -4.2f;
            gen.StartNewGame();
            scoreManager.StartNewGame();
            ballManager.StartNewGame();
            EnterStartState();
        }
        public override void Save(GameDataWriter writer)
        {
            vaus.Save(writer);
            gen.Save(writer);
            scoreManager.Save(writer);
        }
        public override void Load(GameDataReader reader)
        {
            int version = reader.SaveVersion;
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
        private void LostTheGame()
        {
            ExitState();
            scoreBoard.Activate();
        }
    }
}
