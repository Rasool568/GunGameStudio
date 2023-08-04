using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField][Range(1,3)]private int score = 1;
    private bool isHit = false;

    public void TakeHit()
    {
        if(isHit) return;
        isHit = true;
        FindObjectOfType<ScoreManager>().AddScore(score);
        GetComponent<Animator>().SetTrigger("Hit");
    }

    public void ResetTarget()
    {
        if(!isHit) return;
        GetComponent<Animator>().SetTrigger("Reset");
        isHit = false;
    }
}