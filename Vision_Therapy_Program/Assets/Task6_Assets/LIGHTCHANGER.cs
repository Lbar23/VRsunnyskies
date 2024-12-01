using UnityEngine;

public class RandomLightUp : MonoBehaviour
{
    public float minTime = 1f;   // Minimum time before the next random light-up
    public float maxTime = 5f;   // Maximum time before the next random light-up
    public float glowIntensity = 5f;  // Emission intensity to make the object light up
    public Color glowColor = Color.white;  // Color of the glow
    public float glowDuration = 5f;  // Duration for which the object glows

    private Renderer[] renderers;

    // Store original colors to reset later
    private Color[] originalColors;

    void Start()
    {
        // Get all renderers under the parent
        renderers = GetComponentsInChildren<Renderer>();
        
        // Initialize array to store the original colors of each cube
        originalColors = new Color[renderers.Length];

        // Store the original color for each cube
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;  // Store the initial color
        }

        // Start the random light-up cycle
        InvokeRepeating("RandomLightUpCycle", 0f, Random.Range(minTime, maxTime));
    }

    void RandomLightUpCycle()
    {
        // Choose a random object to light up (Renderer)
        int randomIndex = Random.Range(0, renderers.Length);
        Renderer chosenRenderer = renderers[randomIndex];
        Material mat = chosenRenderer.material;

        // Toggle emission to make the object glow
        bool isEmissive = mat.IsKeywordEnabled("_EMISSION");
        if (!isEmissive)
        {
            // Turn on emission
            mat.SetColor("_EmissionColor", glowColor * glowIntensity);
            mat.EnableKeyword("_EMISSION");

            // Start a coroutine to reset the material back to its original color after the glow duration
            StartCoroutine(ResetGlowAfterDelay(chosenRenderer, mat));
        }
    }

    // Coroutine to reset the glow after a specified delay (5 seconds)
    private System.Collections.IEnumerator ResetGlowAfterDelay(Renderer renderer, Material material)
    {
        // Wait for the glow duration
        yield return new WaitForSeconds(glowDuration);

        // Reset the emission to the original color (or to black to turn off the glow)
        material.SetColor("_EmissionColor", Color.black);
        material.DisableKeyword("_EMISSION");
    }
}
