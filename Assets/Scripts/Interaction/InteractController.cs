using UnityEngine;
using TMPro;

public class InteractController : MonoBehaviour
{
    public bool lockInteracting;

    private PlayerMovement pm;
    private Transform playerCamera;

    [Header("Variables")]
    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    private Interactable interactable;

    private void Start()
    {
        lockInteracting = false;

        pm = GetComponent<PlayerMovement>();
        playerCamera = pm.playerCamera;

        itemDescriptionText.text = string.Empty;
        itemNameText.text = string.Empty;
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey) && interactable != null && !lockInteracting)
        {
            interactable.BaseInteract();
        }
    }

    private void FixedUpdate()
    {
        if (!lockInteracting)
            PlayerInteract();
    }

    //ѕровер€ет возможность взаимодействи€ и взаимодействует при необходимости
    private void PlayerInteract()
    {
        Ray ray = new(playerCamera.position, playerCamera.forward);
        RaycastHit hitInfo;

        interactable = null;

        if (itemDescriptionText.text != string.Empty)
        {
            itemDescriptionText.text = string.Empty;
            itemNameText.text = string.Empty;
        }

        if (Physics.Raycast(ray, out hitInfo, interactDistance, interactableLayer))
        {
            if (interactable = hitInfo.collider.GetComponent<Interactable>())
            {
                if (itemDescriptionText.text != interactable.itemDescription)
                {
                    itemDescriptionText.text = interactable.itemDescription;
                    itemNameText.text = interactable.itemName;
                }
            }
        }
    }
}
