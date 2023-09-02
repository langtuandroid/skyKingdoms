using UnityEngine;

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
        _rb.velocity = new Vector3(_rb.velocity.x, jumpHeight, _rb.velocity.z);
    }

    public void DoubleJumpAction(float jumpHeight)
    {
        float doubleJumpForce = Mathf.Sqrt(2f * jumpHeight * Physics.gravity.magnitude);
       _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
       _rb.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
       _rb.velocity = new Vector3(_rb.velocity.x, jumpHeight, _rb.velocity.z);
    }
}
