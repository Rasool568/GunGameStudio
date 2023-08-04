using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string itemName;
    public string itemDescription;

    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {

    }
}
