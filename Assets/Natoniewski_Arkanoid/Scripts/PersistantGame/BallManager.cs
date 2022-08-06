using System;
using System.Collections.Generic;
using GameLevel;
using UnityEngine;

namespace PersistantGame
{
    public class BallManager : MonoBehaviour
    {
        private List<Ball> activeBalls;
        private Ball startingBall;
        private Vector2 initOffsetFromVaus = new Vector2(0f, 1f);
        private Factory<Ball> ballsFactory;
        [SerializeField] private Ball ballPrefab;
        [SerializeField] private Vaus vaus;
        public static event Action PlayStart;
        public static event Action EndGame;

        private void OnEnable()
        {
            ballsFactory = new Factory<Ball>(
                ballPrefab, LevelData.FactoryName
            );
            Ball.OnBallKill += KillBall;
        }
        private void OnDisable() => Ball.OnBallKill -= KillBall;
        public void OnStartUpdate()
        {
            startingBall.transform.position =
                (Vector2)vaus.transform.position + initOffsetFromVaus;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                startingBall.OnStart();
                PlayStart?.Invoke();
            }
        }
        public void OnPlayUpdate() {}
        public void StartNewGame()
        {
            if (activeBalls == null) activeBalls = new List<Ball>();

            for (int i = 0; i < activeBalls.Count; i++)
            {
                ballsFactory.Return(activeBalls[i]);
            }
            activeBalls.Clear();

            startingBall = 
                GetBallAt((Vector2)vaus.transform.position + initOffsetFromVaus);

        }
        private Ball GetBallAt(Vector2 pos)
        {
            Ball b = ballsFactory.Get();
            b.transform.position = pos;
            activeBalls.Add(b);
            return b;
        }
        private void KillBall(Ball ball)
        {
            activeBalls.Remove(ball);
            ballsFactory.Return(ball);

            if (activeBalls.Count == 0)
            {
                startingBall = null;
                EndGame?.Invoke();
            }
        }
    }
}