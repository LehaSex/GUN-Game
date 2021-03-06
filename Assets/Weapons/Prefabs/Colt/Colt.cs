using UnityEngine;

public class Colt : Weapon
{
    [Header("Firing")]
    public Transform FirePoint;
    //public GameObject MuzzleFlash;
    public GameObject BulletPrefab;
    
    private PlayerMove playerMovementController;
    private Animator gunAnimator;
    private string weapondata;
    //private Animator muzzleFlashAnimator;

    private void Start()
    {
        SetWeaponParams("colt");
        current_ammo = ammo;
        playerMovementController = GetComponentInParent<PlayerMove>();
        //gunAnimator = GetComponent<Animator>();
        //muzzleFlashAnimator = MuzzleFlash.GetComponent<Animator>();
    }

    /// <summary>
    /// Создает экземпляр пули и устанавливает его скорость относительно текущего направления игрока.
    /// </summary>
    protected override void HandleAttack()
    {
        // Создать пулю, установить его владельца и скорость.
        var bullet = Instantiate(BulletPrefab, FirePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Owner = playerMovementController.gameObject;
        bullet.GetComponent<Rigidbody>().velocity = new Vector3(playerMovementController.GetDirection() * BulletSpeed, 0, 0);

        // Анимации стрельбы
        //gunAnimator.SetTrigger("Fire");
        //muzzleFlashAnimator.SetTrigger("Fire");
    }
    protected override void PlayAttackParticle()
    {
        Vector3 dir = new Vector3(0, playerMovementController.GetDirection()*-90, 0);
        var shotparticle = Instantiate(ShotParticlePrefab, FirePoint.position, Quaternion.LookRotation(transform.position, dir));
        shotparticle.GetComponent<ParticleSystem>().Play();
        shotparticle.GetComponent<AudioSource>().PlayOneShot(shotSound, 0.5f);
        Destroy(shotparticle, 1.0f);
    }
    
}
