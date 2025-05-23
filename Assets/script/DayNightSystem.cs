using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Using TextMeshPro

public class DayNightSystem : MonoBehaviour
{
    [Header("Time Settings")]
    public float currentTime = 0f;
    public float dayLengthMinutes;

    [Header("Day Counter")]
    public int currentDay = 1;

    [Header("Sun Object")]
    public Transform sunTransform;

    [Header("UI Display")]
    public TextMeshProUGUI dayTextDisplay; // Your TextMeshPro element
    public float dayTextPopupDuration = 3f; // How long the "Day X" text stays visible (e.g., 3 seconds)

    private float rotationSpeed;
    private float secondsInFullDay;
    private bool isDayCycleActive = false;
    private Coroutine dayDisplayPopupCoroutine; // To manage the text popup

    void Start()
    {
        secondsInFullDay = dayLengthMinutes * 60f;
        rotationSpeed = 360f / secondsInFullDay;

        if (sunTransform == null)
        {
            sunTransform = transform;
            Debug.LogWarning("Sun Transform not explicitly assigned in DayNightSystem. Assuming this script is on the sun object.");
        }

        // Ensure the day text UI element is hidden initially.
        // It will be shown as a popup when needed.
        if (dayTextDisplay != null)
        {
            dayTextDisplay.gameObject.SetActive(false);
        }
        // We do not call UpdateDayDisplay() here; the first popup will trigger on button press.

        Debug.Log("DayNightSystem initialized. Waiting for 'Start Day' button press. Current day: " + currentDay + ". Day text initially hidden.");
    }

    void Update()
    {
        if (!isDayCycleActive)
        {
            return;
        }

        currentTime += Time.deltaTime;

        if (sunTransform != null)
        {
            sunTransform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }

        if (currentTime >= secondsInFullDay)
        {
            currentTime -= secondsInFullDay;
            currentDay++;
            UpdateDayDisplay(); // Trigger the popup for the new day
            Debug.Log("Day " + currentDay + " has begun.");
        }
    }

    public void StartDayCycleButtonPressed()
    {
        if (!isDayCycleActive)
        {
            isDayCycleActive = true;
            Debug.Log("Start Day button pressed! Day " + currentDay + " cycle is now active.");

            // Trigger the popup for the current day when the button is pressed
            UpdateDayDisplay();
        }
        else
        {
            Debug.Log("Day cycle is already active.");
        }
    }

    /// <summary>
    /// Manages the display of the day text as a temporary popup.
    /// </summary>
    void UpdateDayDisplay()
    {
        if (dayTextDisplay == null)
        {
            Debug.LogWarning("Day Text Display (TextMeshPro) NOT ASSIGNED in DayNightSystem script. Cannot show day popup.");
            return;
        }

        // If a popup is already showing, stop it before starting a new one
        if (dayDisplayPopupCoroutine != null)
        {
            StopCoroutine(dayDisplayPopupCoroutine);
        }
        // Start the new popup coroutine
        dayDisplayPopupCoroutine = StartCoroutine(ShowDayTextPopupCoroutine());
    }

    /// <summary>
    /// Coroutine to show the day text for a set duration, then hide it.
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowDayTextPopupCoroutine()
    {
        Debug.Log("Popup: Displaying 'Day: " + currentDay + "'");
        dayTextDisplay.text = "Day: " + currentDay;
        dayTextDisplay.gameObject.SetActive(true); // Show the text

        yield return new WaitForSeconds(dayTextPopupDuration); // Wait for the specified duration

        dayTextDisplay.gameObject.SetActive(false); // Hide the text
        Debug.Log("Popup: 'Day: " + currentDay + "' text hidden after duration.");
        dayDisplayPopupCoroutine = null; // Clear the reference
    }
}