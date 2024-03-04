using Fusion;
using UnityEngine;
using System.Collections;

public class Health : NetworkBehaviour
{
    private float cooldownTime = 1f;

    [SerializeField] NumberField HealthDisplay;
    private Animator animator;

    [Networked(OnChanged = nameof(NetworkedHealthChanged))]
    public int NetworkedHealth { get; set; } = 100;
    private bool isDead = false;

    public override void Spawned()
    {
        base.Spawned();
        animator = GetComponent<Animator>();
    }

    private static void NetworkedHealthChanged(Changed<Health> changed)
    {
        Debug.Log($"Health changed to: {changed.Behaviour.NetworkedHealth}");
        changed.Behaviour.HealthDisplay.SetNumber(changed.Behaviour.NetworkedHealth);

        if (changed.Behaviour.NetworkedHealth <= 0 && !changed.Behaviour.isDead)
        {
            changed.Behaviour.isDead = true;
            changed.Behaviour.animator.SetBool("isDead", true);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(int damage)
    {
        Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
        NetworkedHealth -= damage;

        // Play hit animation only on the client where the damage is dealt
        if (HasStateAuthority && !isDead)
        {
            StartCoroutine(HitAnimation());
        }
    }

    private IEnumerator HitAnimation()
    {
        Debug.Log("Starting hit Animation");
        animator.SetBool("isHit", true);
        yield return new WaitForSeconds(cooldownTime);
        animator.SetBool("isHit", false);
        Debug.Log("Ending hit Animation");
    }
}
