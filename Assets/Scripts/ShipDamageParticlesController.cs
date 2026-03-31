using UnityEngine;

public class ShipDamageParticlesController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpaceShipManager spaceShipManager;
    [SerializeField] private ParticleSystem[] damageParticles;

    [Header("Health Thresholds")]
    [SerializeField] private int[] activationThresholds = new int[] { 80, 60, 40, 20, 0 };

    private int lastHealth = -1;

    private void Start()
    {
        DisableAllParticles();
        RefreshParticles();
    }

    private void Update()
    {
        if (spaceShipManager == null)
            return;

        if (spaceShipManager.CurrentHealth != lastHealth)
        {
            RefreshParticles();
        }
    }

    private void RefreshParticles()
    {
        if (spaceShipManager == null)
            return;

        lastHealth = spaceShipManager.CurrentHealth;

        int count = Mathf.Min(damageParticles.Length, activationThresholds.Length);

        for (int i = 0; i < count; i++)
        {
            if (damageParticles[i] == null)
                continue;

            bool shouldBeActive = lastHealth <= activationThresholds[i];

            if (shouldBeActive)
            {
                if (!damageParticles[i].isPlaying)
                    damageParticles[i].Play();
            }
            else
            {
                if (damageParticles[i].isPlaying)
                    damageParticles[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    private void DisableAllParticles()
    {
        if (damageParticles == null)
            return;

        foreach (var particle in damageParticles)
        {
            if (particle == null)
                continue;

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}