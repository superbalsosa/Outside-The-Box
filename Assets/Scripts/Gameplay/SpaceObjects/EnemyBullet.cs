using DependencyInjection;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    ISpaceShipManager spaceShipManager;

    private void Start()
    {
        spaceShipManager = InterfaceDependencyInjector.Instance.Resolve<ISpaceShipManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpaceShip"))
        {
            spaceShipManager.Heal(-damage);

            ObjectPoolManager.ReturnToPool(gameObject);
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
}
