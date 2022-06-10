using UnityEngine;

/// <summary>
/// Синхронизирует состояние локального игрока по сети, отправляя частые сетевые пакеты, содержащие соответствующую информацию, такую как скорость, местоположение и входные данные.
/// </summary>
public class PlayerNetworkLocalSync : MonoBehaviour
{
    /// <summary>
    /// Как часто отправлять данные о скорости и местоположении игрока по сети в секундах.
    /// </summary>
    public float StateFrequency = 0.1f;

    private GameManager gameManager;
    //private PlayerHealthController playerHealthController;
    private PlayerInputController playerInputController;
    private PlayerWeapon playerWeaponController;
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private float stateSyncTimer;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerInputController = GetComponent<PlayerInputController>();
        playerWeaponController = GetComponentInChildren<PlayerWeapon>();
        playerRigidbody = GetComponentInChildren<Rigidbody>();
        playerTransform = playerRigidbody.GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        // Отправлять текущую скорость и положение игрока каждые секунды с частотой состояния.
        if (stateSyncTimer <= 0)
        {
            // Отправьте сетевой пакет, содержащий скорость и положение игрока.
            gameManager.SendMatchState(
                OpCodes.VelocityAndPosition,
                MatchDataJson.VelocityAndPosition(playerRigidbody.velocity, playerTransform.position));

            stateSyncTimer = StateFrequency;
        }

        stateSyncTimer -= Time.deltaTime;


        if (!playerInputController.InputChanged)
        {
           return;
        }

        // Пакет с текущим инпутом на сервер
        gameManager.SendMatchState(
            OpCodes.Input, 
            MatchDataJson.Input(playerInputController.HorizontalInput, playerInputController.Jump, playerInputController.JumpOff, playerInputController.Attack, playerInputController.AttackHeld, playerWeaponController.id)
        );
    }
}
