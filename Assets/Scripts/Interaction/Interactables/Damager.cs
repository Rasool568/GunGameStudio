using UnityEngine;

public class Damager : Interactable
{
    [SerializeField] private int damageAmount;

    protected override void Interact()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().DamagePlayer(damageAmount);
    }
}