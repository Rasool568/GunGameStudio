using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{
    public bool Busy = false;

    [Header("Shooting")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform shootOrigin;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadEndSound;
    private Camera playerCamera;
    private WeaponSO currentWeapon;
    private bool shooting, reloading, allowInvoke, readyToShoot;
    private int bulletsShot, bulletsLeft;

    [Header("Weapons")]
    [SerializeField] private WeaponSO pistolWeapon;
    [SerializeField] private WeaponSO shotgunWeapon;
    [SerializeField] private WeaponSO rifleWeapon;
    [SerializeField] private WeaponSO lmgWeapon;

    [Header("Aiming")]
    private bool aiming;
    private float aimMult { get => aiming ? 4f : 1f; }
    [SerializeField] private Transform hands;
    [SerializeField] private Vector3 hipPosition;
    [SerializeField] private Vector3 adsPosition;
    private Vector3 targetPosition;

    [Header("Recoil")]
    [SerializeField] private Transform cameraHolder;
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [Header("Player UI")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI bulletText;

    [Header("Key bindings")]
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    [Header("Sway and headbob")]
    [SerializeField] private Transform movementObject;
    private Vector3 startPosition;
    [SerializeField] float swayAmount = .035f;
    [SerializeField] float swayMaxAmount = .09f;
    [SerializeField] float swaySmoothAmount = 6f;

    [SerializeField] float headbobMoveAmount = 11.5f;
    [SerializeField] float headbobIdleAmount = 2f;
    [SerializeField] float headbobIdleMultiplier = 0.5f;
    [SerializeField] float headbobHorizontalAmplitude = .005f;
    [SerializeField] float headbobVerticalAmplitude = .005f;
    PlayerMovement pm;
    float timer = 0f;

    [Header("Wall checking")]
    [SerializeField] private float wallDistance;
    [SerializeField] private Animator pistolAnim;
    [SerializeField] private LayerMask wallLayer;
    private bool isWall;

    private void Start()
    {
        startPosition = movementObject.position;
        pm = GetComponent<PlayerMovement>();
        playerCamera = pm.playerCamera.GetComponent<Camera>();
        readyToShoot = true;
        allowInvoke = true;
        shooting = false;
        ChangeWeapon(pistolWeapon);
        ReloadEnd();
    }

    private void Update()
    {
        if (Busy) return;
        MyInput();
        SwayAndHeadbob(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        WallCheck();
        Aiming();
        ControlRecoil();
    }

    private void MyInput()
    {
        if (currentWeapon == null) return;
        aiming = Input.GetKey(aimKey);

        if (Input.GetKeyDown(reloadKey) && bulletsLeft != currentWeapon.magazineSize)
            Reload();

        if (currentWeapon.allowButtonHold)
            shooting = Input.GetKey(shootKey);
        else
            shooting = Input.GetKeyDown(shootKey);

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            AudioManager.PlaySoundOnPlayer(shootSound);
            bulletsShot = 0;
            Shoot();
        }

        //Change weapon
        if(Input.GetKeyDown(KeyCode.Alpha1))
            ChangeWeapon(pistolWeapon);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeWeapon(shotgunWeapon);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeWeapon(rifleWeapon);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeWeapon(lmgWeapon);
    }

    private void ChangeWeapon(WeaponSO _weaponToChange)
    {
        if(readyToShoot && !reloading && currentWeapon != _weaponToChange)
        {
            currentWeapon = _weaponToChange;
            weaponNameText.text = currentWeapon.weaponName;
            ReloadEnd();
        }
    }

    #region Shooting
    private void Shoot()
    {
        readyToShoot = false;

        Ray _ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(_ray, out hit, ~playerLayer))
            targetPoint = hit.point;
        else
            targetPoint = _ray.GetPoint(75f);

        Vector3 directionWithoutSpread = targetPoint - shootOrigin.position;
        float xSpread = Random.Range(-currentWeapon.hipSpread, currentWeapon.hipSpread);
        float ySpread = Random.Range(-currentWeapon.hipSpread, currentWeapon.hipSpread);
        Vector3 finalDirection = directionWithoutSpread + new Vector3(xSpread, ySpread, 0);
        AddRecoil();

        GameObject currentBullet = Instantiate(currentWeapon.bullet, shootOrigin.position, Quaternion.identity);
        currentBullet.transform.forward = finalDirection.normalized;

        bulletsLeft--;
        bulletsShot++;
        bulletText.text = (bulletsLeft / currentWeapon.bulletsPerTap).ToString() + "/" + (currentWeapon.magazineSize / currentWeapon.bulletsPerTap);

        if (allowInvoke)
        {
            Invoke("ResetShot", currentWeapon.timeBetweenShooting);
            allowInvoke = false;
        }

        if(currentWeapon.timeBetweenShots > 0 && bulletsShot > 1)
            AudioManager.PlaySoundOnPlayer(shootSound);

        if (bulletsShot < currentWeapon.bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", currentWeapon.timeBetweenShots);
    }

    private void Reload()
    {
        Invoke("ReloadEnd", currentWeapon.reloadTime);
    }

    private void ReloadEnd()
    {
        bulletsLeft = currentWeapon.magazineSize;
        bulletText.text = (bulletsLeft / currentWeapon.bulletsPerTap).ToString() + "/" + (currentWeapon.magazineSize / currentWeapon.bulletsPerTap);
        AudioManager.PlaySoundOnPlayer(reloadEndSound);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Aiming()
    {
        if (aiming && !isWall)
            targetPosition = adsPosition;
        else
            targetPosition = hipPosition;

        hands.localPosition = Vector3.Lerp(hands.localPosition, targetPosition, Time.deltaTime * currentWeapon.aimSpeed);
    }
    #endregion

    #region Recoil

    private void ControlRecoil()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, currentWeapon.returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, currentWeapon.snappiness * Time.deltaTime);
        cameraHolder.localRotation = Quaternion.Euler(currentRotation);
    }

    private void AddRecoil()
    {
        targetRotation += new Vector3(currentWeapon.recoilX / aimMult,
            Random.Range(-currentWeapon.recoilY, currentWeapon.recoilY) / aimMult * 2,
            Random.Range(-currentWeapon.recoilZ, currentWeapon.recoilZ) / aimMult * 2);
    }

    #endregion

    #region WeaponSway and Headbob
    private void SwayAndHeadbob(float _moveX, float _moveY)
    {
        float moveX;
        float moveY;

        moveX = Mathf.Clamp(-_moveX * swayAmount / aimMult, -swayMaxAmount / aimMult, swayMaxAmount / aimMult);
        moveY = Mathf.Clamp(-_moveY * swayAmount / aimMult, -swayMaxAmount / aimMult, swayMaxAmount / aimMult);

        Vector3 finalPos = WeaponHeadbob(new Vector3(moveX, moveY, 0));
        movementObject.localPosition = Vector3.Lerp(movementObject.localPosition, startPosition + finalPos, Time.deltaTime * swaySmoothAmount);
    }

    private Vector3 WeaponHeadbob(Vector3 _vector)
    {
        if (pm.Busy || !pm.controller.isGrounded) return _vector;
        if (Mathf.Abs(pm.moveDirection.x) > .1f || Mathf.Abs(pm.moveDirection.z) > .1f)
        {
            timer += Time.deltaTime * pm.speedMultiplier * headbobMoveAmount;
            return new Vector3(_vector.x + Mathf.Sin(timer * headbobIdleMultiplier) * headbobHorizontalAmplitude * pm.speedMultiplier / aimMult,
                _vector.y + Mathf.Sin(timer) * headbobVerticalAmplitude * pm.speedMultiplier / aimMult,
                _vector.z);
        }
        else
        {
            timer += Time.deltaTime * headbobIdleAmount;
            return new Vector3(_vector.x + Mathf.Sin(timer * headbobIdleMultiplier) * headbobHorizontalAmplitude * pm.speedMultiplier / aimMult,
                _vector.y + Mathf.Sin(timer) * headbobVerticalAmplitude * pm.speedMultiplier / aimMult,
                _vector.z);
        }
    }
    #endregion

    #region Wall cliping
    private void WallCheck()
    {
        isWall = false;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, wallDistance, ~playerLayer))
            isWall = true;
        pistolAnim.SetBool("CloseToWall", isWall);
    }
    #endregion
}