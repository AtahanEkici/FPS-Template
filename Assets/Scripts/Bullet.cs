using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;

    [SerializeField] private bool Allowed_To_Hit_Same_Object = false;

    private TrailRenderer tr;
    private List<GameObject> hit_list = new List<GameObject>();

    [SerializeField]private Color start_Color;
    [SerializeField]private Color end_Color;
    [SerializeField]private Gradient gradient;

    [SerializeField]private uint Max_Ricochet = 0;

    private uint ricochet_count = 0;

    private void Awake()
    {
        tr = GetComponent<TrailRenderer>();
        gradient = new Gradient();
    }
    private void Start()
    {
        tr.startColor = start_Color;
        tr.endColor = end_Color;
        
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(start_Color, 0.0f), new GradientColorKey(end_Color, 1.0f) },new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) });
        tr.colorGradient = gradient;
    }

    private bool Find_In_List(GameObject obj)
    {
        for(int i=0;i<hit_list.Count;i++)
        {
            if(obj == hit_list[i])
            {
                return true;
            }
        }
        return false;
    }

    private void Ricochet()
    {
        ricochet_count++;

        if (ricochet_count > Max_Ricochet)
        {
            Destroy(gameObject);
        }
    }

    private void Collision_Control(GameObject go)
    {
        if(Allowed_To_Hit_Same_Object)
        {
            Ricochet();
        }

        else if (Find_In_List(go)) return;

        else
        {
            Ricochet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collision_Control(collision.gameObject);
    }
}
