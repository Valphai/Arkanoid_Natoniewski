using System;
using UnityEngine;

namespace GameLevel.PowerUps
{
    public abstract class PowerUp : MonoBehaviour, IScorableItem
    {
        private float moveSpeed = 3f;

        public abstract int scoreToGet { get; set; }

        public static event Action<PowerUp> OnPowerUpCollected;

        public virtual void Activate() => OnPowerUpCollected?.Invoke(this);
        public int GetScore() => scoreToGet;

        private void Update()
        {
            transform.position -= Vector3.up * moveSpeed * Time.deltaTime; 
        }
    }
}
