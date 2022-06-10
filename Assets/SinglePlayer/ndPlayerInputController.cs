using UnityEngine;

public class ndPlayerInputController : MonoBehaviour
{
    [HideInInspector] public float HorizontalInput;
    [HideInInspector] public bool Jump;
    [HideInInspector] public bool JumpOff;
    [HideInInspector] public bool AttackHeld;
    [HideInInspector] public bool Attack;
    [HideInInspector] public bool InputChanged;
    
    private PlayerMove playerMovementController;
    private PlayerWeapon playerWeaponController;

    private void Start()
    {
        playerMovementController = GetComponentInChildren<PlayerMove>();
        playerWeaponController = GetComponentInChildren<PlayerWeapon>();
    }

    private void Update()
    {
        // Получить текущие состояния кнопок
        var horizontalInput = Input.GetAxisRaw("HorizontalP");
        var jump = Input.GetButtonDown("JumpP");
        var jumpOff = Input.GetButtonDown("VerticalP");
        var attackHeld = Input.GetButton("FireP");
        var attack = Input.GetButtonDown("FireP");

        // Проверка изменения состояния ввода с момента последнего кадра. (true/false)
        InputChanged = (horizontalInput != HorizontalInput || jump != Jump || attack != Attack || attackHeld != AttackHeld);

        // Для чтения из других мест
        HorizontalInput = horizontalInput;
        Jump = jump;
        JumpOff = jumpOff;
        AttackHeld = attackHeld;
        Attack = attack;

        // Установка инпута игрока 
        playerMovementController.SetHorizontalMovement(HorizontalInput);
        playerMovementController.SetJump(Jump);
        playerMovementController.SetJumpOff(JumpOff);
        playerWeaponController.SetAttackHeld(AttackHeld);

        // Атаковать если атака true
        if (attack)
        {
            playerWeaponController.Attack();
        }
    }
}
