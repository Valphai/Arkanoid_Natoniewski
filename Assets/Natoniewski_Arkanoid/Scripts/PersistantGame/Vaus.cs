using System;
using System.Collections.Generic;
using GameLevel;
using GameLevel.PowerUps;
using GameSave;
using UnityEngine;

namespace PersistantGame
{
    public class Vaus : IPersistantObject
    {
        public KeyCode ShootButton = KeyCode.Space;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private bool hasLaser;
        private Factory<Laser> laserFactory;
        private List<Laser> activeLasers;
        [SerializeField] private float spriteOffsetX;
        [SerializeField] private Sprite vausSprite;
        [SerializeField] private Sprite vausLaserSprite;
        [SerializeField] private Laser laserPrefab;
        [SerializeField] private Vector2 laserSpawnOffset;
        [SerializeField] private float moveSpeed = 1f;

        private void OnEnable()
        {
            VausLaser.OnGrabLaserPower += TurnLaser;
            Laser.OnLaserHit += ReturnLaser;
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponentInChildren<SpriteRenderer>();
            laserFactory = new Factory<Laser>(
                laserPrefab, LevelData.FactoryName
            );
        }
        private void OnDisable()
        {
            VausLaser.OnGrabLaserPower -= TurnLaser;
            Laser.OnLaserHit -= ReturnLaser;
        }
        public void OnStartUpdate()
        {
            MoveVaus();
        }
        public void OnPlayUpdate()
        {
            MoveVaus();
            if (hasLaser)
            {
                if (Input.GetKeyDown(ShootButton))
                {
                    Laser ll = laserFactory.Get();
                    ll.Killed = false;
                    Laser lr = laserFactory.Get();
                    lr.Killed = false;

                    Vector2 vausPos = transform.position;
                    ll.transform.position = vausPos + new Vector2(
                        -laserSpawnOffset.x, laserSpawnOffset.y
                    );
                    lr.transform.position = vausPos + laserSpawnOffset;

                    activeLasers.Add(ll);
                    activeLasers.Add(lr);
                }
            }
        }
        public override void Save(GameDataWriter writer)
        {
            base.Save(writer);
            writer.Write(hasLaser ? 1 : 0);

            int count = activeLasers.Count;
            writer.Write(count);
            for (int i = 0; i < count; i++)
            {
                activeLasers[i].Save(writer);
            }
        }
        public override void Load(GameDataReader reader)
        {
            ResetChecks();
            base.Load(reader);
            hasLaser = reader.ReadInt() == 1;
            if (hasLaser)
                TurnLaser();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Laser l = laserFactory.Get();
                l.Killed = false;
                l.Load(reader);
            }
        }
        public void StartNewGame()
        {
            ResetChecks();
            hasLaser = false;
            sr.sprite = vausSprite;
            transform.position = Vector2.up * -4.2f;
        }
        private void ResetChecks()
        {
            rb.velocity = Vector2.zero;
            if (activeLasers == null) activeLasers = new List<Laser>();

            for (int i = 0; i < activeLasers.Count; i++)
            {
                laserFactory.Return(activeLasers[i]);
            }
            activeLasers.Clear();
        }
        private void TurnLaser()
        {
            sr.sprite = vausLaserSprite;
            hasLaser = true;
        }
        private void ReturnLaser(Laser l)
        {
            activeLasers.Remove(l);
            laserFactory.Return(l);
        }
        private void MoveVaus()
        {
            float xDelta = Input.GetAxisRaw("Horizontal");
            if ((xDelta != 0f))
            {
                AdjustPosition(xDelta);
            }
        }
        private void AdjustPosition(float xDelta)
        {
            var delta = new Vector2(xDelta, 0f) * moveSpeed * Time.deltaTime;
            transform.Translate(delta);
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Ball")
            {
                Ball b = other.gameObject.GetComponent<Ball>();
                
                Vector3 hitPoint = other.contacts[0].point;
                float dstFromCenter = hitPoint.x - transform.position.x + spriteOffsetX;
                float dir;

                dir = hitPoint.x < transform.position.x + spriteOffsetX ? -1 : 1;
                b.BounceOffVaus(dir, dstFromCenter);
            }
            rb.velocity = Vector2.zero;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "PowerUp")
            {
                PowerUp pU = other.gameObject.GetComponent<PowerUp>();
                pU.Activate();
            }
        }
    }
}
