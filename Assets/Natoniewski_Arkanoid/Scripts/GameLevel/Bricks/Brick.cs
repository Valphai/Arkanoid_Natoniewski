using System;
using GameLevel.PowerUps;
using UnityEngine;

namespace GameLevel.Bricks
{
    public class Brick : MonoBehaviour
    {
        public PowerUp PowerUpToSpawn;
        public int XIndex, YIndex;
        private SpriteRenderer sr;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private int hp;
        [SerializeField, HideInInspector] private int maxHp;
        [SerializeField] private int brickDestroyScore;
        public static event Action<Brick> OnBrickDestroyed;

        private void OnEnable() => sr = GetComponent<SpriteRenderer>();
        public void SetUp(
            int hp, Color[] colors, Vector2 pos, int xIndex, int yIndex
        )
        {
            SetHp(hp, colors);
            transform.position = pos;
            XIndex = xIndex;
            YIndex = yIndex;
        }

        public void SetHp(int hp, Color[] colors)
        {
            this.hp = hp;
            this.maxHp = hp;
            sr.color = colors[hp - 1];
        }
        private void DecreaseHp()
        {
            if (hp >= 4) return; // non destructible
            
            hp -= 1;
            SpawnParticle();
            if (hp <= 0) Kill();
        }

        private void SpawnParticle()
        {
            ParticleSystem p = Instantiate(
                hitEffect, 
                transform.position + Vector3.forward * .1f, 
                Quaternion.identity
            );

            var mainModule = p.main;
            mainModule.startColor = sr.color;
            Destroy(p.gameObject, mainModule.startLifetime.constant);
        }
        public int GetScore() => brickDestroyScore * maxHp;
        private void Kill()
        {
            if (PowerUpToSpawn != null)
                Instantiate(PowerUpToSpawn, transform.position, 
                    Quaternion.identity
                );

            OnBrickDestroyed?.Invoke(this);
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Ball")
            {
                DecreaseHp();
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Laser")
            {
                DecreaseHp();
                Laser l = other.gameObject.GetComponent<Laser>();
                l.KillLaser();
            }
        }
    }
}
