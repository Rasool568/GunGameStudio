using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject playerMenu;
    [SerializeField] private GameObject deadMenu;
    [SerializeField] private TextMeshProUGUI audioMuteButtonText;
    [SerializeField] private AudioClip buttonSound;
    private bool pause;
    private PlayerMovement pm;
    private InteractController ic;
    private WeaponController wc;

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();
        ic = GetComponent<InteractController>();
        wc = GetComponent<WeaponController>();
        pause = false;
        pauseMenu.SetActive(false);
        deadMenu.SetActive(false);
        playerMenu.SetActive(true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(pauseKey))
        {
            SetPause();
        }
    }

    public void SetPause()
    {
        pause = !pause;
        Cursor.visible = pause;
        pauseMenu.SetActive(pause);
        playerMenu.SetActive(!pause);
        pm.Busy = pause;
        ic.lockInteracting = pause;
        wc.Busy = pause;

        if (pause)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void AudioToggle()
    {
        if (!AudioManager.AudioMute())
            audioMuteButtonText.text = "Вкл";
        else
            audioMuteButtonText.text = "Выкл";
        PlaySound();
    }

    public void ExitGame()
    {
        Application.Quit();
        PlaySound();
    }

    public void PlayerDied()
    {
        pm.Busy = true;
        ic.lockInteracting = true;
        wc.Busy = true;
        playerMenu.SetActive(false);
        pauseMenu.SetActive(false);
        deadMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlaySound();
    }

    public void PlaySound()
    {
        AudioManager.PlaySoundOnPlayer(buttonSound);
    }
}