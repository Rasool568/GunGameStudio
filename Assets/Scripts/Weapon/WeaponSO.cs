using UnityEngine;

[CreateAssetMenu(fileName = "New weapon", menuName = "Weapon settings")]
public class WeaponSO : ScriptableObject
{
    public GameObject bullet;
    [Header("Общая информация")]
    public string weaponName = "New weapon";
    public int magazineSize = 7;
    public float reloadTime = 1f;
    public float timeBetweenShooting = 0.2f;
    public float timeBetweenShots = 0f;
    public float hipSpread = 0f;
    public int bulletsPerTap = 1;
    public bool allowButtonHold = false;
    public float aimSpeed = 0.2f;
    [Header("Отдача")]
    public float recoilX = -2f;
    public float recoilY = 2f;
    public float recoilZ = 0.35f;
    public float snappiness = 6f;
    public float returnSpeed = 2f;
}