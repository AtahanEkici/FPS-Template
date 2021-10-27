using UnityEditor;
using UnityEngine;


public class Player_Gun_Inventory : MonoBehaviour
{
    public GameObject[] guns = new GameObject[4];
    public static GameObject current_gun;

    private readonly static string gun_tag = "Gun";
    private static float temp_timer;
    private static Player_Gun_Inventory _instance;

    [SerializeField]private bool lerping = false;
    [SerializeField]private int recently_dropped_ID = 0;
    [SerializeField]private float ID_Clear_Time = 2f;
    [SerializeField]private Transform Weapon_pos;
    [SerializeField]private Transform Gun_pos;
    [SerializeField]private float lerp_speed = 10f;
    private static Player_Gun_Inventory Instance
    {
        get { return _instance; }
    }
    private void Start()
    {
        temp_timer = ID_Clear_Time;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag(gun_tag))
        {
            if(recently_dropped_ID != collision.gameObject.GetInstanceID())
            {
                Equip_The_Gun(collision.gameObject, Gun_pos.position, Gun_pos.rotation);
            }
        }
    }
    private void Update()
    {
        Drop_The_Gun();
        Lerp_Transform(Weapon_pos, Gun_pos, Time.deltaTime * lerp_speed, current_gun);
        Clear_Instance_ID();
    }

    public GameObject GetCurrentWeapon()
    {
        if (Is_Empty()) return null;

        GameObject go = null;

        for (int i = 0; i < guns.Length; i++)
        {
            if (guns[i].GetComponent<Weapon>().is_Equipped)
            {
                go = guns[i].gameObject;
                current_gun = guns[i].gameObject;
                return go;
            }
        }
        return go;
    }
    private bool Has_Empty_Space()
    {
        for(int i=0; i< guns.Length;i++)
        {
            if(guns[i] == null)
            {
                return true;
            }
        }
        return false;
    }
    private void Clear_Instance_ID()
    {
        if(recently_dropped_ID == 0) return;

        ID_Clear_Time -= Time.deltaTime;

        if(ID_Clear_Time <= 0)
        {
            recently_dropped_ID = 0;
            ID_Clear_Time = temp_timer;
        }
    }
    private void Lerp_Transform(Transform t1, Transform t2, float t, GameObject go)
    {
        if (lerping && Weapon_pos != null)
        {
            t1.position = Vector3.Lerp(t1.position, t2.position, t);
            t1.rotation = Quaternion.Lerp(t1.rotation, t2.rotation, t);

            if (Quaternion.Dot(t1.rotation, t2.rotation) > 0.9999f)
            {
                if (Vector3.Dot(t1.position, t2.position) > 0.9999f)
                {
                    lerping = false;
                    go.transform.localScale = Vector3.one;
                    go.transform.localRotation = Quaternion.identity;
                }
            }
        } 
    }
    private void Equip_The_Gun(GameObject go, Vector3 pos, Quaternion rotation)
    {
        if (Has_Empty_Space() == false) return;

        go.GetComponent<Collider>().enabled = false;

        Rigidbody rb = go.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.detectCollisions = false;
        rb.isKinematic = true;

        Weapon temp = go.GetComponent<Weapon>();

        if (Is_Empty())
        {
            temp.is_Equipped = true; // Equip gun //
        }
        else
        {
            go.SetActive(false); // if there is a currently equipped weapon simply hide the gun //
        }

        Rotation.Set_Rotation(go.gameObject.name); // change gun_pos rotation according to gun equipped //
        Weapon_pos = go.transform;
        current_gun = go;
        go.transform.parent = Gun_pos;
        go.GetComponent<Weapon>().is_Equipped = true;
        lerping = true;
        Add_To_Guns(go);
    }
    private int FindEquipped()
    {
        if (Is_Empty()) return -1;

        for (int i = 0; i < guns.Length; i++)
        {
            if (guns[i] != null) // the current holdster should not be empty //
            {
                if (guns[i].GetComponent<Weapon>().is_Equipped == true)// If not empty check for currently equipped weapon //
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private void Drop_The_Gun()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            if (Is_Empty() == false) // the gun holdster should not be empty //
            {
                for (int i = 0; i < guns.Length; i++)
                {
                    if (guns[i] != null) // the current holdster should not be empty //
                    {
                        Weapon temp = guns[i].GetComponent<Weapon>();

                        if (temp.is_Equipped == true)// If not empty check for currently equipped weapon //
                        {
                            temp.Cancel_Reload();

                            MeshCollider meshy = guns[i].GetComponent<MeshCollider>();
                            meshy.convex = true;
                            meshy.enabled = true;

                            Rigidbody rb = guns[i].GetComponent<Rigidbody>();
                            rb.detectCollisions = true;
                            rb.isKinematic = false;
                            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                            recently_dropped_ID = guns[i].GetInstanceID();

                            guns[i].transform.parent = null;

                            rb.AddForce((transform.forward + guns[i].transform.forward) * Random.Range(5f, 8f), ForceMode.Impulse); // yeet the gun //

                            temp.is_Equipped = false;

                            guns[i] = null;
                            current_gun = null;
                            Weapon_pos = null;

                            Rotation.Set_Rotation("Default");
                        }
                    }
                }
            }
        }
    }
    public GameObject Find_Gun_By_Name(string given_gun_name)
    {
        if (Is_Empty()) return null;

        for(int i=0;i<guns.Length;i++)
        {
            if(guns[i].name.Contains(given_gun_name))
            {
                return guns[i];
            }
        }
        return null;
    }

    private bool Is_Empty()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            if (guns[i] != null)
            {
                return false;
            }
        }
        return true;
    }
    private void Add_To_Guns(GameObject go)
    {
        for(int i=0;i<guns.Length;i++)
        {
            if(guns[i] == null)
            {
                guns[i] = go;
                break;
            }          
        }
    }
    private void Shuffle_Guns()
    {
        if (Is_Empty()) return;

        int equipped_gun = FindEquipped();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (equipped_gun == 0 || equipped_gun == -1)
            {
                return;
            }

            else
            {
                for (int i = 0; i < guns.Length; i++)
                {
                    if (Gun_pos.gameObject.transform.GetChild(i).GetComponent<Weapon>().is_Equipped)
                    {
                        Gun_pos.gameObject.transform.GetChild(i).gameObject.SetActive(false);
                        i = 0;
                    }

                    if (ReferenceEquals(guns[0].gameObject, Gun_pos.gameObject.transform.GetChild(i)))
                    {
                        gameObject.transform.GetChild(i).gameObject.SetActive(true);
                        guns[0].GetComponent<Weapon>().is_Equipped = true;
                    }
                } 
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (equipped_gun == 1 || equipped_gun == -1)
            {
                return;
            }

            else
            {
                for (int i = 0; i < guns.Length; i++)
                {
                    if (Gun_pos.gameObject.transform.GetChild(i).GetComponent<Weapon>().is_Equipped)
                    {
                        Gun_pos.gameObject.transform.GetChild(i).gameObject.SetActive(false);
                        i = 0;
                    }

                    if (ReferenceEquals(guns[0].gameObject, Gun_pos.gameObject.transform.GetChild(i)))
                    {
                        gameObject.transform.GetChild(i).gameObject.SetActive(true);
                        guns[1].GetComponent<Weapon>().is_Equipped = true;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (equipped_gun == 2 || equipped_gun == -1)
            {
                return;
            }

            else
            {
                for (int i = 0; i < guns.Length; i++)
                {
                    if (Gun_pos.gameObject.transform.GetChild(i).GetComponent<Weapon>().is_Equipped)
                    {
                        Gun_pos.gameObject.transform.GetChild(i).gameObject.SetActive(false);
                        i = 0;
                    }

                    if (ReferenceEquals(guns[0].gameObject, Gun_pos.gameObject.transform.GetChild(i)))
                    {
                        gameObject.transform.GetChild(i).gameObject.SetActive(true);
                        guns[2].GetComponent<Weapon>().is_Equipped = true;
                    }
                }  
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (equipped_gun == 3 || equipped_gun == -1)
            {
                return;
            }

            else
            {
                for (int i = 0; i < guns.Length; i++)
                {
                    if (Gun_pos.gameObject.transform.GetChild(i).GetComponent<Weapon>().is_Equipped)
                    {
                        Gun_pos.gameObject.transform.GetChild(i).gameObject.SetActive(false);
                        i = 0;
                    }

                    if (ReferenceEquals(guns[0].gameObject, Gun_pos.gameObject.transform.GetChild(i)))
                    {
                        gameObject.transform.GetChild(i).gameObject.SetActive(true);
                        guns[0].GetComponent<Weapon>().is_Equipped = true;
                    }
                }  
            }
        }
    }
}