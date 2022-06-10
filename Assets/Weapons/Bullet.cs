using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 InitialVelocity;

   
    public GameObject Owner;

 //   private Animator animator;

    private int direction;
    private float spawnPos;
    private Rigidbody r;

    private void Start()
    {
        r = GetComponent<Rigidbody>();
        // Сохранить стартовую позицию.
        spawnPos = transform.position.x;
        //       animator = GetComponent<Animator>();
        // Если в инспекторе задано начальное значение скорости, использовать его в качестве скорости Rigidbody.
        if (InitialVelocity != Vector3.zero)
        {
            r.velocity = InitialVelocity;
        }
    }

    private void Update()
    {
        // Установить локальную переменную направления, если она еще не установлена, и этот снаряд имеет скорость.
        if (r.velocity != Vector3.zero && direction == 0)
        {
            direction = r.velocity.x < 0 ? -1 : 1;
        }
        // Удалять пулю когда она пролетит некую дистанцию.
        if (Mathf.Abs(transform.position.x) > Mathf.Abs(spawnPos + 10f))
        {
            DestroySelf();
        }
    }

    /// <summary>
    /// Столкновение
    /// </summary>
    // Переделать под рейкаст
    private void OnTriggerEnter(Collider collision)
    {
        // При столкновении с чем-либо прекратить движение и запустить анимацию взрыва.
        //animator.SetTrigger("Explode");
        GetComponent<Rigidbody>().velocity = new Vector3();
        DestroySelf();
    }

    /// <summary>
    /// Связать с анимацией explode потом
    /// </summary>
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Направление движения пули
    /// </summary>
    /// <returns>-1 лево и 1 право</returns>
    public int GetDirection()
    {
        return direction;
    }
}
