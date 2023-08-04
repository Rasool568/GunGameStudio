using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int startHealth;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip healSound;
    private int health;

    private void Start()
    {
        health = startHealth;
        healthText.text = health.ToString() + " +";
    }

    public void DamagePlayer(int _amount)
    {
        if(health - _amount <= 0)
        {
            GetComponent<MenuController>().PlayerDied();
            AudioManager.PlaySoundOnPlayer(deathSound);
        }
        else
        {
            health -= _amount;
            AudioManager.PlaySoundOnPlayer(damageSound, 0.6f);
        }
        healthText.text = health.ToString() + " +";
    }

    public void HealPlayer(int _amount)
    {
        if(health + _amount > startHealth)
            health = startHealth;
        else
            health += _amount;
        AudioManager.PlaySoundOnPlayer(healSound);
        healthText.text = health.ToString() + " +";
    }
}
