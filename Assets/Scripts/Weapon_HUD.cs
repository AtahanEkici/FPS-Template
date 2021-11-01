using UnityEngine;
using TMPro;
public class Weapon_HUD : MonoBehaviour
{
	public bool Expending = true;
	public bool do_once = false;

    [SerializeField]private TextMeshProUGUI CurrentGun_Magazine_Hud;
	[SerializeField]private Player_Gun_Inventory inventory;
	[SerializeField]private Weapon current_weapon;
	[SerializeField]private float Animation_speed;
	[SerializeField]private Vector3 scale_rate = new Vector3(0.01f, 0.01f, 0.01f);
	[SerializeField]private float MAX_Scale = 1.5f;
	[SerializeField]private float MIN_Scale = 1.0f;
	[SerializeField]private string Gun_Text;
	private void Awake()
	{
		inventory = GetComponent<Player_Gun_Inventory>();
	}
    private void Update()
    {
		Update_Info();
		ScaleAnimation(Time.deltaTime);
	}
    private void LateUpdate()
    {
		Update_HUD();
	}
	private void Update_Info()
    {
		GameObject temp = inventory.GetCurrentWeapon();

		if (temp == null)
        {
			current_weapon = null;
			return;
		}	

		else
		{
			current_weapon = temp.GetComponent<Weapon>();
		}
	}

	private void Update_HUD()
	{
		if (current_weapon == null)
        {
			CurrentGun_Magazine_Hud.gameObject.SetActive(false);
		}
		else
        {
			CurrentGun_Magazine_Hud.gameObject.SetActive(true);
			CurrentGun_Magazine_Hud.text = "" + current_weapon.magazine_count + " / " + current_weapon.carried_magazine_temp + "";

			if(Gun_Text != CurrentGun_Magazine_Hud.text && current_weapon.is_reloading == false)
            {
				do_once = true;
			}
		}
	}

	private void ScaleAnimation(float a)
	{
		if (current_weapon == null) return;

		if (Expending && do_once)
		{
			CurrentGun_Magazine_Hud.gameObject.transform.localScale += scale_rate * a * Animation_speed;

			float X = CurrentGun_Magazine_Hud.gameObject.transform.localScale.x;

			if (X >= MAX_Scale)
			{
				Expending = false;
			}
		}
		else if (Expending == false && do_once)
		{
			CurrentGun_Magazine_Hud.gameObject.transform.localScale -= scale_rate * a * Animation_speed;

			float X = CurrentGun_Magazine_Hud.gameObject.transform.localScale.x;

			if (X <= MIN_Scale)
			{
				Expending = true;
				do_once = false;
				Gun_Text = CurrentGun_Magazine_Hud.text;
			}
		}
	}
}
