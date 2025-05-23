using UnityEngine;


public class AutomaticDoor : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator; // Assign the door's Animator here

    private void Awake()
    {
        // Get the Animator component if not assigned in the inspector
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }

        // If still no animator, log an error
        if (doorAnimator == null)
        {
            Debug.LogError("AutomaticDoor script requires an Animator component assigned or on the same GameObject.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Entered by: " + other.gameObject.name + " Tag: " + other.tag); 

        // Check if the entering object has the "Player" or "NPC" tag
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("isOpen", true); // Set the boolean parameter to open
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object has the "Player" or "NPC" tag
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("isOpen", false); // Set the boolean parameter to close
            }
        }
    }
}