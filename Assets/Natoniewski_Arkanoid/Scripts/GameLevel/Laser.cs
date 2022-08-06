using UnityEngine;

namespace GameLevel
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float moveSpeed;
        private void FixedUpdate()
        {
            rb.velocity = Vector2.up * moveSpeed;
        }
    }
}