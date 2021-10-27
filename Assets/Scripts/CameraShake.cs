using UnityEngine;
public class CameraShake : MonoBehaviour
{
    [SerializeField]
    Vector3 maximumTranslationShake = Vector3.one;
    private float ang;
    private Vector3 maximumAngularShake;
    [SerializeField]
    float traumaExponent = 1;
    [SerializeField]
    float recoverySpeed = 1;
    private float trauma;
    private float seed;
    private float frequency = 25;
    private static CameraShake _instance;

    public static CameraShake Instance
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
            seed = Random.value;
    }
    private void Start()
    {
        maximumAngularShake = Vector3.one* ang;
    }
    private void Update()
    {
        float shake = Mathf.Pow(trauma, traumaExponent);
        transform.localPosition = new Vector3(
            maximumTranslationShake.x * (Mathf.PerlinNoise(seed, Time.time * frequency) * 2 - 1),
            maximumTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.time * frequency) * 2 - 1),
            maximumTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.time * frequency) * 2 - 1)
        ) * shake;

        transform.localRotation = Quaternion.Euler(new Vector3(
            maximumAngularShake.x * (Mathf.PerlinNoise(seed + 3, Time.time * frequency) * 2 - 1),
            maximumAngularShake.y * (Mathf.PerlinNoise(seed + 4, Time.time * frequency) * 2 - 1),
            maximumAngularShake.z * (Mathf.PerlinNoise(seed + 5, Time.time * frequency) * 2 - 1)
        ) * shake);

        trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);
    }
    public void InduceStress(float frequency,float ang,float stress)
    {
        this.frequency = frequency;
        this.ang = ang;
        trauma = Mathf.Clamp01(trauma + stress);
    }
}
