using GameLevel;
using GameSave;
using GameUI;
using UnityEngine;

namespace PersistantGame
{
    public class Game : IPersistantObject
    {
        private Generator gen { get { return Generator.Instance; } }
        private ScoreManager scoreManager { get { return ScoreManager.Instance; } }
        private GameSaver gameSaver;
        [SerializeField] private Vaus vaus;
        private const int saveVersion = 1;
        
        private void OnEnable()
        {
            gameSaver = new GameSaver();
        }
        private void Update()
        {
            // if (Input.GetKeyDown(SaveKey)) 
            // {
			//     gameSaver.Save(this, saveVersion);
            // }
            // else if (Input.GetKeyDown(LoadKey)) 
            // {
            //     StartNewGame();
            //     gameSaver.Load(this);
            // }
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
        public void StartNewGame()
        {
            vaus.transform.position = Vector2.up * -4.2f;
        }
    }
}
