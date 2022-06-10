using UnityEngine;
 
public class PlayerMenuRotator : MonoBehaviour
{
    private Transform Target;
    public float speedRotateX = 5;
    public float speedRotateY = 0;
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        Target = GetComponent<Transform>();
    }
    void Update()
    {
        if (!Input.GetMouseButton(0))
            return;
 
        float rotX = Input.GetAxis("Mouse X") * speedRotateX * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * speedRotateY * Mathf.Deg2Rad;
 
        if (Mathf.Abs(rotX) > Mathf.Abs(rotY))
            Target.Rotate(Target.up, -rotX*Mathf.Rad2Deg, Space.World);
        else
        {
            var prev = Target.rotation;
            Target.Rotate(Camera.main.transform.right, rotY*Mathf.Rad2Deg, Space.World);
            if (Vector3.Dot(Target.up, Camera.main.transform.up) < 0.5f)
                Target.rotation = prev;
        }
    }
}