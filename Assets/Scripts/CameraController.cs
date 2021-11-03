using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public Transform camTransform;
    public Vector3 Offset;
    public float SmoothTime = 0.150f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private static CameraController _instance;

    public static CameraController Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
     private void FixedUpdate()
    {
        targetPosition = Target.position + Offset;
        camTransform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime * Time.fixedDeltaTime);
        transform.LookAt(camTransform);
    }
}
