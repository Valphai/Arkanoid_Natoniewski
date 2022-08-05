using System;
using System.Collections.Generic;
using GameLevel;
using UnityEngine;

namespace PersistantGame
{
    public class BallManager : MonoBehaviour
    {
        private List<Ball> balls;
        private Ball startingBall;
        private Vector2 initOffsetFromVaus = new Vector2(0f, 1f);
        [SerializeField] private Ball ballPrefab;
        [SerializeField] private Vaus vaus;
        public static event Action PlayStart;
        // public static event Action EndGame;
        
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
        public void OnPlayUpdate()
        {
            
        }
        public void StartNewGame()
        {
            if (balls == null) balls = new List<Ball>();

            if (startingBall == null)
                startingBall = Instantiate(ballPrefab);
        }
        private void SpawnBallAt(Vector2 position)
        {
            Ball ball = Instantiate(ballPrefab);
            ball.transform.position = position;
            balls.Add(ball);
        }
    }
}