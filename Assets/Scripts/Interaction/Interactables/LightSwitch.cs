using UnityEngine;

public class LightSwitch : Interactable
{
    [SerializeField] private GameObject[] lamps;
    [SerializeField] private GameObject lightSwitch;
    [SerializeField] private Material onMat;
    [SerializeField] private Material offMat;
    [SerializeField] private AudioClip switchSound;

    protected override void Interact()
    {
        foreach (GameObject lamp in lamps)
        {
            lamp.SetActive(!lamp.activeSelf);
        }
        if (lamps[0].activeSelf)
            lightSwitch.GetComponent<MeshRenderer>().material = onMat;
        else
            lightSwitch.GetComponent<MeshRenderer>().material = offMat;

        AudioManager.PlaySoundOnPlayer(switchSound);
    }
}
