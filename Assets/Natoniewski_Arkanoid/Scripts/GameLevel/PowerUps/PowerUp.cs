using UnityEngine;

namespace GameLevel.PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        private float moveSpeed = 3f;

        public abstract void Activate();
        private void Update()
        {
            transform.position -= Vector3.up * moveSpeed * Time.deltaTime; 
        }
    }
}
