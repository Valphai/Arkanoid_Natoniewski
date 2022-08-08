using System;
using System.Collections.Generic;
using GameLevel;
using GameLevel.PowerUps;
using GameSave;
using UnityEngine;

namespace PersistantGame
{
    public class BallManager : IPersistantObject
    {
        public int MaxBallCount;
        private List<Ball> activeBalls;
        private Ball startingBall;
        private Vector2 initOffsetFromVaus = new Vector2(0f, 1f);
        private Factory<Ball> ballsFactory;
        private bool keepBallsAtConstVelocity;
        [SerializeField] private Ball ballPrefab;
        [SerializeField] private Vaus vaus;

        public static event Action OnPlayStart;
        public static event Action OnLifeLost;

        private void OnEnable()
        {
            ballsFactory = new Factory<Ball>(
                ballPrefab, LevelData.FactoryName
            );
            Ball.OnBallKill += KillBall;
            BallMultiplier.OnBallsMultiply += MultiplyBalls;
        }
        private void OnDisable()
        {
            Ball.OnBallKill -= KillBall;
            BallMultiplier.OnBallsMultiply -= MultiplyBalls;
        }
        private void FixedUpdate()
        {
            if (keepBallsAtConstVelocity)
            {
                for (int i = 0; i < activeBalls.Count; i++)
                {
                    activeBalls[i].ConstV();
                }
            }
        }
        public override void Save(GameDataWriter writer)
        {
            int count = activeBalls.Count;
            writer.Write(count);
            for (int i = 0; i < count; i++)
            {
                activeBalls[i].Save(writer);
            }
        }
        public override void Load(GameDataReader reader)
        {
            ResetChecks();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Ball instance = GetBall();
                instance.Load(reader);
                activeBalls.Add(instance);
            }
            keepBallsAtConstVelocity = true;
        }
        public void OnStartUpdate()
        {
            startingBall.transform.position =
                (Vector2)vaus.transform.position + initOffsetFromVaus;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                startingBall.OnStart();
                OnPlayStart?.Invoke();
                keepBallsAtConstVelocity = true;
            }
        }
        public void OnPlayUpdate() {}
        public void OnPauseEnter() 
        {
            if (activeBalls == null) return;
            
            keepBallsAtConstVelocity = false;
            for (int i = 0; i < activeBalls.Count; i++)
            {
                activeBalls[i].StopMovement();
            }
        }
        public void OnPauseExit() 
        {
            if (activeBalls == null) return;
            
            keepBallsAtConstVelocity = true;
            for (int i = 0; i < activeBalls.Count; i++)
            {
                activeBalls[i].ResumeMovement();
            }
        }
        public void StartNewGame()
        {
            ResetChecks();

            startingBall = GetBall();
            startingBall.transform.position = 
                (Vector2)vaus.transform.position + initOffsetFromVaus;

            activeBalls.Add(startingBall);
        }
        private void ResetChecks()
        {
            keepBallsAtConstVelocity = false;
            if (activeBalls == null) activeBalls = new List<Ball>();

            for (int i = 0; i < activeBalls.Count; i++)
            {
                ballsFactory.Return(activeBalls[i]);
            }
            activeBalls.Clear();
        }
        private Ball GetBall() => ballsFactory.Get();
        private void KillBall(Ball ball)
        {
            activeBalls.Remove(ball);
            ballsFactory.Return(ball);

            if (activeBalls.Count == 0)
            {
                startingBall = null;
                OnLifeLost?.Invoke();
            }
        }
        private void MultiplyBalls()
        {
            int count = activeBalls.Count;

            if (count >= MaxBallCount) return;

            Ball[] extraBalls = new Ball[count];
            for (int i = 0; i < count; i++)
            {
                Ball newBall = GetBall();
                newBall.transform.position = activeBalls[i].transform.position;

                extraBalls[i] = newBall;
                activeBalls[i].Split(newBall);
            }
            for (int i = 0; i < count; i++)
            {
                activeBalls.Add(extraBalls[i]);
            }
        }
    }
}