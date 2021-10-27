using UnityEngine;
using UnityEngine.UI;
public class Crosshair_Operations : MonoBehaviour
{
    public bool Lerping = false;

    [SerializeField]private float speed = 360f;
    [SerializeField]private float lerp_speed = 5f;
    [SerializeField]private Vector3 Rotating_Vector = new Vector3(0, 0, 1f);
    [SerializeField]private Weapon current_weapon;
    [SerializeField]private Image image;
    [SerializeField]private Vector3 center_of_the_screen;
    [SerializeField]private Camera cam;
    private void Awake()
    {
        image = GetComponent<Image>();
        cam = Camera.main;
    }
    private void Update()
    {
        Reload_Animation();
        Lerp_Position();
    }
    private void Lerp_Position()
    {
        if(current_weapon == null)
        {
            Lerp_To_Camera_Center(); 
            return;
        }

        Vector3 hit_point = current_weapon.Get_Hit_Point();

        if(hit_point == Vector3.zero)
        {
            Lerp_To_Camera_Center();
        }
        else
        {
            Lerp_To_Gun_Point(hit_point);
        }
    }
    private void Lerp_To_Gun_Point(Vector3 hit_position)
    {
        transform.position = Vector3.Lerp(transform.position, cam.WorldToScreenPoint(hit_position), Time.deltaTime * lerp_speed);
    }
    private void Lerp_To_Camera_Center()
    {
        transform.position = Vector3.Lerp(transform.position, Get_Camera_Center(), Time.deltaTime * lerp_speed);
    }
    private Vector3 Get_Camera_Center()
    {
        return new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
    }
    private void Reload_Animation()
    {
        if (Player_Gun_Inventory.current_gun == null)
        {
            Lerp_To_Camera_Center();
            image.color = Color.white;
            transform.rotation = Quaternion.identity;
            return;
        }

        current_weapon = Player_Gun_Inventory.current_gun.GetComponent<Weapon>();

        if(current_weapon.is_reloading)
        {
            Rotate();
        }
        else
        {
            Lerping = true;
            image.color = Color.white;
            transform.rotation = Quaternion.identity;
        }
    }
    private void Rotate()
    {
            image.color = Color.red;
            transform.Rotate(Rotating_Vector,Time.deltaTime * speed); // 1 rotation per second for speed => 360 //
    }
}