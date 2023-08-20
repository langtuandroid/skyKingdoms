using System;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;
    [SerializeField] private bool isGamePlay;
    private Light sun;

    private float dayDuration = 840f; 

    private Material currentSkybox;

    private void Awake()
    {
        sun = FindObjectOfType<Light>();
    }

    private void Start()
    {
        if(sun == null) return;
        currentSkybox = daySkybox; 
        RenderSettings.skybox = currentSkybox; 
    }

    void Update()
    {
        if (!isGamePlay) return;
        
        if (sun == null)  sun = FindObjectOfType<Light>();//Todo sacar de aqui
        
        float time = Time.time % dayDuration;
        float sunAngle = Mathf.Lerp(-90f, 270f, time / dayDuration);

        sun.transform.rotation = Quaternion.Euler(sunAngle, 0, 0);

        float intensity = Mathf.Clamp01(1.5f - Mathf.Abs(0.5f - (time / dayDuration)) * 3); // Ajuste de intensidad
        sun.intensity = intensity;

        // Cambiar el Skybox en función del tiempo
        if ((time / dayDuration >= 0.25f && time / dayDuration < 0.3f) && currentSkybox != daySkybox) // Cambiar al Skybox de día a las 7:00
        {
            currentSkybox = daySkybox;
            RenderSettings.skybox = currentSkybox;
        }
        else if ((time / dayDuration >= 0.7f && time / dayDuration < 0.75f) && currentSkybox != nightSkybox) // Cambiar al Skybox de noche a las 21:00
        {
            currentSkybox = nightSkybox;
            RenderSettings.skybox = currentSkybox;
        }
        
        Debug.Log("Hora actual: " + GetFormattedTime(time));
    }

    private string GetFormattedTime(float time)
    {
        float hours = 24f * (time / dayDuration); // Calcula las horas actuales
        float minutes = 60f * (hours - Mathf.Floor(hours)); // Calcula los minutos actuales

        return Mathf.Floor(hours).ToString("00") + ":" + Mathf.Floor(minutes).ToString("00");
    }
}