using GameLevel;
using GameSave;
using UnityEngine;

namespace PersistantGame
{
    public class Vaus : IPersistantObject
    {
        private Rigidbody2D rb;
        [SerializeField] private float moveSpeed = 1f;

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        public void OnStartUpdate()
        {
            MoveVaus();
        }
        public void OnPlayUpdate()
        {
            MoveVaus();
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
        }
    }
}
