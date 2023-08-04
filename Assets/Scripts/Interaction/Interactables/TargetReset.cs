public class TargetReset : Interactable
{
    protected override void Interact()
    {
        FindObjectOfType<ScoreManager>().ResetTargets();
    }
}