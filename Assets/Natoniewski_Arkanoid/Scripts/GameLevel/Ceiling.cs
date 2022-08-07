using UnityEngine;

namespace GameLevel
{
    public class Ceiling : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Laser")
            {
                Laser l = other.gameObject.GetComponent<Laser>();
                l.KillLaser();
            }
        }
    }
}