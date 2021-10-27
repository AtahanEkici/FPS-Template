using UnityEngine;

public class Ammo : MonoBehaviour
{
    public GameObject prefab;
    public Weapon intended_weapon;

    [SerializeField]MeshRenderer[] ren;
    [SerializeField]Material glow_Material;
    [SerializeField]private string for_the_gun = "Anaconda";
    [SerializeField]private uint Amount = 1;
    [SerializeField]private uint Ammo_Percantage = 30;
    [SerializeField]private Player_Gun_Inventory pgi;
    [SerializeField]private Material[] initial_Materials;
    [SerializeField]private Material[] glow_materials;

    private static readonly string player_tag = "Player";

    private void Awake()
    {
        uint intended_weapon_magazine_count = intended_weapon.max_carried_magazine;
        if (Ammo_Percantage > 100) Ammo_Percantage = 100;
        Amount = (uint)Random.Range(1, (intended_weapon_magazine_count * Ammo_Percantage / 100));
        for_the_gun = intended_weapon.gameObject.name;
        GameObject go = GameObject.FindGameObjectsWithTag(player_tag)[0];
        pgi = go.GetComponent<Player_Gun_Inventory>();
        ren = GetComponentsInChildren<MeshRenderer>();
        initial_Materials = ren[0].materials;
    }

    public void Glow()
    {
        GameObject go = pgi.GetCurrentWeapon();

        if (go == null) return;

        Weapon current = go.GetComponent<Weapon>();

        uint absolute_max = current.max_carried_magazine + current.max_magazine_size;
        uint current_ammo_count = current.carried_magazine_temp + current.magazine_count;
        uint required = absolute_max - current_ammo_count;

        if (required != 0)
        {
            for (int i=0;i< ren.Length;i++)
            {
                ren[i].materials = glow_materials;
            }
        }

        else
        {
            for (int i = 0; i < ren.Length; i++)
            {
                ren[i].materials = initial_Materials;
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag(player_tag))
        {
            if (pgi.Find_Gun_By_Name(for_the_gun) == null) return;

            GameObject go = pgi.Find_Gun_By_Name(for_the_gun);

            Weapon temp = go.GetComponent<Weapon>();

            uint absolute_max = temp.max_carried_magazine + temp.max_magazine_size;
            uint current_ammo_count = temp.carried_magazine_temp + temp.magazine_count;
            uint required = absolute_max - current_ammo_count;

            if (required <= 0)
            {
                return;
            }

            else if(Amount <= 0)
            {
                Destroy(gameObject);
            }

            else if (required < Amount)
            {
                Amount -= required;
                temp.carried_magazine_temp += required;
            }

            else if (required >= Amount)
            {
                temp.carried_magazine_temp += Amount;
                Destroy(gameObject);
            }

            else
            {
                Debug.LogError("Ammo Error please check");
            }
        }
    }
}
