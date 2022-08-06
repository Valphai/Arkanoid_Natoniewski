using UnityEngine;

namespace GameLevel
{
    public class KillBallsFloor : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            switch (other.gameObject.tag)
            {
                case "Ball":
                {
                    Ball b = other.gameObject.GetComponent<Ball>();
                    b.Kill();
                    break;
                }
                case "PowerUp":
                {
                    Destroy(other.gameObject);
                    break;
                }
            }
        }
    }   
}