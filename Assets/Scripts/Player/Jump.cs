using UnityEngine;

namespace Player
{
    public class Jump
    {
        private Rigidbody _rb;

        public void Init(Rigidbody rb)
        {
            _rb = rb;
        }

        public void JumpAction(float jumpHeight)
        {
            float jumpForce = Mathf.Sqrt(2f * jumpHeight * Physics.gravity.magnitude);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        public void DoubleJumpAction(float jumpHeight)
        {
            float doubleJumpForce = Mathf.Sqrt(3f * jumpHeight * Physics.gravity.magnitude);
            _rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
        }
    }
}
