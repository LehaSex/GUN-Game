using UnityEngine;


public class PlayerHealthController : MonoBehaviour
{
    public PlayerDiedEvent PlayerDied;
    public int MaxHealth = 1;
    
    private int health = 1;
    private PlayerMove playerMovementController;
    private PlayerWeapon playerWeaponController;
    private PlayerInputController playerInputController;

    private void Start()
    {
        // Инициализировать ивент
        if (PlayerDied == null)
        {
            PlayerDied = new PlayerDiedEvent();
        }

        // Установить хп персонажу
        health = MaxHealth;

        playerMovementController = GetComponentInChildren<PlayerMove>();
        playerWeaponController = GetComponentInChildren<PlayerWeapon>();
        playerInputController = GetComponent<PlayerInputController>();

        // Начать слушать OnCollidedWithProjectile
        playerMovementController.CollidedWithProjectile.AddListener(OnCollidedWithProjectile);
    }

    /// <summary>
    /// Уменьшает хп персонажа
    /// </summary>
    /// <param name="damage">Количество дамага который получает игрок.</param>
    public void TakeDamage(int damage = 1)
    {
        // Уменьшить хп персонажа
        health -= damage;

        // Если хп персонажа 0 и меньше то вызывается ивент PlayerDied и отключается всё
        if (health <= 0)
        {
            playerInputController.enabled = false;
            playerMovementController.SetHorizontalMovement(0);
            playerMovementController.SetJump(false);
            playerWeaponController.SetAttackHeld(false);
            PlayerDied.Invoke(gameObject);
        }
    }
    
    /// <summary>
    /// Вызывается когда CollidedWithProjectile
    /// </summary>
    private void OnCollidedWithProjectile()
    {
        TakeDamage();
    }
}
