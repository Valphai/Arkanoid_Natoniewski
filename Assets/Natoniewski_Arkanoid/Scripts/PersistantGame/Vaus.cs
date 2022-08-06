using System;
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
        [SerializeField] private Sprite vausSprite;
        [SerializeField] private Sprite vausLaserSprite;
        [SerializeField] private Laser laserPrefab;
        [SerializeField] private Vector2 laserSpawnOffset;
        [SerializeField] private float moveSpeed = 1f;

        private void OnEnable()
        {
            VausLaser.OnGrabLaserPower += TurnLaser;
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponentInChildren<SpriteRenderer>();
            laserFactory = new Factory<Laser>(
                laserPrefab, LevelData.FactoryName
            );
        }
        private void OnDisable()
        {
            VausLaser.OnGrabLaserPower -= TurnLaser;
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
                    Laser lr = laserFactory.Get();

                    Vector2 vausPos = transform.position;
                    ll.transform.position = vausPos + new Vector2(
                        -laserSpawnOffset.x, laserSpawnOffset.y
                    );
                    lr.transform.position = vausPos + laserSpawnOffset;
                }
            }
        }
        public void StartNewGame()
        {
            hasLaser = false;
            sr.sprite = vausSprite;
            transform.position = Vector2.up * -4.2f;
        }
        private void TurnLaser()
        {
            sr.sprite = vausLaserSprite;
            hasLaser = true;
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
                float dstFromCenter = hitPoint.x - transform.position.x;
                float dir;

                dir = hitPoint.x < transform.position.x ? -1 : 1;
                b.BounceOffVaus(dir, dstFromCenter);
            }
            else if (other.gameObject.tag == "PowerUp")
            {
                PowerUp pU = other.gameObject.GetComponent<PowerUp>();
                pU.Activate();
            }
        }
    }
}
