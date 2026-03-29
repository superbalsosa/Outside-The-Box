using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpaceShip"))
        {
            SpaceShipManager.Instance.ChangeHealth(-damage);

            ObjectPoolManager.ReturnToPool(gameObject);
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
