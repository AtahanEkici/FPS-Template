using UnityEngine;
using UnityEditor;

public class Weapon : MonoBehaviour
{
    public bool is_Equipped = false;
    public bool ready_To_fire = true;
    public bool is_Bolt_Action = true;
    public bool is_reloading = false;
    public bool Auto_Reload = true;
    public bool rotation_set = false;

    public GameObject gun_prefab;
    public GameObject Gun_Exit;
    public GameObject bullet_prefab;

    public uint max_carried_magazine = 60;
    public uint max_magazine_size = 6;
    public float range = 1000f;

    public KeyCode Reload_Button = KeyCode.R;

    [SerializeField]private float projectile_speed = 5f;
    [SerializeField]private float fire_rate = 1f;
    [SerializeField]private float reload_speed = 1f;

    public uint magazine_count;
    public float current_fire_rate;
    public float temp_reload;
    public uint carried_magazine_temp;
    public Ammo specified_ammo;

    private CameraShake camshake;
    private void Awake()
    {
        if (transform.childCount > 0)
        {
            Gun_Exit = transform.GetChild(0).gameObject;
        }
        else
        {
            Debug.LogError("Gun_Exit not set");
        }

        specified_ammo = bullet_prefab.GetComponent<Ammo>();
        camshake = Camera.main.transform.parent.GetComponentInParent<CameraShake>();
    }
    private void Start()
    {
        magazine_count = max_magazine_size;
        current_fire_rate = fire_rate;
        temp_reload = reload_speed;
        carried_magazine_temp = max_carried_magazine;
    }
    private void Update()
    {
        Shoot();
        AutoReload();
        Reload_Timer();
        Get_Hit_Point();
    }

    public void Glow_Needed_Ammo()
    {
        if (is_Equipped == false) return;

        Ammo[] ammo = FindObjectsOfType<Ammo>();

        if (ammo == null) return;

        for(int i=0;i<ammo.Length;i++)
        {
            if(ammo[i].gameObject.name.Contains(bullet_prefab.name))
            {
                ammo[i].Glow();
            }
        }
    }

    public Vector3 Get_Hit_Point()
    {
        if (is_Equipped == false) return Vector3.zero;
        RaycastHit hit;
        Ray ray = new Ray(Gun_Exit.transform.position, Calculate_Shooting_Vector());
        //Debug.DrawRay(ray.origin, ray.direction * range, Color.black);
        Physics.Raycast(ray.origin, ray.direction * range, out hit);
        return hit.point;
    }

    public void Rotation_Check()
    {
        if (is_Equipped == false) return;

        else if (rotation_set) return;

        else if (transform.parent == null) return;

        else
        {
            transform.parent.gameObject.transform.rotation = gun_prefab.transform.rotation;
            transform.rotation = Quaternion.identity;
            rotation_set = true;
        }
    }

    private void Shoot()
    {
        if(is_Bolt_Action)
        {
            if (Input.GetMouseButtonDown(0) && ready_To_fire && is_Equipped && magazine_count > 0 && is_reloading == false)
            {
                GameObject go = Instantiate(bullet_prefab, Gun_Exit.transform.position, Quaternion.identity);
                Bullet bullet = go.GetComponent<Bullet>();
                Rigidbody rb = go.GetComponent<Rigidbody>();
                rb.AddRelativeForce(Calculate_Shooting_Vector() * projectile_speed, ForceMode.Impulse);
                magazine_count--;
                camshake.InduceStress(1, 1, 1f);
                Destroy(go, 10f);
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && ready_To_fire && is_Equipped && magazine_count > 0 && is_reloading == false)
            {
                GameObject go = Instantiate(bullet_prefab, Gun_Exit.transform.position, Quaternion.identity);
                Bullet bullet = go.GetComponent<Bullet>();
                Rigidbody rb = go.GetComponent<Rigidbody>();
                rb.AddRelativeForce(Calculate_Shooting_Vector() * projectile_speed, ForceMode.Impulse);
                magazine_count--;
                camshake.InduceStress(1, 1, 1f);
                Destroy(go, 10f);
            }
        }
        Glow_Needed_Ammo();
    }
    public void Cancel_Reload()
    {
        is_reloading = false;
        temp_reload = reload_speed;
    }
    private void AutoReload()
    {
        if (is_Equipped == false) return;

        if (carried_magazine_temp == 0) return;

        if (Input.GetKeyDown(Reload_Button) && is_reloading == false)
        {
            Reload();
        }

        if (magazine_count == 0 && Auto_Reload && is_reloading == false)
        {
            Reload();
        }
    }

    private void Reload_Timer()
    {
        if(is_reloading)
        {
            temp_reload -= Time.deltaTime;

            if (temp_reload <= 0)
            {
                uint required = max_magazine_size - magazine_count;

                if (carried_magazine_temp > required)
                {
                    carried_magazine_temp -= required;
                    magazine_count = max_magazine_size;
                }
                else if ((carried_magazine_temp + magazine_count) == required)
                {
                    carried_magazine_temp = 0;
                    magazine_count = max_carried_magazine;
                }
                else if ((carried_magazine_temp + magazine_count) < required)
                {
                    magazine_count += carried_magazine_temp;
                    carried_magazine_temp = 0;
                }
                else
                {
                    Debug.Log("You should not have been seeing this Error");
                }
                is_reloading = false;
                temp_reload = reload_speed;
            }
        }
    }

    private void Reload()
    {
        if (magazine_count >= max_magazine_size) return;

        else if (magazine_count == 0 && carried_magazine_temp <= 0)
        {
            Debug.Log("No Ammo");
            return;
        }

        else if(magazine_count > 0 && carried_magazine_temp <= 0)
        {
            Debug.Log("Don't have ammo to reload");
            return;
        }

        if (magazine_count < max_magazine_size && carried_magazine_temp > 0)
        {
            is_reloading = true;
        }
    }
    private void Check_If_Ready()
    {
        fire_rate -= Time.deltaTime;

        if(fire_rate <= 0)
        {
            ready_To_fire = false;
        }
    }
    public Vector3 Calculate_Shooting_Vector()
    {
        Vector3 target = (Gun_Exit.transform.right).normalized;
        return target;
    }
}
