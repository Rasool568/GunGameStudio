using UnityEngine;

public class Healer : Interactable
{
    [SerializeField] private int healAmount;

    protected override void Interact()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().HealPlayer(healAmount);
    }
}