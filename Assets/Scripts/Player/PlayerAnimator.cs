using UnityEngine;
using Utils;

public class PlayerAnimator
{
    private Animator _animator;
    private bool _idle;
    
    public void Init(Animator animator)
    {
        _animator = animator;
    }
    
    public void PlayAnimation(Vector2 movement, bool isJumping, bool isGrounded)
    {
        //Idle
        _animator.SetBool(Constants.AnimatorIdleBattle, false);
        _animator.SetBool(Constants.AnimatorIdleNoWeapon, false);

        //Movement
        _animator.SetBool(Constants.AnimatorWalk, movement.magnitude > 0.1f);
        _animator.SetFloat(Constants.AnimatorSpeed, movement.magnitude);

        //Jump
        _animator.SetBool(Constants.AnimatorJump, isJumping);
        
        //Land
        _animator.SetBool(Constants.AnimatorLand, isGrounded);
    }

    public void PlaySwordAttack(int actualPhysicaAttack)
    {
        if (actualPhysicaAttack == 1)
        {
            _animator.Play(Constants.AnimatorAttack01);
        }

        if (actualPhysicaAttack == 2)
        {
            _animator.Play("Attack02_SwordAndShiled");
        }
        
        if (actualPhysicaAttack == 3)
        {
            _animator.Play("Attack03_SwordAndShiled");
        }
        
        if (actualPhysicaAttack == 4)
        {
            _animator.Play("Attack04_SwordAndShiled");
        }
    }


    public void Jump()
    {
        _animator.Play(Constants.AnimatorPlayJump);
    }
    
    public void DoubleJump()
    {
        _animator.SetTrigger(Constants.AnimatorDoubleJump);
    }

    public void SpecialAttack()
    {
        //_animator.SetTrigger("specialattack");
        _animator.Play("Spell");
    }
}
