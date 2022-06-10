using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Обработка перемещений игрока
/// </summary>
public class PlayerMove : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent CollidedWithProjectile;

/*     [Header("Rendering")]
    public GameObject WeaponHolder; */

    [Header("Movement")]
    public float MovementSpeed = 1.0f;
    public float GroundedBufferTime = 0.15f;
    public ParticleSystem FootstepParticles;

    [Header("Jumping")]
    public float JumpBufferTime = 0.1f;
    public float JumpForce = 2.5f;
    private bool jump;
    private bool jumpOff;
    [SerializeField]private int _jumps = 2;
    public int jumpCount = 2;
    public ParticleSystem LandingParticles;

    [Header("Sounds")]
    public AudioSource PlayerSFX;
    public AudioClip hitSound;
    public AudioClip dropSound;
    public AudioClip dieSound;

    [Header("Ground Collision")]
    public LayerMask GroundLayer;
    public LayerMask PlatformLayer;
    public LayerMask PlayerLayer;
    public int intLayerP;
    public int intLayerG;

    [Header("Bullet Collision")]
    public ParticleSystem BloodParticles;
    public float KnockbackForce;
    public float ForceHor, ForceVert;
    public bool hited;
    private float i = .2f;
    private IEnumerator coroutine;

    private Rigidbody r;
    private Animator animator;
    private float horizontalMovement;
    private int direction = 1;
    private bool isGrounded;
    private float jumpTimer;
    private float groundedTimer;
    private bool falling;
    private ParticleSystem.EmissionModule footstepEmission;
    private int PlayerObject, CollideObject;

    private void Start()
    {
        r = GetComponent<Rigidbody>();
        footstepEmission = FootstepParticles.emission;
        animator = GetComponent<Animator>();
        intLayerP = LayerMask.NameToLayer("Player");
        intLayerG = LayerMask.NameToLayer("Player");
        PlayerSFX = GetComponent<AudioSource>();
        Physics.IgnoreLayerCollision(intLayerP, intLayerG, true);
        
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", Mathf.Abs(horizontalMovement));
        
        if (jump)
        {
            --_jumps;
            jumpTimer = Time.time + JumpBufferTime;
        }
        HandleJumpingOff();

       

        footstepEmission.rateOverTime = 0f;

        if (horizontalMovement != 0)
        {
            direction = horizontalMovement < 0 ? -1 : 1;
            Flip(horizontalMovement < 0) ;

            if (isGrounded)
            {
                footstepEmission.rateOverTime = 20f;
            }
        }
/*         if (r.velocity.y >0)
        {
            Physics.IgnoreLayerCollision(intLayerP, intLayerG, true);
        }
        else
        {
            Physics.IgnoreLayerCollision(intLayerP, intLayerG, false);
        } */
    }
    

    public void Flip(bool flip)
    {
        Vector3 Temp = transform.rotation.eulerAngles;
        if(flip)
        {
            Temp.y = 180f;
            transform.eulerAngles = Temp;
        }
        if(!flip)
        {
           Temp.y = 0f;
           transform.eulerAngles = Temp; 
        }
    }

    private void FixedUpdate()
    {
        CheckIfFalling();
        CheckIfGrounded();
        HandleMovement();
        HandleJumping();
        if (hited)
        {
            i = i - Time.deltaTime;
            if (i > 0)
            {
                r.AddForce(new Vector2(ForceHor, 0), ForceMode.Impulse);
            }
            else
            {
                i = .2f;
                hited = false;
            }
        }

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
        {
            // Сдох
            PlayerSFX.PlayOneShot(dieSound, 1);
            CollidedWithProjectile.Invoke();
        }

        if (collider.tag != "Bullet")
        {
            return;
        }

        var bullet = collider.gameObject.GetComponent<Bullet>();

        if (bullet.Owner == gameObject)
        {
            return;
        }

        r.velocity = new Vector2(0, r.velocity.y);
        KnockbackForce = Mathf.Abs(bullet.Owner.GetComponent<PlayerWeapon>().Weapon.power);
        ForceHor = bullet.GetDirection() * KnockbackForce;
        ForceVert = KnockbackForce/5;
        hited = true;
        // Кровь
        BloodParticles.Play();
        PlayerSFX.PlayOneShot(hitSound, 1);

//        animator.SetTrigger("Hit");
        

    }

    private void CheckIfFalling()
    {
        var wasFalling = falling == true;

        // Установить новое состояние падения в зависимости от того, является ли скорость игрока y ниже 0.
        falling = r.velocity.y < 0;

        // Если игрок не падает, запустить событие анимации приземления.
        if (!falling)
        {
            animator.SetTrigger("Land");
        }
        
        if (!wasFalling && falling)
        {
    //        animator.SetTrigger("Fall");
        }
    }

    /// <summary>
    /// Проверяет, находится ли игрок на земле, и устанавливает переменную isGrounded.
    /// </summary>
    private void CheckIfGrounded()
    {
        var wasGrounded = isGrounded == true;
        CapsuleCollider capsulecollider = transform.GetComponent<CapsuleCollider>();
        float radius = capsulecollider.radius * 0.9f;
        Vector3 pos = transform.position + Vector3.up*(radius*0.9f);
        var collider = Physics.CheckSphere(pos, radius, GroundLayer);

        isGrounded = collider != false; //null

        if (isGrounded)
        {
            groundedTimer = GroundedBufferTime;
            _jumps = jumpCount;
        }
        else
        {
            groundedTimer -= Time.deltaTime;
        }

        // Если приземлился то партикль приземления
        if (!wasGrounded && isGrounded)
        {
            PlayerSFX.PlayOneShot(dropSound, (float)0.1);
            LandingParticles.Play();
        }
    }

    private void HandleMovement()
    {
        r.velocity = new Vector2(horizontalMovement * MovementSpeed, r.velocity.y);
    }

    private void HandleJumping()
    {
        if (jumpTimer > Time.time && _jumps > 0)
        {
            r.velocity = new Vector2(r.velocity.x, JumpForce);
            jumpTimer = 0;
            groundedTimer = 0;
            if (_jumps-1 > 0)
            {
            animator.SetTrigger("PistolJump");
            }

        }
    }

    private void HandleJumpingOff()
    {
        if (jumpOff)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3 (0, .2f, 0), transform.TransformDirection(Vector3.down), out hit, 1.0f);
            Debug.DrawRay(transform.position + new Vector3 (0, .2f, 0), transform.TransformDirection(Vector3.down), Color.green, 1f);
            if (hit.transform.tag == "Platform")
            {
                animator.SetTrigger("JumpDown");
                Collider col= GetComponentInChildren<CapsuleCollider>();
                Physics.IgnoreCollision(col, hit.collider, true);
            }
        }
    }

    public void SetHorizontalMovement(float value)
    {
        horizontalMovement = value;
    }

    public void SetJump(bool value)
    {
        jump = value;
    }

    public void SetJumpOff(bool value)
    {
        jumpOff = value;
    }

    public int GetDirection()
    {
        return direction;
    }

/*     public void PlayDeathAnimation()
    {
        animator.SetBool("Dead", true);
    } */
}
