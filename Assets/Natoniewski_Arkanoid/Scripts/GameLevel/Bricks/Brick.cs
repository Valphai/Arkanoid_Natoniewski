using System;
using GameLevel.PowerUps;
using UnityEngine;

namespace GameLevel.Bricks
{
    public class Brick : MonoBehaviour
    {
        public PowerUp PowerUpToSpawn;
        private SpriteRenderer sr;
        [SerializeField] private ParticleSystem hitEffect;
        [SerializeField] private int hp;
        [SerializeField, HideInInspector] private int maxHp;
        [SerializeField] private int brickDestroyScore;
        public static event Action<Brick> OnBrickDestroyed;
        
        private void OnEnable()
        {
            sr = GetComponent<SpriteRenderer>();
        }
        public void SetUp(int hp, Color[] colors, float x, float y)
        {
            SetUp(hp, colors, new Vector2(x, y));
        }
        public void SetUp(int hp, Color[] colors, Vector2 v)
        {
            SetHp(hp, colors);
            transform.position = v;
        }

        public void SetHp(int hp, Color[] colors)
        {
            this.hp = hp;
            this.maxHp = hp;
            sr.color = colors[hp - 1];
        }
        public int GetHp() => hp;
        private void DecreaseHp()
        {
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
            OnBrickDestroyed?.Invoke(this);

            if (PowerUpToSpawn != null)
                Instantiate(PowerUpToSpawn);
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Ball")
            {
                // Ball b = other.gameObject.GetComponent<Ball>();
                DecreaseHp();
            }
        }
    }
}
