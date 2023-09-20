using UnityEngine;

namespace Player
{
    public class Jump
    {
        private CharacterController _characterController;
        private Vector3 _velocity;

        public void Init(CharacterController characterController)
        {
            _characterController = characterController;
        }

        public void JumpAction(float jumpHeight, float gravity)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        public void DoubleJumpAction(float jumpHeight, float gravity)
        {
            _velocity.y = Mathf.Sqrt((jumpHeight * 1.5f) * -3 * gravity);
        }

        public void ApplyGravity(float gravity)
        {
            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }
    }
}
