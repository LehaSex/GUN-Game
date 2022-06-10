using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Синхронизирует персонажа удаленно подключенного игрока с использованием полученных сетевых данных.
/// </summary>
public class PlayerNetworkRemoteSync : MonoBehaviour
{
    public RemotePlayerNetworkData NetworkData;

    /// <summary>
    /// Скорость (в секундах), с которой можно плавно интерполировать фактическое положение игрока при получении исправленных данных.
    /// </summary>
    public float LerpTime = 0.05f;

    private GameManager gameManager;
    private PlayerMove playerMovementController;
    private PlayerWeapon playerWeaponController;
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private float lerpTimer;
    private Vector3 lerpFromPosition;
    private Vector3 lerpToPosition;
    private bool lerpPosition;


    private void Start()
    {
        // Кэшировать ссылку на необходимые компоненты.
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerMovementController = GetComponentInChildren<PlayerMove>();
        playerWeaponController = GetComponentInChildren<PlayerWeapon>();
        playerRigidbody = GetComponentInChildren<Rigidbody>();
        playerTransform = playerRigidbody.GetComponent<Transform>();

        // Добавьте прослушиватель событий для обработки входящих данных о состоянии соответствия.
        gameManager.NakamaConnection.Socket.ReceivedMatchState += EnqueueOnReceivedMatchState;
    }

    private void LateUpdate()
    {
        // Если мы не пытаемся интерполировать позицию игрока, то возвращаемся пораньше.
        if (!lerpPosition)
        {
            return;
        }

        // // Интерполируйте позицию игрока на основе хода lerp.
        playerTransform.position = Vector3.Lerp(lerpFromPosition, lerpToPosition, lerpTimer / LerpTime);
        lerpTimer += Time.deltaTime;

        // // Если мы достигли конца lerp, явно принудительно переместите игрока в последнюю известную правильную позицию.
        if (lerpTimer >= LerpTime)
        {
            playerTransform.position = lerpToPosition;
            lerpPosition = false;
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.NakamaConnection.Socket.ReceivedMatchState -= EnqueueOnReceivedMatchState;
        }
    }

    /// <summary>
    /// Передает выполнение обработчика событий основному потоку unity, чтобы мы могли взаимодействовать с GameObjects.
    /// </summary>
    /// <param name="matchState">The incoming match state data.</param>
    private void EnqueueOnReceivedMatchState(IMatchState matchState)
    {
        var mainThread = UnityMainThreadDispatcher.Instance();
        mainThread.Enqueue(() => OnReceivedMatchState(matchState));
    }

    /// <summary>
    /// Вызывается при получении данных синхронизации с сервера Nakama.
    /// </summary>
    /// <param name="matchState">The incoming match state data.</param>
    private void OnReceivedMatchState(IMatchState matchState)
    {
        // Если входящие данные не связаны с этим удаленным игроком, проигнорируйте их и вернитесь раньше.
        if (matchState.UserPresence.SessionId != NetworkData.User.SessionId)
        {
            return;
        }

        // Решить, что делать, основываясь на Коде операции входящих данных о состоянии, как определено в кодах операций.
        switch (matchState.OpCode)
        {
            case OpCodes.VelocityAndPosition:
                UpdateVelocityAndPositionFromState(matchState.State);
                break;
            case OpCodes.Input:
                SetInputFromState(matchState.State);
                break;
        //    case OpCodes.Died:
          //      playerMovementController.PlayDeathAnimation();
           //     break;
            default:
                break;
        }
    }

    /// <summary>
    /// Преобразует массив байтов строки JSON в кодировке UTF8 в словарь.
    /// </summary>
    /// <param name="state">The incoming state byte array.</param>
    /// <returns>A Dictionary containing state data as strings.</returns>
    private IDictionary<string, string> GetStateAsDictionary(byte[] state)
    {
        return Encoding.UTF8.GetString(state).FromJson<Dictionary<string, string>>();
    }

    /// <summary>
    /// Устанавливает соответствующие входные значения на контроллере PlayerMovementController и контроллере PlayerWeapon на основе входящих данных о состоянии.
    /// </summary>
    /// <param name="state">Словарь входящих состояний.</param>
    private void SetInputFromState(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);

        playerMovementController.SetHorizontalMovement(float.Parse(stateDictionary["horizontalInput"]));
        playerMovementController.SetJump(bool.Parse(stateDictionary["jump"]));
        playerMovementController.SetJumpOff(bool.Parse(stateDictionary["jumpOff"]));
        playerWeaponController.SetAttackHeld(bool.Parse(stateDictionary["attackHeld"]));
        playerWeaponController.SetNewId(int.Parse(stateDictionary["id"]));
        if (bool.Parse(stateDictionary["attack"]))
        {
            playerWeaponController.Attack();
        }    
    }

    /// <summary>
    /// Обновляет скорость и положение игрока на основе поступающих данных о состоянии.
    /// </summary>
    /// <param name="state">Массив байтов входящего состояния.</param>
    private void UpdateVelocityAndPositionFromState(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);

        playerRigidbody.velocity = new Vector3(float.Parse(stateDictionary["velocity.x"]), float.Parse(stateDictionary["velocity.y"]), float.Parse(stateDictionary["velocity.z"]));

        var position = new Vector3(
            float.Parse(stateDictionary["position.x"]),
            float.Parse(stateDictionary["position.y"]),
            float.Parse(stateDictionary["position.z"])
            );

        // Лагкомпенсация в исправленное положение.
        lerpFromPosition = playerTransform.position;
        lerpToPosition = position;
        lerpTimer = 0;
        lerpPosition = true;
    }
}
