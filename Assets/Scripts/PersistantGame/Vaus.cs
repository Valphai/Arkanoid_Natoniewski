using GameSave;
using UnityEngine;

namespace PersistantGame
{
    public class Vaus : IPersistantObject
    {
        [SerializeField] private float moveSpeed = 1f;

        private void Update()
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
    }
}
