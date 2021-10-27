using UnityEngine;

public class Rotation : MonoBehaviour
{
    public static Transform Gun_pos_transform;

    public static Quaternion Anaconda;
    public Quaternion Anaconda_rotation;

    private Player_Gun_Inventory pgi;
    private static Quaternion current_gun_rotation;

    [SerializeField]private float lerp_speed = 8f;

    private void Awake()
    {
        Anaconda = Anaconda_rotation;
        Gun_pos_transform = GetComponent<Transform>();
        pgi = GetComponentInParent<Player_Gun_Inventory>();
    }
    private void LateUpdate()
    {
        Look_At_Center(current_gun_rotation);
    }

    public void Set_Rotation(Quaternion qt)
    {
        transform.localRotation = qt;
    }
    public static void Set_Rotation(string qt)
    {
        if(qt.Contains("Anaconda"))
        {
            Gun_pos_transform.localRotation = Anaconda;
            current_gun_rotation = Anaconda;
        }
        else
        {
            Gun_pos_transform.localRotation = Quaternion.identity;
        }
    }

    public void Look_At_Center(Quaternion desired_rotation)
    {
        if (pgi.GetCurrentWeapon() == null) return;

        Gun_pos_transform.localRotation = Quaternion.Lerp(Gun_pos_transform.localRotation, desired_rotation, Time.deltaTime * lerp_speed);
    }
}
