using GameSave;
using UnityEngine;

namespace GameLevel
{
    public class Ball : IPersistantObject
    {
        [SerializeField] private float moveSpeed;
        private Rigidbody2D rb;

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        public void OnStart()
        {
            rb.isKinematic = false;
            rb.AddForce(new Vector2(moveSpeed, moveSpeed));
        }

        public void BounceOffVaus(float dir, float dstFromCenter)
        {
            rb.velocity = Vector2.zero;
            
            Vector2 force = new Vector2(
                dir * Mathf.Abs(dstFromCenter * moveSpeed),
                moveSpeed
            );
            rb.AddForce(force);
        }
    }
}