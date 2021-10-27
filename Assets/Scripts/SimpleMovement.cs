using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    private static SimpleMovement _instance;
    private Rigidbody rb;

    [SerializeField]private Transform cam;
    [SerializeField]private Vector3 initialScale;

    [SerializeField]public bool Can_jump = false;
    [SerializeField]public bool Is_Moving = false;
    [SerializeField]public bool Is_Running = false;

    [SerializeField]private float jumpForce = 1000f;
    [SerializeField]private float speed = 15f;
    [SerializeField]private float run_speed = 30f;
    [SerializeField]private float temp_speed = 15f;
    [SerializeField]private float sensitivity;
    [SerializeField]private float headRotation = 90f;
    [SerializeField]private float headRotationLimit = 90f;
    [SerializeField]private float crouch_speed = 10f;
    private static SimpleMovement Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        initialScale = transform.localScale;
        cam.position = transform.position + new Vector3(0, (initialScale.y) / 2, 0);
    }
    private void Start()
    {
        //cam.transform.parent = transform;
        temp_speed = speed;
        run_speed = speed * 2;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Can_jump = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Can_jump = false;
        }
    }
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(Can_jump)
            { 
                rb.AddForce(transform.up * jumpForce * rb.mass, ForceMode.Force);
                Can_jump = false;
            } 
        }      
    }
    private void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(initialScale.x, (initialScale.y / 2), initialScale.z), Time.deltaTime * crouch_speed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(initialScale.x, initialScale.y, initialScale.z), Time.deltaTime * crouch_speed);
        }
    }
    private void MouseLook()
    {
        float x = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime;
        float y = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * -1f;
        transform.Rotate(0f, x, 0f);
        headRotation += y;
        headRotation = Mathf.Clamp(headRotation, -headRotationLimit, headRotationLimit);
        cam.localEulerAngles = new Vector3(headRotation, 0f, 0f);
    }

    private void Run()
    {
        if(Is_Moving && Input.GetKey(KeyCode.LeftShift))
        {
            speed = run_speed;
            Is_Running = true;
        }
        else
        {
            speed = temp_speed;
            Is_Running = false;
        }
    }
    private void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 moveBy = transform.right * x + transform.forward * z;
        float mag = moveBy.sqrMagnitude;

        if (mag != 0)
        {
            Is_Moving = true;
        }
        else
        {
            Is_Moving = false;
        }

        if (Can_jump)
        {
            rb.MovePosition(transform.position + moveBy.normalized * speed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(transform.position + moveBy.normalized * speed * Time.fixedDeltaTime / 1.4f);
            Is_Moving = true;
        }   
    }
    private void FixedUpdate()
    {
        Movement();
    }
    private void Update()
    {
        MouseLook();
        Jump();
        Crouch();
        Run();
    }
}
