using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    [SerializeField]
    float leftBorder; 
    [SerializeField]
    float rightBorder; 
    [SerializeField]
    float upBorder; 
    [SerializeField]
    float downBorder; 
    [SerializeField]
    float depth;

    public float smoothSpeed = 0.15f;
    void Start()
    {
        var player = GetComponentInChildren<PlayerMove>();
        target = player.GetComponent<Transform>();
        offset = Camera.main.transform.position - target.position;
    }
    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, desiredPosition, smoothSpeed);
        Camera.main.transform.position = new Vector3
        (Mathf.Clamp(Camera.main.transform.position.x, leftBorder, rightBorder),
        Mathf.Clamp(Camera.main.transform.position.y, downBorder, upBorder),
        Camera.main.transform.position.z
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftBorder, upBorder,depth), new Vector3(rightBorder, upBorder,depth));
        Gizmos.DrawLine(new Vector3(leftBorder, downBorder,depth), new Vector3(rightBorder, downBorder,depth));
        Gizmos.DrawLine(new Vector3(leftBorder, upBorder,depth), new Vector3(leftBorder, downBorder,depth));
        Gizmos.DrawLine(new Vector3(rightBorder, upBorder,depth), new Vector3(rightBorder, downBorder,depth));

    }
}