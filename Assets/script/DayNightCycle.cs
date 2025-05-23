using UnityEngine;
using System.Collections; // Required for Coroutines
using UnityEngine.UI; // Required for UI elements like Text and Panel

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight; // Drag your Directional Light GameObject here
    public float transitionDuration = 10f; // Duration of each transition (e.g., night to day)
    public float dayDuration = 120f; // Duration the light stays in the day state (2 minutes)
    public float popUpDisplayDuration = 5f; // Duration the pop-up panel stays visible (5 seconds)

    public Text dayDisplay; // Drag the UI Text element for displaying the day here
    public GameObject popUpPanel; // Optional: Drag a UI Panel GameObject here to show/hide the message

    // Store the night and day settings
    private float nightIntensity;
    private Color nightColor;
    private Quaternion nightRotation; // Using Quaternion for rotation

    private float dayIntensity;
    private Color dayColor;
    private Quaternion dayRotation; // Using Quaternion for rotation

    private int dayCount = 0; // Start the day count at 0 so the first increment makes it 1

    private Coroutine currentCycleCoroutine = null; // To keep track of the ongoing full cycle coroutine

    void Awake()
    {
        Debug.Log("DayNightCycle script Awake started.");

        if (directionalLight != null)
        {
            Debug.Log("Directional Light assigned. Capturing night settings.");
            // Capture initial "night" settings from the editor
            nightIntensity = directionalLight.intensity;
            nightColor = directionalLight.color;
            nightRotation = directionalLight.transform.rotation; // Capture as Quaternion

            // --- Define your desired "day" settings based on your screenshot values ---
            dayIntensity = 2f; // Set day intensity to 2
            // Set day color using RGB values (divide by 255 for Unity's Color format)
            dayColor = new Color(248f / 255f, 245f / 255f, 224f / 255f);
            // Set day rotation using Euler angles (convert Euler to Quaternion)
            dayRotation = Quaternion.Euler(120f, 50f, 0f);
        }
        else
        {
            Debug.LogError("Directional Light is NOT assigned in the Inspector!");
        }

        // Initialize the day display
        // The first day count update will happen when the button is pressed
        if (dayDisplay != null)
        {
            // Optionally clear initial text or set to a default like "Press Start"
            dayDisplay.text = "Press Start";
            Debug.Log("Day Display Text assigned. Initial text set to 'Press Start'.");
        }
        else
        {
            Debug.LogError("Day Display Text is NOT assigned in the Inspector!");
        }


        // Hide the pop-up panel initially
        if (popUpPanel != null)
        {
            Debug.Log("Pop Up Panel assigned. Hiding panel.");
            popUpPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Pop Up Panel is NOT assigned in the Inspector!");
        }

        Debug.Log("DayNightCycle script Awake finished.");
    }

    // This public function will be called by the button's On Click event
    // It now starts the full day-night cycle
    public void StartDayNightCycle()
    {
        Debug.Log("StartDayNightCycle button function called.");

        // Stop any ongoing cycle before starting a new one
        if (currentCycleCoroutine != null)
        {
            Debug.Log("Stopping existing coroutine.");
            StopCoroutine(currentCycleCoroutine);
        }

        // Start the full day-night cycle coroutine
        currentCycleCoroutine = StartCoroutine(RunDayNightCycle());
        Debug.Log("RunDayNightCycle coroutine started.");
    }

    // Coroutine to handle the full transition: Start Day -> Night -> Day -> Wait -> Pop-up -> Night
    IEnumerator RunDayNightCycle()
    {
        Debug.Log("RunDayNightCycle coroutine executing.");

        // Declare variables once at the beginning
        float timer = 0f;
        float startIntensity;
        Color startColor;
        Quaternion startRotation;

        float endIntensity;
        Color endColor;
        Quaternion endRotation;


        // --- Increment the day count and display text NOW, when the button is pressed ---
        dayCount++;
        Debug.Log("Attempting to update day count and text AT START OF CYCLE. Current dayCount: " + dayCount);

        if (dayDisplay != null)
        {
            dayDisplay.text = "Day " + dayCount;
            Debug.Log("dayDisplay text updated to: " + dayDisplay.text);
        }
        else
        {
            Debug.LogError("dayDisplay is null when trying to update text AT START OF CYCLE!");
        }
        Debug.Log("Finished text update block AT START OF CYCLE.");


        // Ensure we start from the manually set night values at the beginning of a cycle
        if (directionalLight != null)
        {
            directionalLight.intensity = nightIntensity;
            directionalLight.color = nightColor;
            directionalLight.transform.rotation = nightRotation;
            Debug.Log("Set light to initial night settings.");
        }
        else
        {
            Debug.LogError("Directional Light is null in coroutine!");
        }


        // --- Transition from Night to Day ---
        timer = 0f; // Reset timer for the first transition
        startIntensity = nightIntensity;
        startColor = nightColor;
        startRotation = nightRotation;

        endIntensity = dayIntensity;
        endColor = dayColor;
        endRotation = dayRotation;

        Debug.Log("Starting Night to Day transition...");

        while (timer < transitionDuration)
        {
            float t = timer / transitionDuration;
            // Optional: Apply an easing function to 't'
            // t = Mathf.SmoothStep(0f, 1f, t);

            if (directionalLight != null)
            {
                directionalLight.intensity = Mathf.Lerp(startIntensity, endIntensity, t);
                directionalLight.color = Color.Lerp(startColor, endColor, t);
                directionalLight.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure the values are exactly the day values at the end of the transition
        if (directionalLight != null)
        {
            directionalLight.intensity = dayIntensity;
            directionalLight.color = dayColor;
            directionalLight.transform.rotation = dayRotation;
        }

        Debug.Log("Night to Day transition complete. Light is now at day settings.");

        // --- Show Pop-up Panel ---
        if (popUpPanel != null)
        {
            popUpPanel.SetActive(true);
            Debug.Log("Pop-up panel SET ACTIVE(true) at start of day.");
        }
        else
        {
            Debug.LogError("popUpPanel is null when trying to show panel at start of day!");
        }

        // Wait for the pop-up display duration
        yield return new WaitForSeconds(popUpDisplayDuration);
        Debug.Log("Pop-up display duration complete.");

        // Hide the pop-up panel
        if (popUpPanel != null)
        {
            popUpPanel.SetActive(false);
            Debug.Log("Pop-up panel SET ACTIVE(false) after display duration.");
        }
        else
        {
            Debug.LogError("popUpPanel is null when trying to hide panel after display duration!");
        }


        // --- Stay in Day State ---
        Debug.Log("Day reached. Staying for " + dayDuration + " seconds.");
        yield return new WaitForSeconds(dayDuration);
        Debug.Log("Day duration complete. Transitioning back to night.");


        // --- Transition from Day back to Night ---
        timer = 0f; // Reset timer for the return transition
        startIntensity = dayIntensity;
        startColor = dayColor;
        startRotation = dayRotation;

        endIntensity = nightIntensity;
        endColor = nightColor;
        endRotation = nightRotation;

        Debug.Log("Starting Day to Night transition...");


        while (timer < transitionDuration)
        {
            float t = timer / transitionDuration;
            // Optional: Apply an easing function
            // t = Mathf.SmoothStep(0f, 1f, t);

            if (directionalLight != null)
            {
                directionalLight.intensity = Mathf.Lerp(startIntensity, endIntensity, t);
                directionalLight.color = Color.Lerp(startColor, endColor, t);
                directionalLight.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure values are exact at the very end
        if (directionalLight != null)
        {
            directionalLight.intensity = endIntensity;
            directionalLight.color = endColor;
            directionalLight.transform.rotation = endRotation;
        }

        Debug.Log("Day to Night transition complete. Light is now at night settings. Cycle complete.");

        // Nullify the coroutine reference as the cycle is complete
        currentCycleCoroutine = null;
    }
}