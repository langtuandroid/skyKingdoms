using System;
using Interface;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPunchable
{
    [SerializeField] private int _health;

    private void Damage(int damage)
    {
        _health += damage;
    }

    public void Punch(int damage)
    {
        Damage(damage);
    }
}

