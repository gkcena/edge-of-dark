using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [Header("Skydome")]
    public Transform skydome;

    [Header("Time Settings")]
    public float dayDuration = 180f;
    [Range(0f, 1f)] public float timeOfDay;

    [Header("Light")]
    public Light sun;

    [Header("Ambient Settings")]
    public float dayAmbientIntensity = 1.2f;
    public float nightAmbientIntensity = 0.2f;

    [Header("Fog Settings")]
    public Color dayFogColor = new Color(0.6f, 0.7f, 0.8f);
    public Color nightFogColor = new Color(0.02f, 0.02f, 0.08f);
    public float dayFogDensity = 0.005f;
    public float nightFogDensity = 0.02f;

    [Header("Sun Settings")]
    public float daySunIntensity = 1.2f;
    public float nightSunIntensity = 0.05f;

    [Header("Debug")]
    public KeyCode toggleKey = KeyCode.N;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
    }

    void Update()
    {
        // TEST TUŞU
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleDayNight();
        }

        // ZAMAN AKIŞI
        timeOfDay += Time.deltaTime / dayDuration;
        if (timeOfDay >= 1f) timeOfDay = 0f;

        UpdateSun();
        UpdateEnvironment();
    }

    void ToggleDayNight()
    {
        // Gündüz <-> Gece
        timeOfDay = (timeOfDay < 0.5f) ? 0.75f : 0.25f;
    }

    void UpdateSun()
    {
        // Zaman açısı
        float angle = timeOfDay * 360f;

        // Güneş (ışık)
        sun.transform.rotation = Quaternion.Euler(angle - 90f, 30f, 0f);

        // Skydome (ÇOK ÖNEMLİ: 180 derece offset)
        if (skydome != null)
        {
            skydome.rotation = Quaternion.Euler(angle + 90f, 0f, 0f);
        }

        // Işık şiddeti
        float sunFactor = Mathf.Clamp01(Mathf.Cos(timeOfDay * Mathf.PI * 2f));
        sun.intensity = Mathf.Lerp(nightSunIntensity, daySunIntensity, sunFactor);
    }


    void UpdateEnvironment()
    {
        float t = Mathf.Clamp01(Mathf.Cos(timeOfDay * Mathf.PI * 2f));

        // Ambient
        RenderSettings.ambientIntensity =
            Mathf.Lerp(nightAmbientIntensity, dayAmbientIntensity, t);

        // Fog
        RenderSettings.fogColor =
            Color.Lerp(nightFogColor, dayFogColor, t);

        RenderSettings.fogDensity =
            Mathf.Lerp(nightFogDensity, dayFogDensity, t);

        DynamicGI.UpdateEnvironment();
    }
}
