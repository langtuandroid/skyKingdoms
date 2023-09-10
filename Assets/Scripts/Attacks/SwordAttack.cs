using System;
using System.Collections.Generic;
using Interface;
using Service;
using UnityEngine;
using Utils;

namespace Attacks
{
    public class SwordAttack : MonoBehaviour
    {
        [SerializeField] private int physicalDamage = 1;
        [SerializeField] private GameObject _trails;
        
        private bool canCheckPhysicalCollisions;
        private List<GameObject> _hitImpactList;
        private int _hitImpactControl;
        private bool _canHit;

        private void Start()
        {
            _hitImpactList = ServiceLocator.GetService<Impacts>().HitImpactList;
            _hitImpactControl = 0;
        }


        private void Update()
        {
            if (canCheckPhysicalCollisions)
                CheckPhysicalCollisions();
            
            _trails.SetActive(canCheckPhysicalCollisions);
        }

        public void Attack()
        {
            canCheckPhysicalCollisions = true;
        }

        public void ResetPhysicalAttackCollisions()
        {
            canCheckPhysicalCollisions = false;
            _canHit = false;
        }

        private void CheckPhysicalCollisions()
        {
            Collider[] collisions = Physics.OverlapSphere(
                transform.position, 1f, LayerMask.GetMask(Constants.LayerEnemy));

            foreach (Collider collision in collisions)
            {
                IPunchable punchable = collision.GetComponent<IPunchable>();
                if (punchable != null)
                {
                    if (_canHit) return;
                    _canHit = true;
                    if (_hitImpactControl == _hitImpactList.Count) _hitImpactControl = 0;
                    punchable.Punch(physicalDamage);
                    Transform hitPoint = collision.transform.Find("HitPoint");
                    ParticleSystem hitParticle = Instantiate(_hitImpactList[1], hitPoint).GetComponent<ParticleSystem>();
                    hitParticle.Play();
                    Destroy(hitParticle.gameObject, hitParticle.time);
                    _hitImpactControl++;
                }
            }
        }
    }
}
