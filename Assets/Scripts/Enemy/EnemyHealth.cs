using Interface;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPunchable, IMagicAttack
{
    [SerializeField] private int _health;

    private void Damage(int damage)
    {
        _health -= damage;
        
        if(_health <= 0f) Destroy(gameObject);
    }

    public void Punch(int damage)
    {
        Damage(damage);
    }

    public void ReceiveMagicAtackk(int damage)
    {
        Damage(damage);
    }
}

