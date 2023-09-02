using Interface;
using UnityEngine;
using Utils;

namespace Attacks
{
    public class SwordAttack : MonoBehaviour
    {
        [SerializeField] private int physicalDamage = 1;
        private bool canCheckPhysicalCollisions = false;

        private void Update()
        {
            if (canCheckPhysicalCollisions)
                CheckPhysicalCollisions();
        }

        public void Attack()
        {
            canCheckPhysicalCollisions = true;
        }

        public void ResetPhysicalAttackCollisions()
        {
            canCheckPhysicalCollisions = false;
        }

        private void CheckPhysicalCollisions()
        {
            Collider[] collisions = Physics.OverlapSphere(
                transform.position, 1f, LayerMask.GetMask(Constants.LayerEnemy));

            foreach (Collider collision in collisions)
                collision.GetComponent<IPunchable>()?.Punch(physicalDamage);
        }
    }
}
