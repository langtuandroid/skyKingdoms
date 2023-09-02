using DG.Tweening;
using Interface;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPunchable
{
    private void Damage()
    {
        transform.DOScale(Vector3.zero, 1).SetEase(Ease.InBounce).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void Punch(int damage)
    {
        Damage();
    }
}

