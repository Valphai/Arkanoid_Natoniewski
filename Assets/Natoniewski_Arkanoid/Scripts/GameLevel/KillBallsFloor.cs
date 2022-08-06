using UnityEngine;

namespace GameLevel
{
    public class KillBallsFloor : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Ball")
            {
                Ball b = other.gameObject.GetComponent<Ball>();
                b.Kill();
            }
        }
    }   
}