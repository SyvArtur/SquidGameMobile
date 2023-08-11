using UnityEngine;

namespace Mirror.Examples.Pong
{
    public class Player : NetworkBehaviour
    {
        public float speed = 30;
        public Transform rigidbody2d;

        // need to use FixedUpdate for rigidbody
        void FixedUpdate()
        {
            // only let the local player control the racket.
            // don't control other player's rackets
            if (isLocalPlayer)
                rigidbody2d.position = new Vector2(rigidbody2d.position.x, rigidbody2d.position.y + Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
        }
    }
}
