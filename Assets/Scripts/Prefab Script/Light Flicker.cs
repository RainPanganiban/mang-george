using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    private UnityEngine.Rendering.Universal.Light2D light2D; // Reference to 2D Light

    [Header("Flicker Settings")]
    public float minIntensity = 0.5f; // Minimum light intensity
    public float maxIntensity = 1.5f; // Maximum light intensity
    public float flickerSpeed = 0.1f; // Time between flickers

    void Start()
    {
        light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>(); // Get the Light2D component
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true) // Infinite loop to keep flickering
        {
            float randomIntensity = Random.Range(minIntensity, maxIntensity); // Get a random intensity
            light2D.intensity = randomIntensity; // Apply it
            yield return new WaitForSeconds(Random.Range(flickerSpeed * 0.5f, flickerSpeed * 1.5f)); // Random flicker timing
        }
    }
}
