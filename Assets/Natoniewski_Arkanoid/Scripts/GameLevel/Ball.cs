using System;
using GameSave;
using UnityEngine;

namespace GameLevel
{
    public class Ball : IPersistantObject
    {
        private Rigidbody2D rb;
        private float splitAngle = 60f;
        [SerializeField] private float moveSpeed;
        private Vector2 previousVelocity;

        public static event Action<Ball> OnBallKill;

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        public void OnStart()
        {
            rb.isKinematic = false;
            rb.AddForce(new Vector2(moveSpeed, moveSpeed));
        }
        public override void Save(GameDataWriter writer)
        {
            base.Save(writer);
            writer.Write(previousVelocity);
        }
        public override void Load(GameDataReader reader)
        {
            rb.isKinematic = false;
            base.Load(reader);
            rb.velocity = reader.ReadVector2();
        }
        public void Kill() => OnBallKill?.Invoke(this);
        public void BounceOffVaus(float dir, float dstFromCenter)
        {
            rb.velocity = Vector2.zero;
            
            Vector2 force = new Vector2(
                dir * Mathf.Abs(dstFromCenter * moveSpeed),
                moveSpeed
            );
            rb.AddForce(force);
        }
        public void ConstV() => rb.velocity = rb.velocity.normalized * moveSpeed;
        public void Split(Ball newBall)
        {
            Vector2 initialDirection = rb.velocity;
            
            Quaternion newRotation = Quaternion.AngleAxis(
                splitAngle, -Vector3.forward
            );
            Quaternion newRotationForNewBall = Quaternion.AngleAxis(
                -splitAngle, -Vector3.forward
            );
            rb.velocity = Vector2.zero;
            newBall.rb.velocity = Vector2.zero;
            newBall.rb.isKinematic = false;

            rb.AddForce(newRotation * initialDirection);
            newBall.rb.AddForce(newRotationForNewBall * initialDirection);
        }

        public void StopMovement()
        {
            previousVelocity = rb.velocity;
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        public void ResumeMovement()
        {
            rb.isKinematic = false;
            rb.velocity = previousVelocity;
        }
    }
}