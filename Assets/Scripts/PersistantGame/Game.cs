using System;
using GameLevel;
using GameSave;
using GameUI;
using UnityEngine;

namespace PersistantGame
{
    public class Game : IPersistantObject
    {
        private ScoreManager scoreManager { get { return ScoreManager.Instance; } }
        private GameSaver gameSaver;
        [SerializeField] private Vaus vaus;
        [SerializeField] private Generator gen;
        private const int saveVersion = 1;
        
        private void OnEnable()
        {
            gameSaver = new GameSaver();
            MenuButtons.OnNewGame += StartNewGame;
            MenuButtons.OnGameSave += SaveGame;
            MenuButtons.OnGameLoad += LoadGame;
        }
        private void OnDisable()
        {
            MenuButtons.OnNewGame -= StartNewGame;
            MenuButtons.OnGameSave -= SaveGame;
            MenuButtons.OnGameLoad -= LoadGame;
        }
        private void SaveGame() => gameSaver.Save(this, saveVersion);
        private void LoadGame()
        {
            StartNewGame();
            gameSaver.Load(this);
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
            gen.StartNewGame();
            scoreManager.StartNewGame();
        }
    }
}
