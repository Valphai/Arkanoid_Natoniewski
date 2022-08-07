using System;
using GameSave;
using UnityEngine;

namespace GameLevel
{
    public class Laser : IPersistantObject
    {
        public bool Killed { get; set; }
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float moveSpeed;
        public static event Action<Laser> OnLaserHit;
        private void FixedUpdate()
        {
            rb.velocity = Vector2.up * moveSpeed;
        }
        public void KillLaser()
        {
            if (!Killed)
            {
                rb.velocity = Vector2.zero;
                OnLaserHit?.Invoke(this);
                Killed = true;
            }
        }
    }
}