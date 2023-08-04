using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private Target[] targets;
    [SerializeField] private AudioClip targetSound;
    [SerializeField] private AudioClip targetResetSound;
    private int score;

    private void Start()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    public void AddScore(int _score)
    {
        score += _score;
        scoreText.text = score.ToString();
        AudioManager.PlaySoundOnPlayer(targetSound);
    }

    public void ResetTargets()
    {
        foreach (Target target in targets)
        {
            target.ResetTarget();
        }
        AudioManager.PlaySoundOnPlayer(targetResetSound);
    }
}
