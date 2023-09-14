using System.Collections.Generic;
using Interface;
using Service;
using UnityEngine;
using Utils;

namespace Attacks
{
    public class MagicAttack: MonoBehaviour
    {
        [SerializeField] private int magicalDamage = 1;
        private bool canCheckPhysicalCollisions;
        private List<GameObject> _hitImpactList;
        private int _hitImpactControl;
        public bool CanHit;
        
        private void Start()
        {
            _hitImpactList = ServiceLocator.GetService<Impacts>().HitImpactList;
            _hitImpactControl = 0;
            Attack();
        }


        private void Update()
        {
            if (canCheckPhysicalCollisions)
                CheckPhysicalCollisions();
        }

        public void Attack()
        {
            canCheckPhysicalCollisions = true;
        }
        
        private void CheckPhysicalCollisions()
        {
            Collider[] collisions = Physics.OverlapSphere(
                transform.position, 1f, LayerMask.GetMask(Constants.LayerEnemy));

            foreach (Collider collision in collisions)
            {
                IMagicAttack attack = collision.GetComponent<IMagicAttack>();
                if (attack != null)
                {
                    if (CanHit) return;
                    CanHit = true;
                    if (_hitImpactControl == _hitImpactList.Count) _hitImpactControl = 0;
                    attack.ReceiveMagicAtackk(magicalDamage);
                    Transform hitPoint = collision.transform.Find("HitPoint");
                    ParticleSystem hitParticle = Instantiate(_hitImpactList[1], hitPoint).GetComponent<ParticleSystem>();
                    hitParticle.Play();
                    _hitImpactControl++;
                }
            }
        }
    }
}